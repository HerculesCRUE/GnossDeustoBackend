using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Inference;
using VDS.RDF.Update;

namespace API_DISCOVER.Models.Utility
{
    public static class Discover
    {
        private readonly static List<Disambiguation> mDisambiguationConfigs = LoadDisambiguationConfigs();
        private readonly static float mMaxScore = 0.9f;
        private readonly static float mMinScore = 0.7f;
        private readonly static ConfigSparql mConfigSparql = new ConfigSparql();
        private readonly static string mSPARQLEndpoint = mConfigSparql.GetEndpoint();
        private readonly static string mGraph = mConfigSparql.GetGraph();
        private readonly static string mQueryParam = mConfigSparql.GetQueryParam();

        //TODO, pendiente de implementar despues del CVN
        private readonly static string mPropertyRohIdentifier = "http://purl.org/dc/terms/identifier";

        /// <summary>
        /// Realiza el desubrimiento sobre un RDF
        /// </summary>
        /// <param name="pRDFFile">Fichero RDF</param>
        /// <param name="pDissambiguationProcessed">Indica si están ya resueltos los problemas de desambiguación</param>
        /// <returns>DiscoverResult con los datos del descubrimiento</returns>
        public static DiscoverResult Init(string pRDFFile, bool pDissambiguationProcessed)
        {
            DateTime discoverInitTime = DateTime.Now;
            //Cargamos la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Config/Ontology/roh-v2.owl");

            //Cargamos datos del RDF
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(pRDFFile, new RdfXmlParser());


            //Cargamos el razonador para inferir datos en la ontología
            RohRdfsReasoner reasoner = new RohRdfsReasoner();
            reasoner.Initialise(ontologyGraph);

            //Cargamos los datos con inferencia
            RohGraph dataInferenceGraph = dataGraph.Clone();
            reasoner.Apply(dataInferenceGraph);

            //Almacenamos el nº de entidades descubiertas por cada método sólo como información
            HashSet<string> discoveredEntitiesWithSubject = new HashSet<string>();
            Dictionary<string, string> discoveredEntitiesWithIds = new Dictionary<string, string>();
            Dictionary<string, KeyValuePair<string, float>> discoveredEntitiesWithBBDD = new Dictionary<string, KeyValuePair<string, float>>();

            //Almacenamos las entidades que han sido descubiertas, la clave es el sujeto original y el valor es el nuevo valor            
            Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();

            //Almacenamos las entidades con dudas acerca de su descubrimiento
            Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbability = new Dictionary<string, Dictionary<string, float>>();

            Dictionary<string, Dictionary<string, string>> orcidIntegration = new Dictionary<string, Dictionary<string, string>>();

            if (!pDissambiguationProcessed)
            {
                //Obtenemos los nombres de todas las personas que haya cargadas en la BBDD
                //Clave ID,
                //Valor nombre
                Dictionary<string, string> personsWithName = LoadPersonWithName();

                //Aquí se almacenarán los nombres de las personas del RDF, junto con los candidatos de la BBDD y su score
                Dictionary<string, Dictionary<string, float>> namesScore = new Dictionary<string, Dictionary<string, float>>();

                int numPrevDiscoveredEntities = discoveredEntityList.Count;
                bool firstPass = true;

                //Se realizarán este proceso iterativamente hasta que no haya ningún cambio en lo que a reconciliaciones se refiere
                while (firstPass || numPrevDiscoveredEntities != discoveredEntityList.Count)
                {
                    firstPass = false;
                    numPrevDiscoveredEntities = discoveredEntityList.Count;

                    Dictionary<string, HashSet<string>> entitiesRdfTypes;
                    Dictionary<string, string> entitiesRdfType;
                    Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;

                    //Preparamos los datos para proceder con la reconciliazción
                    PrepareData(dataGraph, reasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf, false);

                    //Carga los scores de las personas
                    LoadNamesScore(ref namesScore, personsWithName, dataInferenceGraph);

                    //0.- Macamos como reconciliadas aquellas que ya estén cargadas en la BBDD con los mismos identificadores
                    List<string> entidadesCargadas = LoadEntitiesDB(entitiesRdfType.Keys.ToList().Except(discoveredEntityList.Keys.Union(discoveredEntityList.Values))).Keys.ToList();
                    foreach (string entitiID in entidadesCargadas)
                    {
                        discoveredEntityList.Add(entitiID, entitiID);
                        discoveredEntitiesWithSubject.Add(entitiID);
                    }

                    //1.- Realizamos reconciliación con los identificadores configurados (y el roh:identifier) y marcamos como reconciliadas las entidades seleccionadas para no intentar reconciliarlas posteriormente
                    Dictionary<string, string> entidadesReconciliadasConIdsAux = ReconciliateIDs(ref discoveredEntityList, ref entitiesRdfTypes, ref entitiesRdfType, ref disambiguationDataRdf, ref dataGraph, ref dataInferenceGraph, reasoner);
                    foreach (string id in entidadesReconciliadasConIdsAux.Keys)
                    {
                        discoveredEntitiesWithIds.Add(id, entidadesReconciliadasConIdsAux[id]);
                    }

                    //TODO considerar si efectuamos reconciliación dentro del RDF

                    //2.- Realizamos la reconciliación con los datos de la BBDD
                    Dictionary<string, KeyValuePair<string, float>> entidadesReconciliadasConBBDDAux = ReconciliateBBDD(ref discoveredEntityList, out discoveredEntitiesProbability, ref entitiesRdfTypes, ref entitiesRdfType, ref disambiguationDataRdf, ref dataGraph, ref dataInferenceGraph, reasoner, namesScore);
                    foreach (string id in entidadesReconciliadasConBBDDAux.Keys)
                    {
                        discoveredEntitiesWithBBDD.Add(id, entidadesReconciliadasConBBDDAux[id]);
                    }
                }

                //Integración con apis externos
                orcidIntegration=ORCIDIntegration(ref dataGraph, ref dataInferenceGraph, reasoner);
            }
            DateTime discoverEndTime = DateTime.Now;
            DiscoverResult resultado = new DiscoverResult(dataGraph, dataInferenceGraph, ontologyGraph, discoveredEntitiesWithSubject, discoveredEntitiesWithIds, discoveredEntitiesWithBBDD, discoveredEntitiesProbability, (discoverEndTime - discoverInitTime).TotalSeconds, orcidIntegration);

            return resultado;
        }

        /// <summary>
        /// Procesa los resultados del descubrimiento
        /// </summary>
        /// <param name="pDiscoverItem">Objeto con los datos de com procesar el proeso de descubrimiento</param>
        /// <param name="pDiscoverResult">Resultado de la aplicación del descubrimiento</param>
        /// <param name="pDiscoverItemBDService">Clase para gestionar las operaciones de las tareas de descubrimiento</param>
        /// <returns></returns>
        public static void Process(DiscoverItem pDiscoverItem, DiscoverResult pDiscoverResult, DiscoverItemBDService pDiscoverItemBDService)
        {
            /*
             En función del resultado obtenido se realiza una de las siguientes acciones:
                Si para alguna entidad hay más de un candidato que supere el umbral máximo o hay alguna entidad que supere el umbral mínimo pero no alcance el máximo se agregará el RDF a una BBDD junto con todos los datos necesarios para que el administrador decida como proceder.
                Si no estamos en el punto anterior
                    Se obtienen las entidades principales del RDF y se eliminan todos los triples que haya en la BBDD en los que aparezcan como sujeto u objeto.
                    Se eliminan todos los triples cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
                    Se vuelcan los triples a la BBDD.
             
             
             */

            if (pDiscoverItem.Publish)
            {
                if (pDiscoverResult.discoveredEntitiesProbability.Count > 0)
                {
                    //Hay dudas en la desambiguación, por lo que lo insertamos en la BBDD
                    pDiscoverItem.Status = DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString();
                    pDiscoverItem.DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
                    pDiscoverItem.DiscoverRdf = pDiscoverResult.GetDataGraphRDF();
                    foreach (string entityID in pDiscoverResult.discoveredEntitiesProbability.Keys)
                    {
                        DiscoverItem.DiscoverDissambiguation discoverDissambiguation = new DiscoverItem.DiscoverDissambiguation() { IDOrigin = entityID, DissambiguationCandiates = new List<DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate>() };
                        foreach (string candidateID in pDiscoverResult.discoveredEntitiesProbability[entityID].Keys)
                        {
                            discoverDissambiguation.DissambiguationCandiates.Add(new DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate() { IDCandidate = candidateID, Score = pDiscoverResult.discoveredEntitiesProbability[entityID][candidateID] });
                        }
                        pDiscoverItem.DissambiguationProblems.Add(discoverDissambiguation);
                    }
                }
                else
                {
                    //No hay problemas en la reconciliación por lo que procedemos

                    #region 1º Eliminamos de la BBD las entidades principales que aparecen en el RDF
                    List<string> mainEntities = new List<string>();
                    string query = @"select distinct * where{?s <http://purl.org/roh/mirror/foaf#primaryTopic> ""true""^^<http://www.w3.org/2001/XMLSchema#boolean>}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDiscoverResult.dataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        mainEntities.Add(sparqlResult["s"].ToString());
                    }

                    //Se eliminan todas las referncias a las entidades principales junto con sus blanknodes de forma recursiva
                    if (mainEntities.Count > 0)
                    {
                        foreach (string mainEntity in mainEntities)
                        {
                            DeleteEntity(mainEntity);
                        }
                    }

                    //Eliminamos el triple que marca las entidades principales para que no se inserte en la BBDD
                    {
                        TripleStore store = new TripleStore();
                        store.Add(pDiscoverResult.dataGraph);
                        SparqlUpdateParser parser = new SparqlUpdateParser();
                        //Actualizamos los sujetos
                        SparqlUpdateCommandSet updateSubject = parser.ParseFromString(
                                @"  DELETE { ?s ?p ?o. }
                                    WHERE 
                                    {
                                        ?s ?p ?o. FILTER(?p =<http://purl.org/roh/mirror/foaf#primaryTopic>)
                                    }");
                        LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                        processor.ProcessCommandSet(updateSubject);
                    }
                    #endregion

                    #region 2º Eliminamos todos los triples de la BBDD cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
                    {
                        //1º Obtnemos las propiedades monovaluadas de las clases
                        Dictionary<string, HashSet<string>> classMonovaluateProperty = new Dictionary<string, HashSet<string>>();

                        SparqlResultSet sparqlResultSet2 = (SparqlResultSet)pDiscoverResult.ontologyGraph.ExecuteQuery(
                            @"  select distinct ?class ?onProperty
                                {
                                    ?class a <http://www.w3.org/2002/07/owl#Class>.
                                    {
                                        ?class <http://www.w3.org/2000/01/rdf-schema#subClassOf> ?restriction.
                                    }
                                    UNION
                                    {
                                        ?class <http://www.w3.org/2000/01/rdf-schema#subClassOf> ?intersection.
                                        ?intersection <http://www.w3.org/2002/07/owl#intersectionOf> ?intersectionOf.
                                        ?intersectionOf <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest>*/<http://www.w3.org/1999/02/22-rdf-syntax-ns#first> ?restriction.
                                    }
                                    ?restriction a <http://www.w3.org/2002/07/owl#Restriction>.
                                    ?restriction <http://www.w3.org/2002/07/owl#onProperty> ?onProperty.
                                    {
                                        ?restriction ?propCardinality ""1""^^<http://www.w3.org/2001/XMLSchema#nonNegativeInteger>.
                                        FILTER(?propCardinality in (<http://www.w3.org/2002/07/owl#maxQualifiedCardinality>, <http://www.w3.org/2002/07/owl#qualifiedCardinality>,<http://www.w3.org/2002/07/owl#maxCardinality>,<http://www.w3.org/2002/07/owl#cardinality>))
                                    }
                                }");
                        foreach (SparqlResult sparqlResult in sparqlResultSet2.Results)
                        {
                            if (!classMonovaluateProperty.ContainsKey(sparqlResult["class"].ToString()))
                            {
                                classMonovaluateProperty.Add(sparqlResult["class"].ToString(), new HashSet<string>());
                            }
                            classMonovaluateProperty[sparqlResult["class"].ToString()].Add(sparqlResult["onProperty"].ToString());
                        }

                        //2º Obtenemos del grafo a cargar los triples (con ?s ?p) de las propiedades monovaluadas
                        Dictionary<string, HashSet<string>> entityMonovaluateProperty = new Dictionary<string, HashSet<string>>();
                        foreach (string clas in classMonovaluateProperty.Keys)
                        {
                            SparqlResultSet sparqlResultSet3 = (SparqlResultSet)pDiscoverResult.dataInferenceGraph.ExecuteQuery(
                                @$" select distinct ?entityID ?property
                                    {{
                                        ?entityID a <{clas}>.
                                        ?entityID ?property ?o.
                                        FILTER(?property in (<{ string.Join(">,<", classMonovaluateProperty[clas]) }>))
                                        FILTER(!isBlank(?entityID))
                                    }}");

                            foreach (SparqlResult sparqlResult in sparqlResultSet3.Results)
                            {
                                if (!entityMonovaluateProperty.ContainsKey(sparqlResult["entityID"].ToString()))
                                {
                                    entityMonovaluateProperty.Add(sparqlResult["entityID"].ToString(), new HashSet<string>());
                                }
                                entityMonovaluateProperty[sparqlResult["entityID"].ToString()].Add(sparqlResult["property"].ToString());
                            }
                        }

                        while (entityMonovaluateProperty.Count > 0)
                        {
                            //3º Ejecutamos las eliminaciones de 100 en 100
                            List<string> deletes = new List<string>();
                            foreach (string entityID in entityMonovaluateProperty.Keys.ToList())
                            {
                                string stringDelete = $@"   {{
                                                                ?s ?p ?o. 
                                                                FILTER(?s = <{entityID}> AND ?p in(<{string.Join(">,<", entityMonovaluateProperty[entityID])}>))
                                                            }}";
                                deletes.Add(stringDelete);
                                entityMonovaluateProperty.Remove(entityID);
                                if (deletes.Count >= 100)
                                {
                                    break;
                                }
                            }
                            string queryDeleteMainEntities = $@"    DELETE {{ ?s ?p ?o. }}
                                                                    WHERE 
                                                                    {{
                                                                        {{{string.Join("}UNION{", deletes)}}}
                                                                    }}";
                            SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, queryDeleteMainEntities, mQueryParam);
                        }
                    }
                    #endregion

                    //3º Insertamos los triples en la BBDD
                    SparqlUtility.LoadTriples(pDiscoverResult.GetDataGraphTriples(), mSPARQLEndpoint, mQueryParam, mGraph);

                    //4º Limpiamos los blanknodes huerfanos, o que no tengan triples
                    DeleteOrphanNodes();
                }
            }
            else if (pDiscoverItem.Status == DiscoverItem.DiscoverItemStatus.Pending.ToString())
            {
                //Actualizamos en BBDD
                DiscoverItem discoverItemBD = pDiscoverItemBDService.GetDiscoverItemById(pDiscoverItem.ID);
                if (discoverItemBD != null)
                {
                    discoverItemBD.DiscoverRdf = pDiscoverResult.GetDataGraphRDF();
                    discoverItemBD.Status = DiscoverItem.DiscoverItemStatus.Processed.ToString();
                    discoverItemBD.DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
                    if (pDiscoverResult.discoveredEntitiesProbability.Count > 0)
                    {
                        //Hay dudas en la desambiguación, por lo que lo almacenamos en la BBDD                    
                        discoverItemBD.Status = DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString();
                        foreach (string entityID in pDiscoverResult.discoveredEntitiesProbability.Keys)
                        {
                            DiscoverItem.DiscoverDissambiguation discoverDissambiguation = new DiscoverItem.DiscoverDissambiguation() { IDOrigin = entityID, DissambiguationCandiates = new List<DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate>() };
                            foreach (string candidateID in pDiscoverResult.discoveredEntitiesProbability[entityID].Keys)
                            {
                                discoverDissambiguation.DissambiguationCandiates.Add(new DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate() { IDCandidate = candidateID, Score = pDiscoverResult.discoveredEntitiesProbability[entityID][candidateID] });
                            }
                            discoverItemBD.DissambiguationProblems.Add(discoverDissambiguation);
                        }
                    }
                    //Reporte de descubrimiento
                    discoverItemBD.DiscoverReport = "Time processed (seconds): " + pDiscoverResult.secondsProcessed + "\n";
                    if (pDiscoverResult.discoveredEntitiesWithSubject != null && pDiscoverResult.discoveredEntitiesWithSubject.Count > 0)
                    {
                        discoverItemBD.DiscoverReport += "Entities discover with the same uri: " + pDiscoverResult.discoveredEntitiesWithSubject.Count + "\n";
                        foreach (string uri in pDiscoverResult.discoveredEntitiesWithSubject)
                        {
                            discoverItemBD.DiscoverReport += "\t" + uri + "\n";
                        }
                    }
                    if (pDiscoverResult.discoveredEntitiesWithId != null && pDiscoverResult.discoveredEntitiesWithId.Count > 0)
                    {
                        discoverItemBD.DiscoverReport += "Entities discover with some common identifier: " + pDiscoverResult.discoveredEntitiesWithId.Count + "\n";
                        foreach (string uri in pDiscoverResult.discoveredEntitiesWithId.Keys)
                        {
                            discoverItemBD.DiscoverReport += "\t" + uri + " --> " + pDiscoverResult.discoveredEntitiesWithId[uri] + "\n";
                        }
                    }
                    if (pDiscoverResult.discoveredEntitiesWithDataBase != null && pDiscoverResult.discoveredEntitiesWithDataBase.Count > 0)
                    {
                        discoverItemBD.DiscoverReport += "Entities discover with reconciliation config: " + pDiscoverResult.discoveredEntitiesWithDataBase.Count + "\n";
                        foreach (string uri in pDiscoverResult.discoveredEntitiesWithDataBase.Keys)
                        {
                            discoverItemBD.DiscoverReport += "\t" + uri + " --> " + pDiscoverResult.discoveredEntitiesWithDataBase[uri] + "\n";
                        }
                    }
                    if (pDiscoverResult.orcidIntegration != null && pDiscoverResult.orcidIntegration.Count > 0)
                    {
                        discoverItemBD.DiscoverReport += "Entities with identifiers obtained with ORCID integration: " + pDiscoverResult.orcidIntegration.Count + "\n";
                        foreach (string uri in pDiscoverResult.orcidIntegration.Keys)
                        {
                            foreach (string property in pDiscoverResult.orcidIntegration[uri].Keys)
                            {
                                discoverItemBD.DiscoverReport += "\t" + uri + " - " + property + " --> " + pDiscoverResult.orcidIntegration[uri][property] + "\n";
                            }
                        }
                    }
                    pDiscoverItemBDService.ModifyDiscoverItem(discoverItemBD);
                }
            }
        }

        #region Configuraciones
        /// <summary>
        /// Cargamos las configuraciones de sincronización
        /// </summary>
        /// <returns></returns>
        private static List<Disambiguation> LoadDisambiguationConfigs()
        {
            return JsonConvert.DeserializeObject<List<Disambiguation>>(File.ReadAllText("Config/reconciliationConfig.json"));
        }
        #endregion

        #region Interacción con RohGraph

        /// <summary>
        /// Obtiene las entidades con sus rdf:type (con la inferencia)
        /// </summary>
        /// <param name="pRohInferenceGraph">Grafo en local (con inferencia)</param>
        /// <param name="pIncludeBlankNodes">indica si hay que incluir los blanknodes</param>
        /// <returns>Diccionario con las entidades y su rdf:types</returns>
        private static Dictionary<string, HashSet<string>> LoadEntitiesWithRdfTypes(RohGraph pRohInferenceGraph, bool pIncludeBlankNodes)
        {
            //Obtenemos las diferentes entidades que se van a cargar junto con su rdftypr (con herencia)
            Dictionary<string, HashSet<string>> entitiesRdfTypes = new Dictionary<string, HashSet<string>>();
            string query = @"select ?s ?rdftype where{?s a ?rdftype. FILTER(!isBlank(?rdftype))}";
            if (!pIncludeBlankNodes)
            {
                query = @"select ?s ?rdftype where{?s a ?rdftype. FILTER(!isBlank(?rdftype)) FILTER(!isBlank(?s))}";
            }
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pRohInferenceGraph.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                string s = sparqlResult["s"].ToString();
                string rdfType = sparqlResult["rdftype"].ToString();
                if (!entitiesRdfTypes.ContainsKey(s))
                {
                    entitiesRdfTypes.Add(s, new HashSet<string>());
                }
                entitiesRdfTypes[s].Add(rdfType);
            }
            return entitiesRdfTypes;
        }

        /// <summary>
        /// Obtiene las entidades con su rdf:type
        /// </summary>
        /// <param name="pRohGraph">Grafo en local</param>
        /// <param name="pIncludeBlankNodes">indica si hay que incluir los blanknodes</param>
        /// <returns>Diccionario con las entidades y su rdf:type</returns>
        private static Dictionary<string, string> LoadEntitiesWithRdfType(RohGraph pRohGraph, bool pIncludeBlankNodes)
        {
            //Obtenemos las diferentes entidades que se van a cargar junto con su rdftypr (sin herencia)
            Dictionary<string, string> entitiesRdfType = new Dictionary<string, string>();
            string query = @"select ?s ?rdftype where {?s a ?rdftype. FILTER(!isBlank(?rdftype))}";
            if (!pIncludeBlankNodes)
            {
                query = @"select ?s ?rdftype where{?s a ?rdftype. FILTER(!isBlank(?rdftype)) FILTER(!isBlank(?s))}";
            }
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pRohGraph.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                string s = sparqlResult["s"].ToString();
                string rdfType = sparqlResult["rdftype"].ToString();
                if (!entitiesRdfType.ContainsKey(s))
                {
                    entitiesRdfType.Add(s, rdfType);
                }
            }
            return entitiesRdfType;
        }

        /// <summary>
        /// Obtiene los datos de desambiguación del RDF
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Listado con los sujetos y sus rdf:type (con inferencia)</param>
        /// <param name="pRohGraph">Grafo en local</param>
        /// <returns>Entidades del RDF con sus datos de desambiguación</returns>
        private static Dictionary<string, List<DisambiguationData>> GetDisambiguationDataRdf(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pRohGraph)
        {
            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf = new Dictionary<string, List<DisambiguationData>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                foreach (string rdfType in pEntitiesRdfTypes[entityID])
                {
                    //Obtenemos las configuraciones aplicables
                    List<Disambiguation> disambiguations = mDisambiguationConfigs.Where(x => x.rdfType == rdfType).ToList();
                    if (disambiguations.Count > 0)
                    {
                        foreach (Disambiguation disambiguation in disambiguations)
                        {
                            DisambiguationData disambiguationData = GetEntityDataForReconciliation(entityID, disambiguation, pRohGraph);

                            if (!disambiguationDataRdf.ContainsKey(entityID))
                            {
                                disambiguationDataRdf.Add(entityID, new List<DisambiguationData>());
                            }
                            if (disambiguationData != null)
                            {
                                disambiguationDataRdf[entityID].Add(disambiguationData);
                            }
                        }
                    }
                }
            }
            //Obtenemos los roh:identifier de todas las entidades           
            Dictionary<string, KeyValuePair<string, string>> disambiguationIdentifiersRdf = new Dictionary<string, KeyValuePair<string, string>>();
            Dictionary<string, Dictionary<string, HashSet<string>>> identifiersData = GetPropertiesValues(pEntitiesRdfTypes.Keys.ToList(), new List<string> { mPropertyRohIdentifier, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type" }, false, pRohGraph);
            foreach (string entityId in identifiersData.Keys)
            {
                if (identifiersData[entityId].ContainsKey(mPropertyRohIdentifier) && identifiersData[entityId].ContainsKey("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"))
                {
                    if (!disambiguationDataRdf.ContainsKey(entityId))
                    {
                        disambiguationDataRdf.Add(entityId, new List<DisambiguationData>());
                    }
                    DisambiguationData data_roh_identifier = new DisambiguationData();
                    data_roh_identifier.identifiers = new Dictionary<string, HashSet<string>>();
                    data_roh_identifier.identifiers.Add(mPropertyRohIdentifier, identifiersData[entityId][mPropertyRohIdentifier]);
                    disambiguationDataRdf[entityId].Add(data_roh_identifier);
                }
            }
            return disambiguationDataRdf;
        }

        /// <summary>
        /// Obtiene datos de una entidad cargada en un grafo en local para la reconciliación
        /// </summary>
        /// <param name="pSubject">Sujeto</param>
        /// <param name="pDisambiguation">Objeto de desambiguación</param>
        /// <param name="pGrafo">Grafo en local</param>
        /// <returns>Datos para la desambiguación</returns>
        private static DisambiguationData GetEntityDataForReconciliation(string pSubject, Disambiguation pDisambiguation, RohGraph pGrafo)
        {
            Dictionary<string, DisambiguationData> data = GetEntityDataForReconciliation(new List<string> { pSubject }, pDisambiguation, pGrafo);
            if (data != null && data.ContainsKey(pSubject))
            {
                return data[pSubject];
            }
            return null;
        }

        /// <summary>
        /// Obtiene datos de varias entidades cargadas en un grafo en local para la reconciliación
        /// </summary>
        /// <param name="pSubject">Sujeto</param>
        /// <param name="pDisambiguation">Objeto de desambiguación</param>
        /// <param name="pGrafo">Grafo en local</param>
        /// <returns>Datos para la desambiguación</returns>
        private static Dictionary<string, DisambiguationData> GetEntityDataForReconciliation(List<string> pSubjects, Disambiguation pDisambiguation, RohGraph pGrafo)
        {
            Dictionary<string, DisambiguationData> response = new Dictionary<string, DisambiguationData>();
            //Propiedades directas
            List<string> properties = pDisambiguation.properties.Where(x => !x.inverse).Select(x => x.property).ToList();
            properties.AddRange(pDisambiguation.identifiers);
            Dictionary<string, Dictionary<string, HashSet<string>>> propertiesData = GetPropertiesValues(pSubjects, properties, false, pGrafo);

            //Propiedades inversas
            List<string> inverseProperties = pDisambiguation.properties.Where(x => x.inverse).Select(x => x.property).ToList();
            Dictionary<string, Dictionary<string, HashSet<string>>> inversePropertiesData = GetPropertiesValues(pSubjects, inverseProperties, true, pGrafo);

            //Identificadores
            foreach (string propertyIdentifier in pDisambiguation.identifiers)
            {
                foreach (string subject in pSubjects)
                {
                    if (propertiesData.ContainsKey(subject))
                    {
                        if (!response.ContainsKey(subject))
                        {
                            DisambiguationData disambiguationData = new DisambiguationData();
                            disambiguationData.disambiguation = pDisambiguation;
                            response.Add(subject, disambiguationData);
                        }
                        if (propertiesData[subject].ContainsKey(propertyIdentifier))
                        {
                            response[subject].identifiers.Add(propertyIdentifier, propertiesData[subject][propertyIdentifier]);
                        }
                    }
                }
            }
            //Propiedades
            foreach (Disambiguation.Property disambiguationProperty in pDisambiguation.properties)
            {
                if (disambiguationProperty.inverse)
                {
                    foreach (string subject in pSubjects)
                    {
                        if (inversePropertiesData.ContainsKey(subject))
                        {
                            if (!response.ContainsKey(subject))
                            {
                                DisambiguationData disambiguationData = new DisambiguationData();
                                disambiguationData.disambiguation = pDisambiguation;
                                response.Add(subject, disambiguationData);
                            }
                            if (inversePropertiesData[subject].ContainsKey(disambiguationProperty.property))
                            {
                                HashSet<string> values = inversePropertiesData[subject][disambiguationProperty.property];
                                DisambiguationData.DataProperty dataProperty = new DisambiguationData.DataProperty(disambiguationProperty, values);
                                response[subject].properties.Add(dataProperty);
                            }
                        }
                    }
                }
                else
                {
                    foreach (string subject in pSubjects)
                    {
                        if (propertiesData.ContainsKey(subject))
                        {
                            if (!response.ContainsKey(subject))
                            {
                                DisambiguationData disambiguationData = new DisambiguationData();
                                disambiguationData.disambiguation = pDisambiguation;
                                response.Add(subject, disambiguationData);
                            }
                            if (propertiesData[subject].ContainsKey(disambiguationProperty.property))
                            {
                                HashSet<string> values = propertiesData[subject][disambiguationProperty.property];
                                DisambiguationData.DataProperty dataProperty = new DisambiguationData.DataProperty(disambiguationProperty, values);
                                response[subject].properties.Add(dataProperty);
                            }
                        }
                    }
                }
            }
            //Si no tiene alguna propiedad obligatoria lo elminamos
            List<string> propsObligatorias = pDisambiguation.properties.Where(x => x.mandatory).ToList().Select(x => x.property).ToList();
            if (propsObligatorias.Count > 0)
            {
                foreach (string key in response.Keys.ToList())
                {
                    List<string> propsEntidad = response[key].properties.Select(x => x.property).Select(x => x.property).ToList();
                    if (propsObligatorias.Intersect(propsEntidad).Count() < propsObligatorias.Count)
                    {
                        response.Remove(key);
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Obtiene N propiedades de N entidades de un grafo en local
        /// </summary>
        /// <param name="pSubjects">Sujetos</param>
        /// <param name="pProperties">Propiedades</param>
        /// <param name="pInverse">Implica si son inversas</param>
        /// <param name="pGrafo">Grafo en local</param>
        /// <returns>Valor de las propiedades para los sujetos introducidos</returns>
        private static Dictionary<string, Dictionary<string, HashSet<string>>> GetPropertiesValues(List<string> pSubjects, List<string> pProperties, bool pInverse, RohGraph pGrafo)
        {
            Dictionary<string, Dictionary<string, HashSet<string>>> propertyValues = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            List<Triple> triples = pGrafo.Triples.ToList();

            Dictionary<string, Dictionary<string, HashSet<string>>> rels = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            foreach (Triple triple in triples)
            {
                string s = triple.Subject.ToString();
                string p = triple.Predicate.ToString();
                if (pInverse)
                {
                    if (!(triple.Object is LiteralNode))
                    {
                        string o = triple.Object.ToString();
                        if (!rels.ContainsKey(o))
                        {
                            rels.Add(o, new Dictionary<string, HashSet<string>>());
                        }
                        if (!rels[o].ContainsKey(p))
                        {
                            rels[o].Add(p, new HashSet<string>());
                        }
                        rels[o][p].Add(triple.Subject.ToString());
                    }
                }
                else
                {
                    if (!rels.ContainsKey(s))
                    {
                        rels.Add(s, new Dictionary<string, HashSet<string>>());
                    }
                    if (!rels[s].ContainsKey(p))
                    {
                        rels[s].Add(p, new HashSet<string>());
                    }
                    if (triple.Object is LiteralNode)
                    {
                        LiteralNode node = (LiteralNode)triple.Object;
                        rels[s][p].Add("\"" + node.Value.Replace("\"", "\\\"") + "\"^^<" + node.DataType + ">");
                    }
                    else
                    {
                        rels[s][p].Add(triple.Object.ToString());
                    }
                }
            }

            foreach (string property in pProperties)
            {
                string[] propertySplit = property.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                if (pInverse)
                {
                    propertySplit = propertySplit.Reverse().ToArray();
                }


                foreach (string subject in pSubjects)
                {
                    HashSet<string> inAux = new HashSet<string>() { subject };
                    HashSet<string> outAux = new HashSet<string>();
                    foreach (string propertyIn in propertySplit)
                    {
                        outAux = new HashSet<string>();
                        foreach (string subjectIn in rels.Keys)
                        {
                            if (rels[subjectIn].ContainsKey(propertyIn) || propertyIn == "?")
                            {
                                if (inAux.Contains(subjectIn))
                                {
                                    if (propertyIn != "?")
                                    {
                                        outAux.UnionWith(rels[subjectIn][propertyIn]);
                                    }
                                    else
                                    {
                                        outAux.UnionWith(rels[subjectIn].Where(x => x.Key != "http://www.w3.org/1999/02/22-rdf-syntax-ns#type").SelectMany(x => x.Value).ToList());
                                    }
                                }
                            }
                        }
                        inAux = outAux;
                    }
                    if (outAux.Count > 0)
                    {
                        if (!propertyValues.ContainsKey(subject))
                        {
                            propertyValues.Add(subject, new Dictionary<string, HashSet<string>>());
                        }
                        if (!propertyValues[subject].ContainsKey(property))
                        {
                            propertyValues[subject].Add(property, new HashSet<string>());
                        }
                        propertyValues[subject][property].UnionWith(outAux);
                    }
                }
            }
            return propertyValues;
        }


        /// <summary>
        /// Carga los pesos de los nombres del RDF con sus equivalentes en la BBDD
        /// </summary>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <param name="pPersonsWithName">Nombres de todas las personas que hay cargadas en la BBDD, Clave ID, Valor nombre</param>
        /// <param name="pDataGraph">Grafo en local</param>
        private static void LoadNamesScore(ref Dictionary<string, Dictionary<string, float>> pNamesScore, Dictionary<string, string> pPersonsWithName, RohGraph pDataGraph)
        {
            HashSet<string> listaNombres = new HashSet<string>();
            string query = @"select distinct ?name where{?s a <http://purl.org/roh/mirror/foaf#Person>. ?s <http://purl.org/roh/mirror/foaf#name>  ?name.}";
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                if (sparqlResult["name"] is LiteralNode)
                {
                    string name = ((LiteralNode)sparqlResult["name"]).Value;
                    if (!pNamesScore.ContainsKey(name))
                    {
                        listaNombres.Add(name);
                    }
                }
            }
            foreach (string nombre in listaNombres)
            {
                pNamesScore.Add(nombre, new Dictionary<string, float>());
                foreach (string personBBDD in pPersonsWithName.Keys)
                {
                    float similarity = GetNameSimilarity(nombre, pPersonsWithName[personBBDD]);
                    //Mapear la similitud de 0.5--1 hacia mMinScore -- 1;
                    if (similarity >= 0.5f)
                    {
                        similarity = mMinScore + ((1 - mMinScore) / (0.5f / (similarity - 0.5f)));
                        pNamesScore[nombre].Add(personBBDD, GetNameSimilarity(nombre, pPersonsWithName[personBBDD]));
                    }
                }
                if (pNamesScore[nombre].Count == 0)
                {
                    pNamesScore.Remove(nombre);
                }
                else
                {
                    pNamesScore[nombre] = pNamesScore[nombre].OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    if (pNamesScore[nombre].Count > 10)
                    {
                        pNamesScore[nombre].ToList().GetRange(0, 10).ToDictionary(x => x.Key, x => x.Value);
                    }
                }
            }
        }

        #endregion

        #region Interacción con la BBDD SPARQL

        /// <summary>
        /// Obtiente los sujetos que están cargados en la BBDD SPARQL junto con su rdf:type
        /// </summary>
        /// <param name="pSubjects">Lista de identificadores de las entidades a buscar</param>
        /// <returns></returns>
        private static Dictionary<string, string> LoadEntitiesDB(IEnumerable<string> pSubjects)
        {
            Dictionary<string, string> entitiesDB = new Dictionary<string, string>();
            List<List<string>> listaListas = SplitList(pSubjects.ToList(), 500).ToList();
            foreach (List<string> listaIn in listaListas)
            {
                string consulta = "select distinct ?s ?rdftype where{?s a ?rdftype. Filter(?s in (<" + string.Join(">,<", listaIn) + ">))}";
                SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    entitiesDB.Add(row["s"].value, row["rdftype"].value);
                }
            }
            return entitiesDB;
        }

        /// <summary>
        /// Obtiene de la BBDD los candidatos para la desambiguación junto con sus datos para unoa serie de entidades junto con sus datos de desambiguación
        /// </summary>
        /// <param name="pEntitiesRdfTypeBBDD">rdf:type de las entidades obtenidas de la BBDD</param>
        /// <param name="pEntitiesRdfType">rdf:type de las entidades a las que se les va a buscar candidatos</param>
        /// <param name="pListaEntidadesOmitir">lista de entidades que hay que omitir al buscar candidatos</param>
        /// <param name="pDisambiguationDataRdf">datos de desambiguación del RDF</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <returns>Diccionario con los candidatos encontrados y sus datos para apoyar la desambiguación</returns>
        private static Dictionary<string, List<DisambiguationData>> GetDisambiguationDataBBDD(out Dictionary<string, string> pEntitiesRdfTypeBBDD, Dictionary<string, string> pEntitiesRdfType, HashSet<string> pListaEntidadesOmitir, Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, Dictionary<string, Dictionary<string, float>> pNamesScore)
        {
            Dictionary<string, List<DisambiguationData>> disambiguationDataBBDD = new Dictionary<string, List<DisambiguationData>>();
            pEntitiesRdfTypeBBDD = new Dictionary<string, string>();
            foreach (string entityID in pDisambiguationDataRdf.Keys)
            {
                if (!pListaEntidadesOmitir.Contains(entityID))
                {
                    foreach (DisambiguationData disambiguationData in pDisambiguationDataRdf[entityID])
                    {
                        Dictionary<string, DisambiguationData> datosCandidatos = GetEntityDataForReconciliationBBDD(pEntitiesRdfType[entityID], disambiguationData, pNamesScore);
                        foreach (string key in datosCandidatos.Keys)
                        {
                            if (!disambiguationDataBBDD.ContainsKey(key))
                            {
                                disambiguationDataBBDD.Add(key, new List<DisambiguationData>());
                            }
                            disambiguationDataBBDD[key].Add(datosCandidatos[key]);
                            pEntitiesRdfTypeBBDD[key] = pEntitiesRdfType[entityID];
                        }
                    }
                }
            }
            return disambiguationDataBBDD;
        }

        /// <summary>
        /// Obtiene de la BBDD los candidatos para la desambiguación junto con sus datos para unos datos de desambiguación concretos
        /// </summary>
        /// <param name="pRdfType">rdf:type de los candidatos</param>
        /// <param name="pDisambiguationData">Datos de desambiguación</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <returns></returns>
        private static Dictionary<string, DisambiguationData> GetEntityDataForReconciliationBBDD(string pRdfType, DisambiguationData pDisambiguationData, Dictionary<string, Dictionary<string, float>> pNamesScore)
        {
            Dictionary<string, DisambiguationData> returnDisambiguationData = new Dictionary<string, DisambiguationData>();
            if (pDisambiguationData != null && pDisambiguationData.properties != null && pDisambiguationData.properties.Count > 0)
            {
                string selectGlobal = "select distinct ?s ?prop ?value where\n{";

                #region ObtenerProps
                string whereProps = "";
                List<string> wherePropsList = new List<string>();
                foreach (Disambiguation.Property property in pDisambiguationData.disambiguation.properties)
                {
                    string propertyString = property.property;
                    bool inverse = property.inverse;
                    string[] propertyStringSplit = propertyString.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    string whereAux = "\n\t{";
                    if (!inverse)
                    {
                        for (int i = 0; i < propertyStringSplit.Length; i++)
                        {
                            if (i == 0)
                            {
                                whereAux += $"\n\t\t?s";
                            }
                            else
                            {
                                whereAux += $"\n\t\t?s_{i - 1}";
                            }
                            if (propertyStringSplit[i] == "?")
                            {
                                whereAux += $" ?aux{i} ";
                            }
                            else
                            {
                                whereAux += $" <{propertyStringSplit[i]}> ";
                            }
                            if (i + 1 == propertyStringSplit.Length)
                            {
                                whereAux += $" ?value.";
                            }
                            else
                            {
                                whereAux += $" ?s_{i}.";
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < propertyStringSplit.Length; i++)
                        {
                            if (i == 0)
                            {
                                whereAux += $"\n\t\t?value";
                            }
                            else
                            {
                                whereAux += $"\n\t\t?s_{i - 1}";
                            }
                            if (propertyStringSplit[i] == "?")
                            {
                                whereAux += $" ?aux{i} ";
                            }
                            else
                            {
                                whereAux += $" <{propertyStringSplit[i]}> ";
                            }
                            if (i + 1 == propertyStringSplit.Length)
                            {
                                whereAux += $" ?s.";
                            }
                            else
                            {
                                whereAux += $" ?s_{i}.";
                            }
                        }
                    }
                    whereAux += $"\n\t\tBIND('{propertyString}' as ?prop)";
                    whereAux += "\n\t}";
                    wherePropsList.Add(whereAux);
                }
                whereProps = string.Join("\n\tUNION", wherePropsList);
                #endregion

                #region Obtener sujetos    
                string whereSujetos = $"\n\t\t\t?s a <{pRdfType}>";
                HashSet<string> whereSujetosMandatory = new HashSet<string>();
                HashSet<string> whereSujetosNoMandatory = new HashSet<string>();
                int numNoMandatory = 0;
                HashSet<string> varsNoMandatory = new HashSet<string>();
                foreach (DisambiguationData.DataProperty dataProperty in pDisambiguationData.properties.OrderByDescending(x => x.property.mandatory))
                {
                    if (dataProperty.property.scorePositive.HasValue)
                    {
                        string selectProperty = "";
                        string whereProperty = "";
                        string orderProperty = "";
                        string varScore = "?scoreMandatory";
                        if (!dataProperty.property.mandatory)
                        {
                            varScore = "?scoreNoMandatoryAux" + numNoMandatory;
                            varsNoMandatory.Add(varScore);
                            numNoMandatory++;
                        }
                        if (dataProperty.property.type == Disambiguation.Property.Type.name)
                        {
                            //selectProperty = $"\n\t\t\t\tselect ?s count(?p)*{dataProperty.property.scorePositive.Value.ToString().Replace(",", ".")} as {varScore} where\n\t\t\t\t{{";
                            //orderProperty = $"\n\t\t\t\t}} order by desc (count(?p)*{dataProperty.property.scorePositive.Value.ToString().Replace(",", ".")})   limit 10";
                        }
                        else if (dataProperty.property.type == Disambiguation.Property.Type.title)
                        {

                            selectProperty = $"\n\t\t\t\tselect ?s count(?p)*{dataProperty.property.scorePositive.Value.ToString().Replace(",", ".")} as {varScore} where\n\t\t\t\t{{";
                            orderProperty = $"\n\t\t\t\t}} order by desc (count(?p)*{dataProperty.property.scorePositive.Value.ToString().Replace(",", ".")})   limit 10";
                        }
                        else
                        {
                            selectProperty = $"\n\t\t\t\tselect ?s count(distinct ?p)*{dataProperty.property.scorePositive.Value.ToString().Replace(",", ".")} as {varScore} where\n\t\t\t\t{{";
                            orderProperty = $"\n\t\t\t\t}}";
                        }



                        string propertyString = dataProperty.property.property;
                        bool inverse = dataProperty.property.inverse;
                        string[] propertyStringSplit = propertyString.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!inverse)
                        {
                            for (int i = 0; i < propertyStringSplit.Length; i++)
                            {
                                if (i == 0)
                                {
                                    whereProperty += $"\n\t\t\t\t\t\t?s";
                                }
                                else
                                {
                                    whereProperty += $"\n\t\t\t\t\t\t?s_{i - 1}";
                                }
                                if (propertyStringSplit[i] == "?")
                                {
                                    whereProperty += $" ?aux{i} ";
                                }
                                else
                                {
                                    whereProperty += $" <{propertyStringSplit[i]}> ";
                                }
                                if (i + 1 == propertyStringSplit.Length)
                                {
                                    whereProperty += $" ?p.";
                                }
                                else
                                {
                                    whereProperty += $" ?s_{i}.";
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < propertyStringSplit.Length; i++)
                            {
                                if (i == 0)
                                {
                                    whereProperty += $"\n\t\t\t\t\t\t?p";
                                }
                                else
                                {
                                    whereProperty += $"\n\t\t\t\t\t\t?s_{i - 1}";
                                }
                                if (propertyStringSplit[i] == "?")
                                {
                                    whereProperty += $" ?aux{i} ";
                                }
                                else
                                {
                                    whereProperty += $" <{propertyStringSplit[i]}> ";
                                }
                                if (i + 1 == propertyStringSplit.Length)
                                {
                                    whereProperty += $" ?s.";
                                }
                                else
                                {
                                    whereProperty += $" ?s_{i}.";
                                }
                            }
                        }
                        if (dataProperty.property.type == Disambiguation.Property.Type.title)
                        {
                            string text = dataProperty.values.First();
                            if (text.StartsWith("\""))
                            {
                                text = text.Substring(1, text.LastIndexOf("\"") - 1);
                            }
                            text = text.ToLower();
                            string filter = "lcase(str(?p))";

                            Dictionary<char, List<char>> charsSubstitute = new Dictionary<char, List<char>>();
                            List<char> charsRemove = new List<char>();
                            foreach (char value in charRemovePropertyTitle.Keys)
                            {
                                if (charRemovePropertyTitle[value].HasValue)
                                {
                                    text = text.Replace(value, charRemovePropertyTitle[value].Value);
                                    if (!charsSubstitute.ContainsKey(charRemovePropertyTitle[value].Value))
                                    {
                                        charsSubstitute.Add(charRemovePropertyTitle[value].Value, new List<char>());
                                    }
                                    charsSubstitute[charRemovePropertyTitle[value].Value].Add(value);
                                }
                                else
                                {
                                    text = text.Replace(value.ToString(), "");
                                    charsRemove.Add(value);
                                }

                            }

                            foreach (char value in charsSubstitute.Keys)
                            {
                                if (charsSubstitute[value].Count > 1)
                                {
                                    filter = $"REPLACE({filter},'[{string.Join("", charsSubstitute[value]).Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\\\[").Replace("]", "\\\\]").Replace("-", "\\\\-")}]','{value}')";
                                }
                                else
                                {
                                    filter = $"REPLACE({filter},'{charsSubstitute[value].First().ToString().Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\\\[").Replace("]", "\\\\]").Replace("-", "\\\\-")}','{value}')";
                                }
                            }
                            if (charsRemove.Count > 0)
                            {
                                filter = $"REPLACE({filter},'[{string.Join("", charsRemove).Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\\\[").Replace("]", "\\\\]").Replace("-", "\\\\-")}]','')";
                            }

                            whereProperty += $"\n\t\t\t\t\tFILTER({filter} ='{text}')";
                        }
                        else if (dataProperty.property.type == Disambiguation.Property.Type.name)
                        {
                            string text = dataProperty.values.First();
                            if (text.StartsWith("\""))
                            {
                                text = text.Substring(1, text.LastIndexOf("\"") - 1);
                            }
                            whereProperty = "";
                            if (pNamesScore.ContainsKey(text))
                            {
                                Dictionary<string, float> namesWithScore = pNamesScore[text];
                                List<string> filtersName = new List<string>();
                                foreach (string name in namesWithScore.Keys)
                                {
                                    filtersName.Add(@$"  {{
					                                BIND(<{name}> as ?s)
					                                BIND({namesWithScore[name].ToString().Replace(",", ".")}*{dataProperty.property.scorePositive.Value.ToString().Replace(",", ".")} as ?scoreMandatory)
				                                }}");
                                }
                                whereProperty = string.Join("UNION", filtersName);
                            }

                        }
                        else if (dataProperty.property.type == Disambiguation.Property.Type.ignoreCaseSensitive)
                        {
                            HashSet<string> filters = new HashSet<string>();
                            foreach (string value in dataProperty.values)
                            {
                                if (value.StartsWith("\""))
                                {
                                    filters.Add($" lcase(str(?p)) =lcase(str({value}))");
                                }
                                else
                                {
                                    filters.Add($" lcase(str(?p)) =lcase(str(<{value}>))");
                                }
                            }
                            whereProperty += $"\n\t\t\t\t\tFILTER({ string.Join(" OR ", filters)})";
                        }
                        else
                        {
                            HashSet<string> filters = new HashSet<string>();
                            foreach (string value in dataProperty.values)
                            {
                                if (value.StartsWith("\""))
                                {
                                    filters.Add($" ?p ={value}");
                                }
                                else
                                {
                                    filters.Add($" ?p =<{value}>");
                                }
                            }
                            whereProperty += $"\n\t\t\t\t\tFILTER({ string.Join(" OR ", filters)})";
                        }


                        string filterProperty = selectProperty + whereProperty + orderProperty;
                        if (dataProperty.property.mandatory)
                        {
                            whereSujetosMandatory.Add($"\n\t\t\t{{{ filterProperty}\n\t\t\t}}");
                        }
                        else
                        {
                            whereSujetosNoMandatory.Add($"\n\t\t\tOPTIONAL{{{ filterProperty}\n\t\t\t}}");
                        }
                    }
                }
                whereSujetos += string.Join("", whereSujetosMandatory) + string.Join("", whereSujetosNoMandatory);
                #endregion

                string selectSujetos = $"\n\t\tselect ?s ?scoreMandatory where\n\t\t{{";
                string orderSujetos = $"\n\t\t}}order by desc(?scoreMandatory) limit 10";
                if (varsNoMandatory.Count > 0)
                {
                    string scoresNoMandatory = "(sum(" + string.Join(")+sum(", varsNoMandatory) + "))";
                    selectSujetos = $"\n\t\tselect ?s ?scoreMandatory {scoresNoMandatory} as ?scoreNoMandatory where\n\t\t{{";
                    orderSujetos = $"\n\t\t}}order by desc(?scoreMandatory) desc {scoresNoMandatory} limit 10";
                }

                string consulta = selectSujetos + whereSujetos + orderSujetos;
                HashSet<string> listaItems = new HashSet<string>();
                if (whereSujetosMandatory.Count > 0 || whereSujetosNoMandatory.Count > 0)
                {
                    SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        string s = row["s"].value;
                        listaItems.Add(s);
                    }
                }
                if (listaItems.Count > 0)
                {
                    consulta = selectGlobal + whereProps + "FILTER(?s in (<" + string.Join(">,<", listaItems) + ">))" + "\n\t}";
                    SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        string s = row["s"].value;
                        string prop = row["prop"].value;
                        string value = row["value"].value;
                        if (!returnDisambiguationData.ContainsKey(s))
                        {
                            returnDisambiguationData.Add(s, new DisambiguationData());
                            returnDisambiguationData[s].disambiguation = pDisambiguationData.disambiguation;
                            returnDisambiguationData[s].properties = new List<DisambiguationData.DataProperty>();
                        }
                        if (!returnDisambiguationData[s].properties.Exists(x => x.property.property == prop))
                        {
                            returnDisambiguationData[s].properties.Add(new DisambiguationData.DataProperty(pDisambiguationData.disambiguation.properties.First(x => x.property == prop), new HashSet<string>()));
                        }
                        if (row["value"].type == "typed-literal")
                        {
                            value = "\"" + row["value"].value.Replace("\"", "\\\"") + "\"^^<" + row["value"].datatype + ">";
                        }
                        returnDisambiguationData[s].properties.First(x => x.property.property == prop).values.Add(value);
                    }
                }
            }
            return returnDisambiguationData;
        }

        /// <summary>
        /// Obtiene los nombres de todas las personas
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> LoadPersonWithName()
        {
            Dictionary<string, string> personsWithName = new Dictionary<string, string>();
            int numPagination = 10000;
            int offset = 0;
            int numResulados = numPagination;
            while (numResulados == numPagination)
            {
                string consulta = $"select * where{{select * where {{?s a <http://purl.org/roh/mirror/foaf#Person>. ?s <http://purl.org/roh/mirror/foaf#name> ?name}}order by ?s }}offset {offset} limit {numPagination}";
                SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
                numResulados = sparqlObject.results.bindings.Count;
                if (sparqlObject.results.bindings.Count > 0)
                {
                    offset += numPagination;
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        if (!personsWithName.ContainsKey(row["s"].value))
                        {
                            personsWithName.Add(row["s"].value, row["name"].value);
                        }
                    }
                }
            }
            return personsWithName;
        }


        /// <summary>
        /// Elimina una entidad de la BBDD (y sus blank nodes de forma recursiva)
        /// </summary>
        /// <param name="pEntity">Entida</param>
        private static void DeleteEntity(string pEntity)
        {
            //Obtenemos todos los blanknodes a los que apunta la entidad para luego borrarlos
            HashSet<string> bnodeChildrens = new HashSet<string>();
            SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, $"select distinct ?bnode where{{<{pEntity}> ?p ?bnode. FILTER(isblank(?bnode))}}", mQueryParam);
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {
                bnodeChildrens.Add(row["bnode"].value);
            }

            string queryDeleteS = $@"DELETE {{ <{pEntity}> ?p ?o. }}
                                    WHERE 
                                    {{
                                        <{pEntity}> ?p ?o. 
                                    }}";
            SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, queryDeleteS, mQueryParam);
            string queryDeleteO = $@"DELETE {{ ?s ?p <{pEntity}>. }}
                                    WHERE 
                                    {{
                                        ?s ?p <{pEntity}>. 
                                    }}";
            SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, queryDeleteO, mQueryParam);

            foreach (string bnode in bnodeChildrens)
            {
                DeleteEntity(bnode);
            }
        }

        /// <summary>
        /// Limpiamos los blanknodes huerfanos, o que no tengan triples (sólo rdftype)
        /// </summary>
        private static void DeleteOrphanNodes()
        {
            bool existeNodosHuerfanos = true;
            bool existeNodosSinDatos = true;
            while (existeNodosHuerfanos || existeNodosSinDatos)
            {
                existeNodosHuerfanos = false;
                existeNodosSinDatos = false;

                //Nodos huerfanos
                string queryASKOrphan = $@"ASK
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            FILTER(isblank(?s))
                                            MINUS{{?x ?y ?s}}
                                        }}";
                if (SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, queryASKOrphan, mQueryParam).boolean)
                {
                    existeNodosHuerfanos = true;
                    string deleteOrphanNodes = $@"DELETE {{ ?s ?p ?o. }}
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            FILTER(isblank(?s))
                                            MINUS{{?x ?y ?s}}
                                        }}";
                    SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, deleteOrphanNodes, mQueryParam);
                }

                //Nodos vacíos
                string queryASKEmpty = $@"ASK
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            FILTER(isblank(?s))
                                            MINUS{{
                                                ?s ?p  ?o.
                                                ?s ?p2 ?o2.
                                                FILTER(?p2 !=<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>)
                                            }}
                                        }}";
                if (SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, queryASKEmpty, mQueryParam).boolean)
                {
                    existeNodosSinDatos = true;
                    string deleteEmptyNodes = $@"DELETE {{ ?s ?p ?o. }}
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            FILTER(isblank(?s))
                                            MINUS{{
                                                ?s ?p  ?o.
                                                ?s ?p2 ?o2.
                                                FILTER(?p2 !=<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>)
                                            }}
                                        }}";
                    SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, deleteEmptyNodes, mQueryParam);
                }
            }

        }

        #endregion

        #region Métodos de descubrimiento


        /// <summary>
        /// Método que prepara los datos para efectuar la reconciliación
        /// </summary>
        /// <param name="pDataGraph">Grafo en local con los datos a procesar</param>
        /// <param name="pReasoner">Razonador para la inferencia de la ontología</param>
        /// <param name="pDataInferenceGraph">Grafo en local con los datos a procesar (con la inferencia de la ontología)</param>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades encontradas y sus rdf:type (con inferencia)</param>
        /// <param name="pEntitiesRdfType">Diccionario con las entidades encontradas y su rdf:type (sin inferencia)</param>
        /// <param name="pDisambiguationDataRdf">Datos extraidos del grafo para la reconciliación</param>
        /// <param name="pIncludeBlankNodes">indica si hay que incluir los blanknodes</param>
        private static void PrepareData(RohGraph pDataGraph, RohRdfsReasoner pReasoner, out RohGraph pDataInferenceGraph, out Dictionary<string, HashSet<string>> pEntitiesRdfTypes, out Dictionary<string, string> pEntitiesRdfType, out Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, bool pIncludeBlankNodes = false)
        {
            //Cargamos datos del RDF con inferencias
            pDataInferenceGraph = pDataGraph.Clone();
            pReasoner.Apply(pDataInferenceGraph);
            pEntitiesRdfTypes = LoadEntitiesWithRdfTypes(pDataInferenceGraph, pIncludeBlankNodes);
            pEntitiesRdfType = LoadEntitiesWithRdfType(pDataGraph, pIncludeBlankNodes);
            pDisambiguationDataRdf = GetDisambiguationDataRdf(pEntitiesRdfTypes, pDataGraph);
        }

        /// <summary>
        /// Efectua la reconciliación con los identificadores
        /// </summary>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades encontradas y sus rdf:type (con inferencia)</param>
        /// <param name="pEntitiesRdfType">Diccionario con las entidades encontradas y su rdf:type (sin inferencia)</param>
        /// <param name="pDisambiguationDataRdf">Datos extraidos del grafo para la reconciliación</param>
        /// <param name="pDataGraph">Grafo en local con los datos a procesar</param>
        /// <param name="pDataInferenceGraph">Grafo en local con los datos a procesar (con la inferencia de la ontología)</param>
        /// <param name="pReasoner">Razonador para la inferencia de la ontología</param>
        /// <returns>Lista con las entidades reconciliadas</returns>
        private static Dictionary<string, string> ReconciliateIDs(ref Dictionary<string, string> pListaEntidadesReconciliadas, ref Dictionary<string, HashSet<string>> pEntitiesRdfTypes, ref Dictionary<string, string> pEntitiesRdfType, ref Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, ref RohGraph pDataGraph, ref RohGraph pDataInferenceGraph, RohRdfsReasoner pReasoner)
        {
            Dictionary<string, string> entidaesReconciliadas = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> identificadoresRDFPorRdfType = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            foreach (string entityID in pDisambiguationDataRdf.Keys)
            {
                foreach (DisambiguationData disambiguationData in pDisambiguationDataRdf[entityID])
                {
                    if (disambiguationData.identifiers.Count > 0)
                    {
                        string rdfType = pEntitiesRdfType[entityID];
                        if (!identificadoresRDFPorRdfType.ContainsKey(rdfType))
                        {
                            identificadoresRDFPorRdfType.Add(rdfType, new Dictionary<string, Dictionary<string, List<string>>>());
                        }
                        if (!identificadoresRDFPorRdfType[rdfType].ContainsKey(entityID))
                        {
                            identificadoresRDFPorRdfType[rdfType].Add(entityID, new Dictionary<string, List<string>>());
                        }
                        foreach (string property in disambiguationData.identifiers.Keys)
                        {
                            if (!identificadoresRDFPorRdfType[rdfType][entityID].ContainsKey(property))
                            {
                                identificadoresRDFPorRdfType[rdfType][entityID].Add(property, new List<string>());
                            }
                            identificadoresRDFPorRdfType[rdfType][entityID][property].AddRange(disambiguationData.identifiers[property]);
                        }
                    }
                }
            }
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> identificadoresBBDDPorRdfType = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            foreach (string rdftype in identificadoresRDFPorRdfType.Keys)
            {
                Dictionary<string, List<string>> propVals = new Dictionary<string, List<string>>();
                foreach (string entityID in identificadoresRDFPorRdfType[rdftype].Keys)
                {
                    foreach (string prop in identificadoresRDFPorRdfType[rdftype][entityID].Keys)
                    {
                        if (!propVals.ContainsKey(prop))
                        {
                            propVals.Add(prop, new List<string>());
                        }
                        propVals[prop].AddRange(identificadoresRDFPorRdfType[rdftype][entityID][prop]);
                    }
                }

                foreach (string property in propVals.Keys)
                {
                    string consulta = $@"select distinct ?s ?identifier 
                                        where{{?s a <{rdftype}>. ?s  <{property}> ?identifier.  FILTER(?identifier in({string.Join(",", propVals[property])}) )}}";
                    SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        string s = row["s"].value;
                        if (!identificadoresBBDDPorRdfType.ContainsKey(rdftype))
                        {
                            identificadoresBBDDPorRdfType.Add(rdftype, new Dictionary<string, Dictionary<string, List<string>>>());
                        }
                        if (!identificadoresBBDDPorRdfType[rdftype].ContainsKey(s))
                        {
                            identificadoresBBDDPorRdfType[rdftype].Add(s, new Dictionary<string, List<string>>());
                        }
                        if (!identificadoresBBDDPorRdfType[rdftype][s].ContainsKey(property))
                        {
                            identificadoresBBDDPorRdfType[rdftype][s].Add(property, new List<string>());
                        }
                        if (row["identifier"].type == "typed-literal")
                        {
                            identificadoresBBDDPorRdfType[rdftype][s][property].Add("\"" + row["identifier"].value.Replace("\"", "\\\"") + "\"^^<" + row["identifier"].datatype + ">");
                        }
                        else
                        {
                            identificadoresBBDDPorRdfType[rdftype][s][property].Add(row["identifier"].value);
                        }
                    }
                }
            }


            foreach (string rdfType in identificadoresRDFPorRdfType.Keys)
            {
                if (identificadoresBBDDPorRdfType.ContainsKey(rdfType))
                {
                    foreach (string entityID in identificadoresRDFPorRdfType[rdfType].Keys)
                    {
                        if (!pListaEntidadesReconciliadas.ContainsKey(entityID))
                        {
                            foreach (string property in identificadoresRDFPorRdfType[rdfType][entityID].Keys)
                            {
                                var coincidenciaBBDD = identificadoresBBDDPorRdfType[rdfType].FirstOrDefault(x => x.Value.ContainsKey(property) && x.Value[property].Intersect(identificadoresRDFPorRdfType[rdfType][entityID][property]).Count() > 0);

                                if (coincidenciaBBDD.Key != null && coincidenciaBBDD.Value != null)
                                {
                                    TripleStore store = new TripleStore();
                                    store.Add(pDataGraph);

                                    //Cambiamos candidato.Key por entityID
                                    SparqlUpdateParser parser = new SparqlUpdateParser();

                                    //Actualizamos los sujetos
                                    SparqlUpdateCommandSet updateSubject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                                INSERT{<" + coincidenciaBBDD.Key + @"> ?p ?o.}
                                                                                WHERE 
                                                                                {
                                                                                    ?s ?p ?o.   FILTER(?s = <" + entityID + @">)
                                                                                }");
                                    //Actualizamos los objetos
                                    SparqlUpdateCommandSet updateObject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                                INSERT{?s ?p <" + coincidenciaBBDD.Key + @">.}
                                                                                WHERE 
                                                                                {
                                                                                    ?s ?p ?o.   FILTER(?o = <" + entityID + @">)
                                                                                }");
                                    LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                                    processor.ProcessCommandSet(updateSubject);
                                    processor.ProcessCommandSet(updateObject);
                                    pListaEntidadesReconciliadas.Add(entityID, coincidenciaBBDD.Key);
                                    entidaesReconciliadas.Add(entityID, coincidenciaBBDD.Key);
                                }
                            }
                        }
                    }
                }
            }
            PrepareData(pDataGraph, pReasoner, out pDataInferenceGraph, out pEntitiesRdfTypes, out pEntitiesRdfType, out pDisambiguationDataRdf);
            return entidaesReconciliadas;
        }


        /// <summary>
        /// Reconcilia el RDF con la BBDD
        /// </summary>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pListaEntidadesReconciliadasDudosas">Lista con las entidades dudosas</param>
        /// <param name="pEntitiesRdfTypes">Lista con los rdf:type de las entidades (con inferencia)</param>
        /// <param name="pEntitiesRdfType">Lista con los rdf:type de las entidades (sin inferencia)</param>
        /// <param name="pDisambiguationDataRdf">Datos para la desambiguación del RDF</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDataInferenceGraph">Grafo en local con los datos del RDF (con inferencia)</param>
        /// <param name="pReasoner">Razonador</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <returns>Diccioario con las entidades reconciliadas</returns>
        private static Dictionary<string, KeyValuePair<string, float>> ReconciliateBBDD(ref Dictionary<string, string> pListaEntidadesReconciliadas, out Dictionary<string, Dictionary<string, float>> pListaEntidadesReconciliadasDudosas, ref Dictionary<string, HashSet<string>> pEntitiesRdfTypes, ref Dictionary<string, string> pEntitiesRdfType, ref Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, ref RohGraph pDataGraph, ref RohGraph pDataInferenceGraph, RohRdfsReasoner pReasoner, Dictionary<string, Dictionary<string, float>> pNamesScore)
        {
            Dictionary<string, KeyValuePair<string, float>> discoveredEntityList = new Dictionary<string, KeyValuePair<string, float>>();
            pListaEntidadesReconciliadasDudosas = new Dictionary<string, Dictionary<string, float>>();
            PrepareData(pDataGraph, pReasoner, out pDataInferenceGraph, out pEntitiesRdfTypes, out pEntitiesRdfType, out pDisambiguationDataRdf);
            bool hayQueReprocesar = true;
            while (hayQueReprocesar)
            {
                Dictionary<string, string> entitiesRdfTypeBBDD;
                Dictionary<string, List<DisambiguationData>> disambiguationDataBBDD = GetDisambiguationDataBBDD(out entitiesRdfTypeBBDD, pEntitiesRdfType, new HashSet<string>(pListaEntidadesReconciliadas.Keys.Union(pListaEntidadesReconciliadas.Values)), pDisambiguationDataRdf, pNamesScore);

                hayQueReprocesar = false;
                Dictionary<string, KeyValuePair<string, float>> listaEntidadesReconciliadasAux = ReconciliateData(ref pListaEntidadesReconciliadas, out pListaEntidadesReconciliadasDudosas, pEntitiesRdfType, pDisambiguationDataRdf, entitiesRdfTypeBBDD, disambiguationDataBBDD, ref pDataGraph, false);
                if (listaEntidadesReconciliadasAux.Count > 0)
                {
                    hayQueReprocesar = true;
                    foreach (string id in listaEntidadesReconciliadasAux.Keys)
                    {
                        discoveredEntityList.Add(id, listaEntidadesReconciliadasAux[id]);
                    }

                }
                PrepareData(pDataGraph, pReasoner, out pDataInferenceGraph, out pEntitiesRdfTypes, out pEntitiesRdfType, out pDisambiguationDataRdf);
            }
            return discoveredEntityList;
        }

        /// <summary>
        /// Efectua la reconciliación con los datos proporcionados
        /// </summary>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pListaEntidadesReconciliadasDudosas"></param>
        /// <param name="pEntitiesRdfType">Diccionario con las entidades encontradas y sus rdf:type</param>
        /// <param name="pEntitiesRdfTypeCandidate">Diccionario con los candidatos y sus rdf:type</param>
        /// <param name="pDisambiguationDataRdf">Datos extraidos del grafo en local para la reconciliación</param>
        /// <param name="pDisambiguationDataCandidate">Datos de los candidatos para actualizar el rdf en local</param>
        /// <param name="pDataGraph">Grafo en local con los datos a procesar</param>
        /// <param name="pExternalIntegration">Indica si es para una integración externa, en ese caso no se hace efectiva la reconciliación y no se aplica la coincidencia de rdftypes incluye la herencia</param>
        /// <returns>Diccionario de entidades reconciliadas</returns>
        private static Dictionary<string, KeyValuePair<string, float>> ReconciliateData(ref Dictionary<string, string> pListaEntidadesReconciliadas, out Dictionary<string, Dictionary<string, float>> pListaEntidadesReconciliadasDudosas, Dictionary<string, string> pEntitiesRdfType, Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, Dictionary<string, string> pEntitiesRdfTypeCandidate, Dictionary<string, List<DisambiguationData>> pDisambiguationDataCandidate, ref RohGraph pDataGraph, bool pExternalIntegration)
        {
            Dictionary<string, KeyValuePair<string, float>> discoveredEntityList = new Dictionary<string, KeyValuePair<string, float>>();
            pListaEntidadesReconciliadasDudosas = new Dictionary<string, Dictionary<string, float>>();

            //Clave ID rdf, valor ID BBDD,score
            Dictionary<string, Dictionary<string, float>> candidatos = new Dictionary<string, Dictionary<string, float>>();

            //Iteramos varias veces para que las dependencias se vayan cargando
            //1.-La primera vez sólo obtenemos las probabilidades positivas con los datos originales
            //2.-La segunda vez sólo obtenemos las probabilidades positivas ayudandonos de los candidatos obtenidos en el punto anterior
            //3.-La tercera y última vez aplicamos también las probabilidades negativas
            for (int i = 0; i < 3; i++)
            {
                //Almacena los candidatos de la iteración
                Dictionary<string, Dictionary<string, float>> candidatosAux = new Dictionary<string, Dictionary<string, float>>();
                bool GetOnlyPositiveScore = false;
                //Primera iteración
                if (i == 0 || i == 1)
                {
                    GetOnlyPositiveScore = true;
                }
                foreach (string entityID_RDF in pEntitiesRdfType.Keys)
                {
                    if (!pListaEntidadesReconciliadas.ContainsKey(entityID_RDF) && pDisambiguationDataRdf.ContainsKey(entityID_RDF))
                    {
                        foreach (DisambiguationData disambiguationData in pDisambiguationDataRdf[entityID_RDF])
                        {
                            //Recorremos los datos de desambiguación del resto de elementos con el mismo rdf:type
                            foreach (KeyValuePair<string, List<DisambiguationData>> candidato in pDisambiguationDataCandidate.Where(x => (!pExternalIntegration && pEntitiesRdfType[entityID_RDF] == pEntitiesRdfTypeCandidate[x.Key]) || (pExternalIntegration)).ToList())
                            {
                                //y la misma configuración de desambiguación
                                DisambiguationData disambiguationDataCandidato = candidato.Value.FirstOrDefault(x => x.disambiguation == disambiguationData.disambiguation);
                                if (disambiguationDataCandidato != null)
                                {
                                    bool mismaEntidad = false;
                                    bool puedeSerMismaEntidad = true;

                                    //Comprobamos los identificadores
                                    if (disambiguationData.identifiers != null && disambiguationData.identifiers.Count > 0 && disambiguationDataCandidato.identifiers != null && disambiguationDataCandidato.identifiers.Count > 0)
                                    {
                                        foreach (string propID in disambiguationData.identifiers.Keys)
                                        {
                                            if (disambiguationDataCandidato.identifiers.ContainsKey(propID))
                                            {
                                                bool coincideID = disambiguationData.identifiers[propID].Intersect(disambiguationDataCandidato.identifiers[propID]).Count() > 0;
                                                if (coincideID)
                                                {
                                                    //Si el identificador coincide es la misma entidad
                                                    mismaEntidad = true;
                                                }
                                                else
                                                {
                                                    //Si tiene diferentesIDs no puede ser la misma entidad
                                                    puedeSerMismaEntidad = false;
                                                }
                                                break;
                                            }
                                        }
                                    }

                                    //Comprobamos las propiedades configuradas
                                    if (puedeSerMismaEntidad && disambiguationData.disambiguation != null && disambiguationDataCandidato.disambiguation != null)
                                    {
                                        float similarity = GetSimilarity(disambiguationData, disambiguationDataCandidato, candidatos, GetOnlyPositiveScore);
                                        if (mismaEntidad)
                                        {
                                            similarity = 1;
                                        }
                                        if (similarity >= mMinScore)
                                        {
                                            if (!candidatosAux.ContainsKey(entityID_RDF))
                                            {
                                                candidatosAux.Add(entityID_RDF, new Dictionary<string, float>());
                                            }
                                            if (!candidatosAux[entityID_RDF].ContainsKey(candidato.Key))
                                            {
                                                candidatosAux[entityID_RDF].Add(candidato.Key, similarity);
                                            }
                                            else if (candidatosAux[entityID_RDF][candidato.Key] < similarity)
                                            {
                                                candidatosAux[entityID_RDF][candidato.Key] = similarity;
                                            }
                                        }
                                    }
                                }

                            }

                        }
                    }
                }
                candidatos = new Dictionary<string, Dictionary<string, float>>(candidatosAux);
            }


            foreach (string entityRDF in candidatos.Keys)
            {
                //Candidatos con un 1 de probabilidad
                List<string> canditosSeguros = candidatos[entityRDF].Where(x => x.Value == 1).ToList().Select(x => x.Key).ToList();
                //Candidatos que superan el umbral máximo (excluyendo los anteriores)
                List<string> canditosUmbralMaximo = candidatos[entityRDF].Where(x => x.Value >= mMaxScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).ToList();
                //Candidatos que superan el umbral mínimo (excluyendo los anteriores)
                List<string> canditosUmbralMinimo = candidatos[entityRDF].Where(x => x.Value >= mMinScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).Except(canditosUmbralMaximo).ToList();


                if (!pExternalIntegration && (canditosSeguros.Count == 1 || canditosUmbralMaximo.Count == 1))
                {
                    //Si sólo hay un candidato seguro
                    //O sólo un candidato supera el umbral máximo realizamos la reconciliación
                    string urlReconciliada = null;
                    if (canditosSeguros.Count == 1)
                    {
                        urlReconciliada = canditosSeguros[0];
                    }
                    else
                    {
                        urlReconciliada = canditosUmbralMaximo[0];
                    }

                    TripleStore store = new TripleStore();
                    store.Add(pDataGraph);
                    //Cambiamos candidato.Key por entityID
                    SparqlUpdateParser parser = new SparqlUpdateParser();
                    //Actualizamos los sujetos
                    SparqlUpdateCommandSet updateSubject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                    INSERT{<" + urlReconciliada + @"> ?p ?o.}
                                                                    WHERE 
                                                                    {
                                                                        ?s ?p ?o.   FILTER(?s = <" + entityRDF + @">)
                                                                    }");
                    //Actualizamos los objetos
                    SparqlUpdateCommandSet updateObject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                    INSERT{?s ?p <" + urlReconciliada + @">.}
                                                                    WHERE 
                                                                    {
                                                                        ?s ?p ?o.   FILTER(?o = <" + entityRDF + @">)
                                                                    }");
                    LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                    processor.ProcessCommandSet(updateSubject);
                    processor.ProcessCommandSet(updateObject);

                    pListaEntidadesReconciliadas.Add(entityRDF, urlReconciliada);
                    float score = 0;
                    if (canditosSeguros.Count == 1)
                    {
                        score = 1;
                    }
                    else
                    {
                        score = candidatos[entityRDF][canditosUmbralMaximo[0]];
                    }
                    discoveredEntityList.Add(entityRDF, new KeyValuePair<string, float>(urlReconciliada, score));
                }
                else if (pExternalIntegration || (canditosUmbralMaximo.Count > 1 || canditosUmbralMinimo.Count > 0))
                {
                    //Si para alguna entidad hay más de un candidato que supere el umbral máximo 
                    //o hay alguna entidad que supere el umbral mínimo pero no alcance el máximo
                    //Lo marcamos para que lo decida el usuario
                    pListaEntidadesReconciliadasDudosas.Add(entityRDF, candidatos[entityRDF]);
                }
            }

            return discoveredEntityList;
        }

        /// <summary>
        /// Obtiene la similaridad entre dos DisambiguationData
        /// </summary>
        /// <param name="pDisambiguationDataOriginal">DisambiguationData Original</param>
        /// <param name="pDisambiguationDataCandidate">DisambiguationData Candidato</param>
        /// <param name="pCandidates">Candidatos a la desambiguación, el sujeto es ID del RDF y loas valores son los posibles valores de la BBDD</param>
        /// <param name="pOnlyPositive">Sólo los calculos positivos</param>
        /// <returns></returns>
        private static float GetSimilarity(DisambiguationData pDisambiguationDataOriginal, DisambiguationData pDisambiguationDataCandidate, Dictionary<string, Dictionary<string, float>> pCandidates, bool pOnlyPositive)
        {
            List<Disambiguation.Property> propieadesObligatorias = pDisambiguationDataOriginal.disambiguation.properties.Where(x => x.mandatory).ToList();
            List<Disambiguation.Property> propieadesComunes = pDisambiguationDataOriginal.properties.Select(x => x.property).ToList().Intersect(pDisambiguationDataCandidate.properties.Select(x => x.property).ToList()).OrderByDescending(x => x.mandatory).ToList();
            //Tiene alguna propiedad en común y tiene al menos todas las obligatorias
            if (propieadesComunes.Count > 0 && propieadesObligatorias.Intersect(propieadesComunes).Count() == propieadesObligatorias.Count)
            {
                float similitudGlobal = 0;
                List<float> similitudesNegativas = new List<float>();
                foreach (Disambiguation.Property propiedad in propieadesComunes)
                {
                    float similitudActual = 0;
                    HashSet<string> valorPropiedadesOriginal = new HashSet<string>(pDisambiguationDataOriginal.properties.First(x => x.property == propiedad).values);
                    HashSet<string> valorPropiedadesCandidato = new HashSet<string>(pDisambiguationDataCandidate.properties.First(x => x.property == propiedad).values);
                    float minScore = 0;
                    if (propiedad.mandatory)
                    {
                        minScore = mMinScore;
                    }
                    switch (propiedad.type)
                    {
                        case Disambiguation.Property.Type.equals:
                            {
                                bool hasCoincidencia = false;
                                if (propiedad.scorePositive.HasValue)
                                {
                                    foreach (string original in valorPropiedadesOriginal)
                                    {
                                        foreach (string candidato in valorPropiedadesCandidato)
                                        {
                                            //Para el valor de las variables originales hay que comprobar su mapeo con los candidatos (además del propio valor)
                                            //Propio valor
                                            float aux = GetSimilarity(original, candidato, propiedad.type);
                                            if (aux > 0)
                                            {
                                                hasCoincidencia = true;
                                                similitudActual += (1 - similitudActual) * aux * (propiedad.scorePositive.Value);
                                            }
                                            else
                                            {
                                                //Si no coincide el valor probamos a usar el valor de los candidatos
                                                if (pCandidates.ContainsKey(original))
                                                {
                                                    foreach (string originalCandidato in pCandidates[original].Keys)
                                                    {
                                                        //cambiar
                                                        float aux2 = GetSimilarity(originalCandidato, candidato, propiedad.type) * pCandidates[original][originalCandidato];
                                                        if (aux2 > 0)
                                                        {
                                                            hasCoincidencia = true;
                                                            similitudActual += (1 - similitudActual) * aux2 * (propiedad.scorePositive.Value);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!pOnlyPositive)
                                {
                                    if (propiedad.scoreNegative.HasValue && valorPropiedadesOriginal.Count > 0 && valorPropiedadesCandidato.Count > 0)
                                    {
                                        if (!hasCoincidencia)
                                        {
                                            similitudesNegativas.Add(propiedad.scoreNegative.Value);
                                        }
                                    }
                                }
                            }
                            break;
                        case Disambiguation.Property.Type.ignoreCaseSensitive:
                            {
                                bool hasCoincidencia = false;
                                if (propiedad.scorePositive.HasValue)
                                {
                                    if (propiedad.scorePositive.HasValue)
                                    {
                                        foreach (string original in valorPropiedadesOriginal)
                                        {
                                            foreach (string candidato in valorPropiedadesCandidato)
                                            {
                                                float aux = GetSimilarity(original, candidato, propiedad.type);
                                                if (aux > 0 && aux > minScore)
                                                {
                                                    hasCoincidencia = true;
                                                }
                                                similitudActual += (1 - similitudActual) * aux * (propiedad.scorePositive.Value);
                                            }
                                        }
                                    }
                                }
                                if (!pOnlyPositive)
                                {
                                    if (propiedad.scoreNegative.HasValue && valorPropiedadesOriginal.Count > 0 && valorPropiedadesCandidato.Count > 0)
                                    {
                                        if (!hasCoincidencia)
                                        {
                                            similitudesNegativas.Add(propiedad.scoreNegative.Value);
                                        }
                                    }
                                }
                            }
                            break;
                        case Disambiguation.Property.Type.name:
                            {
                                bool hasCoincidencia = false;
                                if (propiedad.scorePositive.HasValue)
                                {
                                    float max = 0;
                                    foreach (string propOriginal in valorPropiedadesOriginal)
                                    {
                                        foreach (string propCandidato in valorPropiedadesCandidato)
                                        {
                                            float aux = GetSimilarity(propOriginal, propCandidato, propiedad.type);
                                            if (aux > max && aux > minScore)
                                            {
                                                max = aux;
                                                hasCoincidencia = true;
                                            }
                                        }
                                    }
                                    similitudActual += (1 - similitudActual) * (propiedad.scorePositive.Value * max);
                                }
                                if (!pOnlyPositive)
                                {
                                    if (propiedad.scoreNegative.HasValue && valorPropiedadesOriginal.Count > 0 && valorPropiedadesCandidato.Count > 0)
                                    {
                                        if (!hasCoincidencia)
                                        {
                                            similitudesNegativas.Add(propiedad.scoreNegative.Value);
                                        }
                                    }
                                }
                            }
                            break;
                        case Disambiguation.Property.Type.title:
                            {
                                bool hasCoincidencia = false;
                                if (propiedad.scorePositive.HasValue)
                                {
                                    float max = 0;
                                    foreach (string propOriginal in valorPropiedadesOriginal)
                                    {
                                        foreach (string propCandidato in valorPropiedadesCandidato)
                                        {
                                            float aux = GetSimilarity(propOriginal, propCandidato, propiedad.type, propiedad.maxNumWordsTitle);
                                            if (aux > max && aux > minScore)
                                            {
                                                max = aux;
                                                hasCoincidencia = true;
                                            }
                                        }
                                    }
                                    similitudActual += (1 - similitudActual) * (propiedad.scorePositive.Value * max);
                                }
                                if (!pOnlyPositive)
                                {
                                    if (propiedad.scoreNegative.HasValue && valorPropiedadesOriginal.Count > 0 && valorPropiedadesCandidato.Count > 0)
                                    {
                                        if (!hasCoincidencia)
                                        {
                                            similitudesNegativas.Add(propiedad.scoreNegative.Value);
                                        }
                                    }
                                }
                            }
                            break;

                    }
                    if (propiedad.mandatory && similitudActual == 0)
                    {
                        //Si la propiedad es obligatoria y no ha obtenido puntuación no recuperamos nada
                        similitudGlobal = 0;
                        break;
                    }
                    else
                    {
                        similitudGlobal += (1 - similitudGlobal) * (similitudActual);
                    }
                }
                if (similitudesNegativas.Count > 0 && similitudGlobal > 0)
                {
                    foreach (float similitudNegativa in similitudesNegativas)
                    {
                        similitudGlobal -= similitudNegativa;
                    }
                }
                return similitudGlobal;
            }
            return 0;
        }

        /// <summary>
        /// Obtenemos la similitud
        /// </summary>
        /// <param name="pOriginal">Dato Original</param>
        /// <param name="pCandidato">Dato Candidato</param>
        /// <param name="pMaxNumWordsTitle">Número de palabras a partir de la cual la similitud de tipo 'title' obtiene la máxima puntuación</param>
        /// <param name="pType">Tipo de similitud</param>
        /// <returns></returns>
        private static float GetSimilarity(string pOriginal, string pCandidato, Disambiguation.Property.Type pType, int? pMaxNumWordsTitle = null)
        {
            float similarity = 0;
            switch (pType)
            {
                case Disambiguation.Property.Type.equals:
                    if (pOriginal == pCandidato)
                    {
                        similarity = 1;
                    }
                    break;
                case Disambiguation.Property.Type.ignoreCaseSensitive:
                    if (pOriginal.ToLower() == pCandidato.ToLower())
                    {
                        similarity = 1;
                    }
                    break;
                case Disambiguation.Property.Type.name:
                    {
                        string originalAux = pOriginal;
                        if (originalAux.StartsWith("\""))
                        {
                            originalAux = originalAux.Substring(1, originalAux.LastIndexOf("\"") - 1).Trim(new char[] { ' ', '-' });
                        }
                        string candidatoAux = pCandidato;
                        if (candidatoAux.StartsWith("\""))
                        {
                            candidatoAux = candidatoAux.Substring(1, candidatoAux.LastIndexOf("\"") - 1).Trim(new char[] { ' ', '-' });
                        }
                        similarity = GetNameSimilarity(originalAux, candidatoAux);
                    }
                    break;
                case Disambiguation.Property.Type.title:
                    {
                        int numWords = pOriginal.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Count();
                        float scoreByNumWords;
                        if (numWords >= pMaxNumWordsTitle.Value)
                        {
                            scoreByNumWords = 1;
                        }
                        else
                        {
                            scoreByNumWords = mMinScore + (1 - mMinScore) * ((float)numWords / (float)pMaxNumWordsTitle.Value);
                        }

                        string originalAux = pOriginal;
                        if (originalAux.StartsWith("\""))
                        {
                            originalAux = originalAux.Substring(1, originalAux.LastIndexOf("\"") - 1).ToLower();
                        }
                        foreach (char value in charRemovePropertyTitle.Keys)
                        {
                            if (charRemovePropertyTitle[value].HasValue)
                            {
                                originalAux = originalAux.Replace(value, charRemovePropertyTitle[value].Value);
                            }
                            else
                            {
                                originalAux = originalAux.Replace(value.ToString(), "");
                            }
                        }
                        string candidatoAux = pCandidato;
                        if (candidatoAux.StartsWith("\""))
                        {
                            candidatoAux = candidatoAux.Substring(1, candidatoAux.LastIndexOf("\"") - 1).ToLower();
                        }
                        foreach (char value in charRemovePropertyTitle.Keys)
                        {
                            if (charRemovePropertyTitle[value].HasValue)
                            {
                                candidatoAux = candidatoAux.Replace(value, charRemovePropertyTitle[value].Value);
                            }
                            else
                            {
                                candidatoAux = candidatoAux.Replace(value.ToString(), "");
                            }
                        }
                        if (originalAux == candidatoAux)
                        {
                            similarity = 1f * scoreByNumWords;
                        }
                        else
                        {
                            similarity = 0f;
                        }
                    }
                    break;

            }
            return similarity;
        }

        #endregion

        #region Integración con APIs externos

        /// <summary>
        /// Integración con ORCID
        /// </summary>
        /// <param name="pDataGraph">Grafo en local en el que aplicar la implementación de ORCID</param>
        /// <param name="pDataInferenceGraph">Grafo en local en el que aplicar la implementación de ORCID (con inferencia)</param>
        /// <param name="pReasoner">Razonador para la inferencia de la ontología</param>
        /// <returns>Identificadores descubiertos con la integración de ORCID</returns>
        private static Dictionary<string,Dictionary<string,string>> ORCIDIntegration(ref RohGraph pDataGraph, ref RohGraph pDataInferenceGraph, RohRdfsReasoner pReasoner)
        {
            Dictionary<string, Dictionary<string, string>> identifiersDiscover = new Dictionary<string, Dictionary<string, string>>();

            //Obtenemos las personas del RDF junto con sus nombres (que no tengan cargado el ORCID)
            Dictionary<string, string> personWithName = new Dictionary<string, string>();
            string query = @"select distinct ?s ?name where{?s a <http://purl.org/roh/mirror/foaf#Person>. ?s <http://purl.org/roh/mirror/foaf#name>  ?name. MINUS{?s <http://purl.org/roh#ORCID> ?orcid }}";
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                if (sparqlResult["name"] is LiteralNode)
                {
                    string name = ((LiteralNode)sparqlResult["name"]).Value;
                    if (!personWithName.ContainsKey(sparqlResult["s"].ToString()))
                    {
                        personWithName.Add(sparqlResult["s"].ToString(), name);
                    }
                }
            }

            if (personWithName.Count > 0)
            {
                TripleStore store = new TripleStore();
                store.Add(pDataGraph);

                //Obtenemos los identificadores de la BBDD en caso de que ya estén cargados
                Dictionary<string, string> entityORCID = new Dictionary<string, string>();
                Dictionary<string, string> entityResearcherID = new Dictionary<string, string>();
                Dictionary<string, string> entityScopusID = new Dictionary<string, string>();

                string consulta = @$"select ?s ?orcid ?researcherid ?scopusid
                                where
                                {{
                                    ?s a <http://purl.org/roh/mirror/foaf#Person>. 
                                    OPTIONAL{{?s <http://purl.org/roh#ORCID> ?orcid}}
                                    OPTIONAL{{?s <http://purl.org/roh/mirror/vivo#researcherId> ?researcherid}}
                                    OPTIONAL{{?s <http://purl.org/roh/mirror/vivo#scopusId> ?scopusid}}
                                    Filter(?s in (<" + string.Join(">,<", personWithName.Keys) + ">))" +
                                    "}";
                SparqlObject sparqlObject = SparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    string s = row["s"].value;
                    if (row.ContainsKey("orcid"))
                    {
                        entityORCID[s] = row["orcid"].value;
                    }
                    if (row.ContainsKey("researcherid"))
                    {
                        entityResearcherID[s] = row["researcherid"].value;
                    }
                    if (row.ContainsKey("scopusid"))
                    {
                        entityScopusID[s] = row["scopusid"].value;
                    }
                }

                //Cargamos en el RDF los que hayamos encontrado en la BBDD
                if (entityORCID.Count > 0)
                {
                    foreach (string entityID in entityORCID.Keys)
                    {
                        IUriNode s = pDataGraph.CreateUriNode(UriFactory.Create(entityID));
                        IUriNode p = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                        ILiteralNode o = pDataGraph.CreateLiteralNode(entityORCID[entityID], new Uri("http://www.w3.org/2001/XMLSchema#string"));
                        pDataGraph.Assert(new Triple(s, p, o));
                        if(!identifiersDiscover.ContainsKey(entityID))
                        {
                            identifiersDiscover.Add(entityID, new Dictionary<string, string>());
                        }
                        identifiersDiscover[entityID].Add("http://purl.org/roh#ORCID", entityORCID[entityID]);
                    }
                }
                if (entityResearcherID.Count > 0)
                {
                    foreach (string entityID in entityResearcherID.Keys)
                    {
                        IUriNode s = pDataGraph.CreateUriNode(UriFactory.Create(entityID));
                        IUriNode p = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#researcherId"));
                        ILiteralNode o = pDataGraph.CreateLiteralNode(entityResearcherID[entityID], new Uri("http://www.w3.org/2001/XMLSchema#string"));
                        pDataGraph.Assert(new Triple(s, p, o));
                        if (!identifiersDiscover.ContainsKey(entityID))
                        {
                            identifiersDiscover.Add(entityID, new Dictionary<string, string>());
                        }
                        identifiersDiscover[entityID].Add("http://purl.org/roh/mirror/vivo#researcherId", entityResearcherID[entityID]);
                    }
                }
                if (entityScopusID.Count > 0)
                {
                    foreach (string entityID in entityScopusID.Keys)
                    {
                        IUriNode s = pDataGraph.CreateUriNode(UriFactory.Create(entityID));
                        IUriNode p = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#scopusId"));
                        ILiteralNode o = pDataGraph.CreateLiteralNode(entityScopusID[entityID], new Uri("http://www.w3.org/2001/XMLSchema#string"));
                        pDataGraph.Assert(new Triple(s, p, o));
                        if (!identifiersDiscover.ContainsKey(entityID))
                        {
                            identifiersDiscover.Add(entityID, new Dictionary<string, string>());
                        }
                        identifiersDiscover[entityID].Add("http://purl.org/roh/mirror/vivo#scopusId", entityScopusID[entityID]);
                    }
                }


                //Buscamos en el API de ORCID aquellas personas para las que no hayamos conseguido su ORCID
                List<string> entitiesSearchORCID = personWithName.Keys.Except(entityORCID.Keys).ToList();
                if (entitiesSearchORCID.Count > 0)
                {
                    //Obtenemos los datos del grafo de carga
                    Dictionary<string, HashSet<string>> entitiesRdfTypesRDF;
                    Dictionary<string, string> entitiesRdfTypeRDF;
                    Dictionary<string, List<DisambiguationData>> disambiguationDataRDF;
                    PrepareData(pDataGraph, pReasoner, out pDataInferenceGraph, out entitiesRdfTypesRDF, out entitiesRdfTypeRDF, out disambiguationDataRDF, false);

                    foreach (string personID in entitiesSearchORCID)
                    {
                        WebClient webClient = new WebClient();
                        webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
                        //1.-Hacemos una petición a ORCID al método  ‘expanded - search' con el nombre de la persona
                        string jsonRespuesta = webClient.DownloadString("https://pub.orcid.org/v3.0/expanded-search?q=" + HttpUtility.UrlEncode(NormalizeName(personWithName[personID])) + "&rows=5");

                        ORCIDExpandedSearch expandedSearch = JsonConvert.DeserializeObject<ORCIDExpandedSearch>(jsonRespuesta);
                        if (expandedSearch.expanded_result.Count > 5)
                        {
                            expandedSearch.expanded_result = expandedSearch.expanded_result.GetRange(0, 5);
                        }

                        RohGraph orcidGraph = new RohGraph();

                        foreach (ORCIDExpandedSearch.Result result in expandedSearch.expanded_result)
                        {

                            string name = result.given_names + " " + result.family_names;
                            string id = "http://orcid.com/Person/" + result.orcid_id;

                            //comprobamos la similitud del nombre obtenido con el nombre del RDF
                            //Si no alcanza un mínimo de similitud procedemos con el siguiente resultado que habíamos obtenido con el método 'expanded - search’, así hasta llegar al 5º          
                            if (!string.IsNullOrEmpty(result.orcid_id) && GetNameSimilarity(name, personWithName[personID]) >= 0.5f)
                            {
                                //Si sí que se alcanza esa similitud se procede con el siguiente paso.

                                //Insertamos los datos de la persona (nombre + ORCID)
                                IUriNode subjectPerson = orcidGraph.CreateUriNode(UriFactory.Create(id));
                                IUriNode rdftypeProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                                IUriNode rdftypePerson = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                orcidGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                IUriNode nameProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                ILiteralNode namePerson = orcidGraph.CreateLiteralNode(name, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                orcidGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                IUriNode orcidProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                ILiteralNode nameOrcid = orcidGraph.CreateLiteralNode(result.orcid_id, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                orcidGraph.Assert(new Triple(subjectPerson, orcidProperty, nameOrcid));



                                //Hacemos peticiones al métdo dee ORCID ‘orcid}/ person' y almacenamos en un grafo en local los datos de los identificadores
                                webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
                                string jsonRespuestaOrcidPerson = webClient.DownloadString("https://pub.orcid.org/v3.0/" + result.orcid_id + "/person");
                                ORCIDPerson person = JsonConvert.DeserializeObject<ORCIDPerson>(jsonRespuestaOrcidPerson);

                                if (person.external_identifiers != null && person.external_identifiers.external_identifier != null)
                                {
                                    foreach (ORCIDPerson.ExternalIdentifiers.ExternalIdentifier extIdentifier in person.external_identifiers.external_identifier)
                                    {
                                        if (extIdentifier.external_id_type == "ResearcherID")
                                        {
                                            IUriNode researcherProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#researcherId"));
                                            ILiteralNode nameResearcher = orcidGraph.CreateLiteralNode(extIdentifier.external_id_value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            orcidGraph.Assert(new Triple(subjectPerson, researcherProperty, nameResearcher));
                                        }
                                        else if (extIdentifier.external_id_type == "Scopus Author ID")
                                        {
                                            IUriNode scopusProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#scopusId"));
                                            ILiteralNode nameScopus = orcidGraph.CreateLiteralNode(extIdentifier.external_id_value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            orcidGraph.Assert(new Triple(subjectPerson, scopusProperty, nameScopus));
                                        }
                                    }
                                }


                                //y 'orcid}/ works’ y almacenamos en un grafo en local los datos de los trabajos realizados.
                                webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
                                string jsonRespuestaOrcidWorks = webClient.DownloadString("https://pub.orcid.org/v3.0/" + result.orcid_id + "/works");
                                ORCIDWorks works = JsonConvert.DeserializeObject<ORCIDWorks>(jsonRespuestaOrcidWorks);
                                HashSet<string> worksTitles = new HashSet<string>();
                                if (works.group != null)
                                {
                                    foreach (ORCIDWorks.Group group in works.group)
                                    {
                                        if (group.work_summary != null)
                                        {
                                            foreach (ORCIDWorks.Group.WorkSummary workSummary in group.work_summary)
                                            {
                                                if (workSummary.title != null && workSummary.title.title2 != null && workSummary.title.title2.value != null)
                                                {
                                                    worksTitles.Add(workSummary.title.title2.value);
                                                }
                                            }
                                        }
                                    }
                                }
                                foreach (string workTitle in worksTitles)
                                {
                                    IUriNode subjectWork = orcidGraph.CreateUriNode(UriFactory.Create("http://orcid.com/Work/" + Guid.NewGuid()));

                                    IUriNode titleProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                    ILiteralNode nameTitle = orcidGraph.CreateLiteralNode(workTitle, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    orcidGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                                    IBlankNode subjectAuthorList = orcidGraph.CreateBlankNode();
                                    IUriNode rdftypeAuthorList = orcidGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                                    orcidGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                                    IUriNode authorListProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                                    orcidGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                                    IUriNode firstAuthorProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                    orcidGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));

                                    IUriNode rdftypeDocument = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                                    orcidGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));
                                }
                            }
                        }

                        //Obtenemos los datos del grafo de ORCID
                        RohGraph dataInferenceGraphORCID;
                        Dictionary<string, HashSet<string>> entitiesRdfTypesORCID;
                        Dictionary<string, string> entitiesRdfTypeORCID;
                        Dictionary<string, List<DisambiguationData>> disambiguationDataORCID;
                        PrepareData(orcidGraph, pReasoner, out dataInferenceGraphORCID, out entitiesRdfTypesORCID, out entitiesRdfTypeORCID, out disambiguationDataORCID, false);

                        Dictionary<string, string> aux = new Dictionary<string, string>();
                        Dictionary<string, Dictionary<string, float>> listaEntidadesReconciliadasDudosas;
                        ReconciliateData(ref aux, out listaEntidadesReconciliadasDudosas, entitiesRdfTypeRDF, disambiguationDataRDF, entitiesRdfTypeORCID, disambiguationDataORCID, ref pDataGraph, true);

                        if (listaEntidadesReconciliadasDudosas.ContainsKey(personID))
                        {
                            //Candidatos con un 1 de probabilidad
                            List<string> canditosSeguros = listaEntidadesReconciliadasDudosas[personID].Where(x => x.Value == 1).ToList().Select(x => x.Key).ToList();
                            //Candidatos que superan el umbral máximo (excluyendo los anteriores)
                            List<string> canditosUmbralMaximo = listaEntidadesReconciliadasDudosas[personID].Where(x => x.Value >= mMaxScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).ToList();

                            string personIDORCIDGraph = "";
                            if (canditosSeguros.Count == 1)
                            {
                                personIDORCIDGraph = canditosSeguros[0];
                            }
                            else if (canditosUmbralMaximo.Count == 1)
                            {
                                personIDORCIDGraph = canditosUmbralMaximo[0];
                            }
                            if (!string.IsNullOrEmpty(personIDORCIDGraph))
                            {
                                string consultaORCIDGraph = @$"select ?orcid ?researcherid ?scopusid
                                where
                                {{
                                    ?s a <http://purl.org/roh/mirror/foaf#Person>. 
                                    OPTIONAL{{?s <http://purl.org/roh#ORCID> ?orcid}}
                                    OPTIONAL{{?s <http://purl.org/roh/mirror/vivo#researcherId> ?researcherid}}
                                    OPTIONAL{{?s <http://purl.org/roh/mirror/vivo#scopusId> ?scopusid}}
                                    Filter(?s = <" + personIDORCIDGraph + ">)" +
                                    "}";
                                SparqlResultSet sparqlResultSetORCIDGraph = (SparqlResultSet)orcidGraph.ExecuteQuery(consultaORCIDGraph);
                                foreach (SparqlResult sparqlResult in sparqlResultSetORCIDGraph.Results)
                                {
                                    IUriNode s = pDataGraph.CreateUriNode(UriFactory.Create(personID));
                                    if (sparqlResult.HasValue("orcid"))
                                    {

                                        IUriNode p = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                        ILiteralNode o = pDataGraph.CreateLiteralNode(((LiteralNode)sparqlResult["orcid"]).Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        pDataGraph.Assert(new Triple(s, p, o));
                                        if (!identifiersDiscover.ContainsKey(personID))
                                        {
                                            identifiersDiscover.Add(personID, new Dictionary<string, string>());
                                        }
                                        identifiersDiscover[personID].Add("http://purl.org/roh#ORCID", ((LiteralNode)sparqlResult["orcid"]).Value);
                                    }
                                    if (sparqlResult.HasValue("researcherid"))
                                    {
                                        IUriNode p = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#researcherId"));
                                        ILiteralNode o = pDataGraph.CreateLiteralNode(((LiteralNode)sparqlResult["researcherid"]).Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        pDataGraph.Assert(new Triple(s, p, o));
                                        if (!identifiersDiscover.ContainsKey(personID))
                                        {
                                            identifiersDiscover.Add(personID, new Dictionary<string, string>());
                                        }
                                        identifiersDiscover[personID].Add("http://purl.org/roh/mirror/vivo#researcherId", ((LiteralNode)sparqlResult["researcherid"]).Value);
                                    }
                                    if (sparqlResult.HasValue("scopusid"))
                                    {
                                        IUriNode p = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#scopusId"));
                                        ILiteralNode o = pDataGraph.CreateLiteralNode(((LiteralNode)sparqlResult["scopusid"]).Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        pDataGraph.Assert(new Triple(s, p, o));
                                        if (!identifiersDiscover.ContainsKey(personID))
                                        {
                                            identifiersDiscover.Add(personID, new Dictionary<string, string>());
                                        }
                                        identifiersDiscover[personID].Add("http://purl.org/roh/mirror/vivo#scopusId", ((LiteralNode)sparqlResult["scopusid"]).Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return identifiersDiscover;
        }
        #endregion

        #region Auxiliares
        /// <summary>
        /// Divide una lista en N listas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pItems">Lista</param>
        /// <param name="pSize">Tamaño máximo de las sublistas devueltas</param>
        /// <returns>Lista de listas</returns>
        private static IEnumerable<List<T>> SplitList<T>(List<T> pItems, int pSize)
        {
            for (int i = 0; i < pItems.Count; i += pSize)
            {
                yield return pItems.GetRange(i, Math.Min(pSize, pItems.Count - i));
            }
        }

        /// <summary>
        /// Array para aplicar en las restricciones de tipo 'title'
        /// </summary>
        private readonly static Dictionary<char, char?> charRemovePropertyTitle = new Dictionary<char, char?>()
        {
            { ' ',null },
            { '.', null },
            { ',',null },
            { ';', null },
            { ':', null },
            { '"', null },
            { '\'',null },
            { '-', null },
            { '(', null },
            { ')', null },
            { '[', null },
            { ']', null },
            { '{', null },
            { '}', null },
            { 'á', 'a' },
            { 'é', 'e' },
            { 'í', 'i' },
            { 'ó', 'o' },
            { 'ú', 'u' },
            { 'ñ', 'n' },
            { '\\', null },
            { '/', null },
        };

        /// <summary>
        /// Obtiene la similitud de dos nombres de personas, a partir de 0.5, se consideran probables
        /// </summary>
        /// <param name="pNombreA">Nombre A</param>
        /// <param name="pNombreB">Nombre B</param>
        /// <returns>Scores</returns>
        private static float GetNameSimilarity(string pNombreA, string pNombreB)
        {
            float indice_desplazamiento = 5;
            float scoreMin = 0.5f;

            string nombreANormalizado = NormalizeName(pNombreA);
            string nombreBNormalizado = NormalizeName(pNombreB);

            string[] nombreANormalizadoSplit = nombreANormalizado.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] nombreBNormalizadoSplit = nombreBNormalizado.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            string[] source = nombreANormalizadoSplit;
            string[] target = nombreBNormalizadoSplit;
            if (nombreANormalizadoSplit.Length > nombreBNormalizadoSplit.Length || (nombreANormalizadoSplit.Length == nombreBNormalizadoSplit.Length && string.Compare(nombreANormalizado, nombreBNormalizado) > 0))
            {
                source = nombreBNormalizadoSplit;
                target = nombreANormalizadoSplit;
            }

            int indexTarget = 0;
            List<float> scores = new List<float>();
            for (int i = 0; i < source.Length; i++)
            {
                float score = 0;
                string word = source[i];
                bool wordInicial = word.Length == 1;
                int desplazamiento = 0;
                for (int j = indexTarget; j < target.Length; j++)
                {
                    string word2 = target[j];
                    bool word2Inicial = word2.Length == 1;
                    if (wordInicial || word2Inicial)
                    {
                        if (word[0] == word2[0])
                        {
                            score = scoreMin;
                            indexTarget = j + 1;
                            desplazamiento = Math.Abs(j - i);
                            break;
                        }
                    }
                    float scoreSingleName = CompareSingleName(word, word2);
                    if (scoreSingleName >= scoreMin)
                    {
                        score = scoreSingleName;
                        indexTarget = j + 1;
                        desplazamiento = Math.Abs(j - i);
                        break;
                    }
                }
                float coefJaccardGNOSS = score * (indice_desplazamiento / (desplazamiento + indice_desplazamiento));
                scores.Add(coefJaccardGNOSS);
            }
            return scores.Sum() / Math.Max(source.Length, target.Length);

        }

        /// <summary>
        /// Obtiene el coeficiente jackard de dos nombres
        /// </summary>
        /// <param name="pNameA">Nombre A</param>
        /// <param name="pNameB">Nombre B</param>
        /// <returns>Coeficiente</returns>
        private static float CompareSingleName(string pNameA, string pNameB)
        {
            HashSet<string> ngramsNameA = GetNGramas(pNameA, 2);
            HashSet<string> ngramsNameB = GetNGramas(pNameB, 2);
            float tokens_comunes = ngramsNameA.Intersect(ngramsNameB).Count();
            float union_tokens = ngramsNameA.Union(ngramsNameB).Count();
            float coeficiente_jackard = tokens_comunes / union_tokens;
            return coeficiente_jackard;
        }

        /// <summary>
        /// Obtiene ngramas del tamaño especificado de un texto
        /// </summary>
        /// <param name="pText">Texto del que obtener los ngramas</param>
        /// <param name="pNgramSize">Tamaño de los ngramas</param>
        /// <returns>Lista de ngramas</returns>
        private static HashSet<string> GetNGramas(string pText, int pNgramSize)
        {
            HashSet<string> ngramas = new HashSet<string>();
            int textLength = pText.Length;
            if (pNgramSize == 1)
            {
                for (int i = 0; i < textLength; i++)
                {
                    ngramas.Add(pText[i].ToString());
                }
                return ngramas;
            }

            HashSet<string> ngramasaux = new HashSet<string>();
            for (int i = 0; i < textLength; i++)
            {
                foreach (string ngram in ngramasaux.ToList())
                {
                    string ngamaux = ngram + pText[i];
                    if (ngamaux.Length == pNgramSize)
                    {
                        ngramas.Add(ngamaux);
                    }
                    else
                    {
                        ngramasaux.Add(ngamaux);
                    }
                    ngramasaux.Remove(ngram);
                }
                ngramasaux.Add(pText[i].ToString());
                if (i < pNgramSize)
                {
                    foreach (string ngrama in ngramasaux)
                    {
                        if (ngrama.Length == i + 1)
                        {
                            ngramas.Add(ngrama);
                        }
                    }
                }
            }
            for (int i = (textLength - pNgramSize) + 1; i < textLength; i++)
            {
                if (i >= pNgramSize)
                {
                    ngramas.Add(pText.Substring(i));
                }
            }
            return ngramas;
        }

        /// <summary>
        /// Normaliza un texto eliminando acentos y caracteres que no sean letras
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string NormalizeName(string text)
        {
            //Si tiene ',' lo reordenamos
            if (text.Contains(","))
            {
                text = text.Substring(text.IndexOf(",") + 1) + " " + text.Substring(0, text.IndexOf(","));
            }
            text = text.Trim().ToLower();
            //Eliminamos artículos típicos
            text = text.Replace(" de ", " ");
            text = text.Replace(" la ", " ");
            text = text.Replace(" del ", " ");
            StringBuilder sb = new StringBuilder();
            foreach (char charin in text)
            {
                if (char.IsLetter(charin))
                {
                    sb.Append(charin);
                }
                else
                {
                    sb.Append(' ');
                }
            }
            var normalizedString = sb.ToString().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        #endregion

    }
}
