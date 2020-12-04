using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Entities.ExternalAPIs;
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
using System.Threading;
using System.Web;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Inference;
using VDS.RDF.Update;

namespace API_DISCOVER.Utility
{
    /// <summary>
    /// Clase para el descubrimiento
    /// </summary>
    public static class Discover
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly static float mMaxScore = mConfigService.GetMaxScore();
        private readonly static float mMinScore = mConfigService.GetMinScore();
        private readonly static List<Disambiguation> mDisambiguationConfigs = LoadDisambiguationConfigs();
        private readonly static ConfigSparql mConfigSparql = new ConfigSparql();
        private readonly static string mSPARQLEndpoint = mConfigSparql.GetEndpoint();
        private readonly static string mGraph = mConfigSparql.GetGraph();
        private readonly static string mQueryParam = mConfigSparql.GetQueryParam();
        private readonly static ConfigScopus mConfigScopus = new ConfigScopus();
        private readonly static string mScopusApiKey = mConfigScopus.GetScopusApiKey();
        private readonly static string mScopusUrl = mConfigScopus.GetScopusUrl();
        private readonly static ConfigCrossref mConfigCrossref = new ConfigCrossref();
        private readonly static string mCrossrefUserAgent = mConfigCrossref.GetCrossrefUserAgent();
        private readonly static ConfigWOS mConfigWOS = new ConfigWOS();
        private readonly static string mWOSAuthorization = mConfigWOS.GetWOSAuthorization();
        public static I_SparqlUtility mSparqlUtility = new SparqlUtility();


        private readonly static string mPropertyRohIdentifier = "http://purl.org/dc/terms/identifier";

        /// <summary>
        /// Realiza el desubrimiento sobre un RDF
        /// </summary>
        /// <param name="pDiscoverItem">Item de descubrimiento</param>
        /// <param name="pCallEtlApiService">Servicio para hacer llamadas a los métodos del controlador etl del API_CARGA </param>
        /// <returns>DiscoverResult con los datos del descubrimiento</returns>
        public static DiscoverResult Init(DiscoverItem pDiscoverItem, CallEtlApiService pCallEtlApiService)
        {
            DateTime discoverInitTime = DateTime.Now;
            //Cargamos la ontología           
            RohGraph ontologyGraph = pCallEtlApiService.CallGetOntology();

            //Cargamos datos del RDF
            RohGraph dataGraph = new RohGraph();
            Dictionary<string, HashSet<string>> discardDissambiguations = new Dictionary<string, HashSet<string>>();
            if (!string.IsNullOrEmpty(pDiscoverItem.DiscoverRdf))
            {
                //Si tenemos valor en DiscoverRdf, trabajamos con este RDF, ya que estamos reprocesando un rdf validado
                dataGraph.LoadFromString(pDiscoverItem.DiscoverRdf, new RdfXmlParser());
                if (pDiscoverItem.DiscardDissambiguations != null)
                {
                    foreach (DiscoverItem.DiscardDissambiguation discardDissambiguation in pDiscoverItem.DiscardDissambiguations)
                    {
                        if (!discardDissambiguations.ContainsKey(discardDissambiguation.IDOrigin))
                        {
                            discardDissambiguations.Add(discardDissambiguation.IDOrigin, new HashSet<string>());
                        }
                        discardDissambiguations[discardDissambiguation.IDOrigin].UnionWith(discardDissambiguation.DiscardCandidates);
                    }
                }
            }
            else
            {
                dataGraph.LoadFromString(pDiscoverItem.Rdf, new RdfXmlParser());
            }


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
            Dictionary<string, KeyValuePair<string, float>> discoveredEntitiesWithExternalIntegration = new Dictionary<string, KeyValuePair<string, float>>();

            //Almacenamos las entidades que han sido descubiertas, la clave es el sujeto original y el valor es el nuevo valor            
            Dictionary<string, string> discoveredEntityList = new Dictionary<string, string>();

            //Almacenamos las entidades con dudas acerca de su descubrimiento
            Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbability = new Dictionary<string, Dictionary<string, float>>();

            Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> externalIntegration = new Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>>();

            if (!pDiscoverItem.DissambiguationProcessed)
            {
                //Obtenemos los nombres de todas las personas que haya cargadas en la BBDD
                //Clave ID,
                //Valor nombre
                Dictionary<string, string> personsWithName = LoadPersonWithName();

                //Aquí se almacenarán los nombres de las personas del RDF, junto con los candidatos de la BBDD y su score
                Dictionary<string, Dictionary<string, float>> namesScore = new Dictionary<string, Dictionary<string, float>>();

                bool hasChanges = true;

                DiscoverCache discoverCache = new DiscoverCache();

                //Se realizarán este proceso iterativamente hasta que no haya ningún cambio en lo que a reconciliaciones se refiere
                while (hasChanges)
                {
                    hasChanges = false;

                    Dictionary<string, HashSet<string>> entitiesRdfTypes;
                    Dictionary<string, string> entitiesRdfType;
                    Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;

                    //Preparamos los datos para proceder con la reconciliazción
                    PrepareData(dataGraph, reasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf, false);

                    //Carga los scores de las personas
                    LoadNamesScore(ref namesScore, personsWithName, dataInferenceGraph, discoverCache);

                    //0.- Macamos como reconciliadas aquellas que ya estén cargadas en la BBDD con los mismos identificadores
                    List<string> entidadesCargadas = LoadEntitiesDB(entitiesRdfType.Keys.ToList().Except(discoveredEntityList.Keys.Union(discoveredEntityList.Values))).Keys.ToList();
                    foreach (string entitiID in entidadesCargadas)
                    {
                        discoveredEntityList.Add(entitiID, entitiID);
                        discoveredEntitiesWithSubject.Add(entitiID);
                    }

                    //1.- Realizamos reconciliación con los identificadores configurados (y el roh:identifier) y marcamos como reconciliadas las entidades seleccionadas para no intentar reconciliarlas posteriormente
                    Dictionary<string, string> entidadesReconciliadasConIdsAux = ReconciliateIDs(ref hasChanges, ref discoveredEntityList, entitiesRdfType, disambiguationDataRdf, discardDissambiguations, ref dataGraph);
                    foreach (string id in entidadesReconciliadasConIdsAux.Keys)
                    {
                        discoveredEntitiesWithIds.Add(id, entidadesReconciliadasConIdsAux[id]);
                    }

                    //2.- Realizamos la reconciliación con los datos del Propio RDF
                    //TODO eliminar propiedades monovaliads
                    ReconciliateRDF(ref hasChanges, ref discoveredEntityList, ref dataGraph, reasoner, discardDissambiguations, discoverCache);

                    //3.- Realizamos la reconciliación con los datos de la BBDD
                    Dictionary<string, KeyValuePair<string, float>> entidadesReconciliadasConBBDDAux = ReconciliateBBDD(ref hasChanges, ref discoveredEntityList, out discoveredEntitiesProbability, ref dataGraph, reasoner, namesScore, discardDissambiguations, discoverCache);
                    foreach (string id in entidadesReconciliadasConBBDDAux.Keys)
                    {
                        discoveredEntitiesWithBBDD.Add(id, entidadesReconciliadasConBBDDAux[id]);
                    }

                    //4.- Realizamos la reconciliación con los datos de las integraciones externas
                    Dictionary<string, KeyValuePair<string, float>> entidadesReconciliadasConIntegracionExternaAux;

                    Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> externalIntegrationAux = ExternalIntegration(ref hasChanges, ref discoveredEntityList, ref discoveredEntitiesProbability, ref dataGraph, reasoner, namesScore, ontologyGraph, out entidadesReconciliadasConIntegracionExternaAux, discardDissambiguations, discoverCache);
                    foreach (string id in entidadesReconciliadasConIntegracionExternaAux.Keys)
                    {
                        discoveredEntitiesWithExternalIntegration.Add(id, entidadesReconciliadasConIntegracionExternaAux[id]);
                    }
                    foreach (string id in externalIntegrationAux.Keys)
                    {
                        if (!externalIntegration.ContainsKey(id))
                        {
                            externalIntegration.Add(id, externalIntegrationAux[id]);
                        }
                        else
                        {
                            foreach (string idProp in externalIntegrationAux[id].Keys)
                            {
                                if (!externalIntegration[id].ContainsKey(idProp))
                                {
                                    externalIntegration[id].Add(idProp, externalIntegrationAux[id][idProp]);
                                }
                            }
                        }
                    }

                    //Eliminamos de las probabilidades aquellos que ya estén reconciliados
                    foreach (string key in discoveredEntityList.Keys)
                    {
                        discoveredEntitiesProbability.Remove(key);
                    }
                }
            }
            DateTime discoverEndTime = DateTime.Now;
            DiscoverResult resultado = new DiscoverResult(dataGraph, dataInferenceGraph, ontologyGraph, discoveredEntitiesWithSubject, discoveredEntitiesWithIds, discoveredEntitiesWithBBDD, discoveredEntitiesWithExternalIntegration, discoveredEntitiesProbability, discoverInitTime, discoverEndTime, externalIntegration);

            return resultado;
        }


        /// <summary>
        /// Procesa los resultados del descubrimiento
        /// </summary>
        /// <param name="pDiscoverItem">Objeto con los datos de com procesar el proeso de descubrimiento</param>
        /// <param name="pDiscoverResult">Resultado de la aplicación del descubrimiento</param>
        /// <param name="pDiscoverItemBDService">Clase para gestionar las operaciones de las tareas de descubrimiento</param>
        /// <param name="pCallCronApiService">Servicio para hacer llamadas a los métodos del apiCron</param>
        /// <param name="pProcessDiscoverStateJobBDService">Clase para gestionar los estados de descubrimiento de las tareas</param>
        /// <returns></returns>
        public static void Process(DiscoverItem pDiscoverItem, DiscoverResult pDiscoverResult, DiscoverItemBDService pDiscoverItemBDService, CallCronApiService pCallCronApiService, ProcessDiscoverStateJobBDService pProcessDiscoverStateJobBDService)
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
                    //Hay dudas en la desambiguación, por lo que lo actualizamos en la BBDD con su estado correspondiente    
                    pDiscoverItem.UpdateDissambiguationProblems(
                        pDiscoverResult.discoveredEntitiesProbability,
                        pDiscoverResult.discoveredEntitiesWithDataBase.Values.Select(x => x.Key).Union(pDiscoverResult.discoveredEntitiesWithId.Values).Union(pDiscoverResult.discoveredEntitiesWithSubject).Union(pDiscoverResult.discoveredEntitiesWithExternalIntegration.Values.Select(x => x.Key)).ToList(),
                        pDiscoverResult.GetDataGraphRDF());
                    pDiscoverItemBDService.ModifyDiscoverItem(pDiscoverItem);
                }
                else
                {
                    AsioPublication asioPublication = new AsioPublication(mSparqlUtility, mSPARQLEndpoint, mQueryParam, mGraph);
                    //TODO usirsfactory
                    asioPublication.PublishRDF(pDiscoverResult.dataGraph, pDiscoverResult.dataInferenceGraph, pDiscoverResult.ontologyGraph,new KeyValuePair<string, string>("http://graph.um.es/res/agent/discover", "Algoritmos de descubrimiento"), pDiscoverResult.start, pDiscoverResult.end, pDiscoverResult.externalIntegration);
                    //Lo marcamos como procesado en la BBDD y eliminamos sus metadatos
                    pDiscoverItem.UpdateProcessed();
                    pDiscoverItemBDService.ModifyDiscoverItem(pDiscoverItem);
                }

                //Actualizamos el estado de descubrimiento de la tarea si el estado encolado esta en estado Success o Error (ha finalizado)
                string statusQueueJob = pCallCronApiService.GetJob(pDiscoverItem.JobID).State;
                if ((statusQueueJob == "Failed" || statusQueueJob == "Succeeded"))
                {
                    ProcessDiscoverStateJob processDiscoverStateJob = pProcessDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJob(pDiscoverItem.JobID);
                    string state;
                    //Actualizamos a error si existen items en estado error o con problemas de desambiguación 
                    if (pDiscoverItemBDService.ExistsDiscoverItemsErrorOrDissambiguatinProblems(pDiscoverItem.JobID))
                    {
                        state = "Error";
                    }
                    else if (pDiscoverItemBDService.ExistsDiscoverItemsPending(pDiscoverItem.JobID))
                    {
                        //Actualizamos a 'Pending' si aún existen items pendientes
                        state = "Pending";
                    }
                    else
                    {
                        //Actualizamos a Success si no existen items en estado error ni con problemas de desambiguación y no hay ninguno pendiente
                        state = "Success";
                    }
                    if (processDiscoverStateJob != null)
                    {
                        processDiscoverStateJob.State = state;
                        pProcessDiscoverStateJobBDService.ModifyProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                    else
                    {
                        processDiscoverStateJob = new ProcessDiscoverStateJob() { State = state, JobId = pDiscoverItem.JobID };
                        pProcessDiscoverStateJobBDService.AddProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                }
            }
            else
            {
                //Actualizamos en BBDD
                DiscoverItem discoverItemBD = pDiscoverItemBDService.GetDiscoverItemById(pDiscoverItem.ID);

                //Reporte de descubrimiento
                string discoverReport = "Time processed (seconds): " + pDiscoverResult.secondsProcessed + "\n";
                if (pDiscoverResult.discoveredEntitiesWithSubject != null && pDiscoverResult.discoveredEntitiesWithSubject.Count > 0)
                {
                    discoverReport += "Entities discover with the same uri: " + pDiscoverResult.discoveredEntitiesWithSubject.Count + "\n";
                    foreach (string uri in pDiscoverResult.discoveredEntitiesWithSubject)
                    {
                        discoverReport += "\t" + uri + "\n";
                    }
                }
                if (pDiscoverResult.discoveredEntitiesWithId != null && pDiscoverResult.discoveredEntitiesWithId.Count > 0)
                {
                    discoverReport += "Entities discover with some common identifier: " + pDiscoverResult.discoveredEntitiesWithId.Count + "\n";
                    foreach (string uri in pDiscoverResult.discoveredEntitiesWithId.Keys)
                    {
                        discoverReport += "\t" + uri + " --> " + pDiscoverResult.discoveredEntitiesWithId[uri] + "\n";
                    }
                }
                if (pDiscoverResult.discoveredEntitiesWithDataBase != null && pDiscoverResult.discoveredEntitiesWithDataBase.Count > 0)
                {
                    discoverReport += "Entities discover with reconciliation config: " + pDiscoverResult.discoveredEntitiesWithDataBase.Count + "\n";
                    foreach (string uri in pDiscoverResult.discoveredEntitiesWithDataBase.Keys)
                    {
                        discoverReport += "\t" + uri + " --> " + pDiscoverResult.discoveredEntitiesWithDataBase[uri] + "\n";
                    }
                }
                if (pDiscoverResult.discoveredEntitiesWithExternalIntegration != null && pDiscoverResult.discoveredEntitiesWithExternalIntegration.Count > 0)
                {
                    discoverReport += "Entities discover with external integrations config: " + pDiscoverResult.discoveredEntitiesWithExternalIntegration.Count + "\n";
                    foreach (string uri in pDiscoverResult.discoveredEntitiesWithExternalIntegration.Keys)
                    {
                        discoverReport += "\t" + uri + " --> " + pDiscoverResult.discoveredEntitiesWithExternalIntegration[uri] + "\n";
                    }
                }
                if (pDiscoverResult.externalIntegration != null && pDiscoverResult.externalIntegration.Count > 0)
                {
                    discoverReport += "Entities with identifiers obtained with External integration: " + pDiscoverResult.externalIntegration.Count + "\n";
                    foreach (string uri in pDiscoverResult.externalIntegration.Keys)
                    {
                        foreach (string property in pDiscoverResult.externalIntegration[uri].Keys)
                        {
                            discoverReport += "\t" + uri + " - " + property + " --> " + pDiscoverResult.externalIntegration[uri][property] + "\n";
                        }
                    }
                }
                discoverItemBD.UpdateReport(pDiscoverResult.discoveredEntitiesProbability, pDiscoverResult.GetDataGraphRDF(), discoverReport);
                pDiscoverItemBDService.ModifyDiscoverItem(discoverItemBD);
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
        /// <param name="pTriples">Lista de triples</param>
        /// <returns>Entidades del RDF con sus datos de desambiguación</returns>
        private static Dictionary<string, List<DisambiguationData>> GetDisambiguationDataRdf(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, List<Triple> pTriples)
        {
            Dictionary<string, Dictionary<string, HashSet<string>>> directRels = ExtractRelsFromTriples(pTriples, false);
            Dictionary<string, Dictionary<string, HashSet<string>>> inverseRels = ExtractRelsFromTriples(pTriples, true);

            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf = new Dictionary<string, List<DisambiguationData>>();

            Dictionary<string, List<Disambiguation>> disambiguationsByRdfType = new Dictionary<string, List<Disambiguation>>();
            Dictionary<Disambiguation, List<string>> dissambiguationProperties = new Dictionary<Disambiguation, List<string>>();
            Dictionary<Disambiguation, List<string>> dissambiguationInverseProperties = new Dictionary<Disambiguation, List<string>>();
            foreach (Disambiguation dissambiguation in mDisambiguationConfigs)
            {
                //Organizadas por rdftype
                if (!disambiguationsByRdfType.ContainsKey(dissambiguation.rdfType))
                {
                    disambiguationsByRdfType.Add(dissambiguation.rdfType, new List<Disambiguation>());
                }
                disambiguationsByRdfType[dissambiguation.rdfType].Add(dissambiguation);

                //Propiedades directas
                dissambiguationProperties.Add(dissambiguation, dissambiguation.properties.Where(x => !x.inverse).Select(x => x.property).ToList().Union(dissambiguation.identifiers).ToList());

                //Propiedades inversas
                dissambiguationInverseProperties.Add(dissambiguation, dissambiguation.properties.Where(x => x.inverse).Select(x => x.property).ToList());
            }


            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                foreach (string rdfType in pEntitiesRdfTypes[entityID])
                {
                    if (disambiguationsByRdfType.ContainsKey(rdfType))
                    {
                        //Obtenemos las configuraciones aplicables
                        List<Disambiguation> disambiguations = disambiguationsByRdfType[rdfType];
                        if (disambiguations.Count > 0)
                        {
                            foreach (Disambiguation disambiguation in disambiguations)
                            {
                                DisambiguationData disambiguationData = GetEntityDataForReconciliation(entityID, disambiguation, dissambiguationProperties[disambiguation], dissambiguationInverseProperties[disambiguation], directRels, inverseRels);

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
            }
            //Obtenemos los roh:identifier de todas las entidades           
            Dictionary<string, KeyValuePair<string, string>> disambiguationIdentifiersRdf = new Dictionary<string, KeyValuePair<string, string>>();
            Dictionary<string, Dictionary<string, HashSet<string>>> identifiersData = GetPropertiesValues(pEntitiesRdfTypes.Keys.ToList(), new List<string> { mPropertyRohIdentifier, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type" }, false, directRels);
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
        /// <param name="pProperties">Propiedades directas del objeto de desambigucación (incluidos los identificadores)</param>
        /// <param name="pInverseProperties">Propiedades inversas del objeto de desambigucación</param>
        /// <param name="pDirectRels">Relaciones directas</param>
        /// <param name="pInverseRels">Relaciones inversas</param>
        /// <returns>Datos para la desambiguación</returns>
        private static DisambiguationData GetEntityDataForReconciliation(string pSubject, Disambiguation pDisambiguation, List<string> pProperties, List<string> pInverseProperties, Dictionary<string, Dictionary<string, HashSet<string>>> pDirectRels, Dictionary<string, Dictionary<string, HashSet<string>>> pInverseRels)
        {
            Dictionary<string, DisambiguationData> data = GetEntityDataForReconciliation(new List<string> { pSubject }, pDisambiguation, pProperties, pInverseProperties, pDirectRels, pInverseRels);
            if (data != null && data.ContainsKey(pSubject))
            {
                return data[pSubject];
            }
            return null;
        }

        /// <summary>
        /// Obtiene datos de varias entidades cargadas en un grafo en local para la reconciliación
        /// </summary>
        /// <param name="pSubjects">Sujetos</param>
        /// <param name="pDisambiguation">Objeto de desambiguación</param>
        /// <param name="pProperties">Propiedades directas del objeto de desambigucación (incluidos los identificadores)</param>
        /// <param name="pInverseProperties">Propiedades inversas del objeto de desambigucación</param>
        /// <param name="pDirectRels">Relaciones directas</param>
        /// <param name="pInverseRels">Relaciones inversas</param>
        /// <returns>Datos para la desambiguación</returns>
        private static Dictionary<string, DisambiguationData> GetEntityDataForReconciliation(List<string> pSubjects, Disambiguation pDisambiguation, List<string> pProperties, List<string> pInverseProperties, Dictionary<string, Dictionary<string, HashSet<string>>> pDirectRels, Dictionary<string, Dictionary<string, HashSet<string>>> pInverseRels)
        {
            Dictionary<string, DisambiguationData> response = new Dictionary<string, DisambiguationData>();
            //Propiedades directas
            Dictionary<string, Dictionary<string, HashSet<string>>> propertiesData = GetPropertiesValues(pSubjects, pProperties, false, pDirectRels);

            //Propiedades inversas
            Dictionary<string, Dictionary<string, HashSet<string>>> inversePropertiesData = GetPropertiesValues(pSubjects, pInverseProperties, true, pInverseRels);

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
            //Si no tiene alguna propiedad obligatoria ni iedntificadores lo elminamos
            List<string> propsObligatorias = pDisambiguation.properties.Where(x => x.mandatory).ToList().Select(x => x.property).ToList();
            if (propsObligatorias.Count > 0)
            {
                foreach (string key in response.Keys.ToList())
                {
                    List<string> propsEntidad = response[key].properties.Select(x => x.property).Select(x => x.property).ToList();
                    if ((response[key].identifiers == null || response[key].identifiers.Count == 0) && propsObligatorias.Intersect(propsEntidad).Count() < propsObligatorias.Count)
                    {
                        response.Remove(key);
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Extrae las relaciones de los triples para procesarlos posteriormente por GetPropertiesValues
        /// </summary>
        /// <param name="pTriples">Lista de triples</param>
        /// <param name="pInverse">Implica si son inversas</param>
        /// <returns>Diccinoario con las relaciones</returns>
        private static Dictionary<string, Dictionary<string, HashSet<string>>> ExtractRelsFromTriples(List<Triple> pTriples, bool pInverse)
        {
            Dictionary<string, Dictionary<string, HashSet<string>>> rels = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            foreach (Triple triple in pTriples)
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
            return rels;
        }

        /// <summary>
        /// Obtiene N propiedades de N entidades de un grafo en local
        /// </summary>
        /// <param name="pSubjects">Sujetos</param>
        /// <param name="pProperties">Propiedades</param>
        /// <param name="pInverse">Implica si son inversas</param>
        /// <param name="pRels">Relaciones</param>
        /// <returns>Valor de las propiedades para los sujetos introducidos</returns>
        private static Dictionary<string, Dictionary<string, HashSet<string>>> GetPropertiesValues(List<string> pSubjects, List<string> pProperties, bool pInverse, Dictionary<string, Dictionary<string, HashSet<string>>> pRels)
        {
            Dictionary<string, Dictionary<string, HashSet<string>>> propertyValues = new Dictionary<string, Dictionary<string, HashSet<string>>>();


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
                        foreach (string subjectIn in inAux)
                        {
                            if (pRels.ContainsKey(subjectIn))
                            {
                                if (pRels[subjectIn].ContainsKey(propertyIn) || propertyIn == "?")
                                {
                                    if (propertyIn != "?")
                                    {
                                        outAux.UnionWith(pRels[subjectIn][propertyIn]);
                                    }
                                    else
                                    {
                                        outAux.UnionWith(pRels[subjectIn].Where(x => x.Key != "http://www.w3.org/1999/02/22-rdf-syntax-ns#type").SelectMany(x => x.Value).ToList());
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
        /// <param name="pDiscoverCache">Caché de discover</param>
        private static void LoadNamesScore(ref Dictionary<string, Dictionary<string, float>> pNamesScore, Dictionary<string, string> pPersonsWithName, RohGraph pDataGraph, DiscoverCache pDiscoverCache)
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
                    float similarity = GetNameSimilarity(nombre, pPersonsWithName[personBBDD], pDiscoverCache);
                    //Mapear la similitud de 0.5--1 hacia mMinScore -- 1;
                    if (similarity >= 0.5f)
                    {
                        similarity = mMinScore + ((1 - mMinScore) / (0.5f / (similarity - 0.5f)));
                        pNamesScore[nombre].Add(personBBDD, GetNameSimilarity(nombre, pPersonsWithName[personBBDD], pDiscoverCache));
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
                SparqlObject sparqlObject = mSparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
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
        /// <param name="pEntitiesRdfType">rdf:types de las entidades a las que se les va a buscar candidatos</param>
        /// <param name="pListaEntidadesOmitir">lista de entidades que hay que omitir al buscar candidatos</param>
        /// <param name="pDisambiguationDataRdf">datos de desambiguación del RDF</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns>Diccionario con los candidatos encontrados y sus datos para apoyar la desambiguación</returns>
        private static Dictionary<string, List<DisambiguationData>> GetDisambiguationDataBBDD(out Dictionary<string, string> pEntitiesRdfTypeBBDD, Dictionary<string, HashSet<string>> pEntitiesRdfType, HashSet<string> pListaEntidadesOmitir, Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, Dictionary<string, Dictionary<string, float>> pNamesScore, DiscoverCache pDiscoverCache)
        {
            Dictionary<string, List<DisambiguationData>> disambiguationDataBBDD = new Dictionary<string, List<DisambiguationData>>();
            pEntitiesRdfTypeBBDD = new Dictionary<string, string>();
            foreach (string entityID in pDisambiguationDataRdf.Keys)
            {
                if (!pListaEntidadesOmitir.Contains(entityID))
                {
                    foreach (DisambiguationData disambiguationData in pDisambiguationDataRdf[entityID])
                    {
                        Dictionary<string, string> rdfTypesReturn;
                        Dictionary<string, DisambiguationData> datosCandidatos = GetEntityDataForReconciliationBBDD(pEntitiesRdfType[entityID], out rdfTypesReturn, disambiguationData, pNamesScore, pDiscoverCache);
                        foreach (string key in datosCandidatos.Keys)
                        {
                            if (!disambiguationDataBBDD.ContainsKey(key))
                            {
                                disambiguationDataBBDD.Add(key, new List<DisambiguationData>());
                            }
                            disambiguationDataBBDD[key].Add(datosCandidatos[key]);
                            pEntitiesRdfTypeBBDD[key] = rdfTypesReturn[key];
                        }
                    }
                }
            }
            return disambiguationDataBBDD;
        }

        /// <summary>
        /// Obtiene de la BBDD los candidatos para la desambiguación junto con sus datos para unos datos de desambiguación concretos
        /// </summary>
        /// <param name="pRdfTypes">posibles rdf:types de los candidatos</param>
        /// <param name="rdfTypesReturn">rdf:type de los candidatos</param>
        /// <param name="pDisambiguationData">Datos de desambiguación</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static Dictionary<string, DisambiguationData> GetEntityDataForReconciliationBBDD(HashSet<string> pRdfTypes, out Dictionary<string, string> rdfTypesReturn, DisambiguationData pDisambiguationData, Dictionary<string, Dictionary<string, float>> pNamesScore, DiscoverCache pDiscoverCache)
        {
            rdfTypesReturn = new Dictionary<string, string>();
            Dictionary<string, DisambiguationData> returnDisambiguationData = new Dictionary<string, DisambiguationData>();
            if (pDisambiguationData != null && pDisambiguationData.properties != null && pDisambiguationData.properties.Count > 0)
            {
                string selectGlobal = "select distinct ?s ?rdfType ?prop ?value where\n{";

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
                string whereSujetos = $"\n\t\t\t?s a ?rdfType.";
                whereSujetos += $"\n\t\t\tFILTER(?rdfType in (<{string.Join(">,<", pRdfTypes)}>))";

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

                string selectSujetos = $"\n\t\tselect ?s ?rdfType ?scoreMandatory where\n\t\t{{";
                string orderSujetos = $"\n\t\t}}order by desc(?scoreMandatory) limit 10";
                if (varsNoMandatory.Count > 0)
                {
                    string scoresNoMandatory = "(sum(" + string.Join(")+sum(", varsNoMandatory) + "))";
                    selectSujetos = $"\n\t\tselect ?s ?rdfType ?scoreMandatory {scoresNoMandatory} as ?scoreNoMandatory where\n\t\t{{";
                    orderSujetos = $"\n\t\t}}order by desc(?scoreMandatory) desc {scoresNoMandatory} limit 10";
                }

                string consulta = selectSujetos + whereSujetos + orderSujetos;
                HashSet<string> listaItems = new HashSet<string>();
                Dictionary<string, string> listaItemsRdfType = new Dictionary<string, string>();
                if (whereSujetosMandatory.Count > 0 || whereSujetosNoMandatory.Count > 0)
                {
                    SparqlObject sparqlObject = SelectDataCache(consulta, pDiscoverCache);
                    foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                    {
                        string s = row["s"].value;
                        string rdftype = row["rdfType"].value;
                        listaItems.Add(s);
                        listaItemsRdfType[s] = rdftype;
                    }
                }
                if (listaItems.Count > 0)
                {
                    consulta = selectGlobal + whereProps + "FILTER(?s in (<" + string.Join(">,<", listaItems) + ">))" + "\n\t}";
                    SparqlObject sparqlObject = SelectDataCache(consulta, pDiscoverCache);
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

                        rdfTypesReturn[s] = listaItemsRdfType[s];
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
                SparqlObject sparqlObject = mSparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
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
        public static void PrepareData(RohGraph pDataGraph, RohRdfsReasoner pReasoner, out RohGraph pDataInferenceGraph, out Dictionary<string, HashSet<string>> pEntitiesRdfTypes, out Dictionary<string, string> pEntitiesRdfType, out Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, bool pIncludeBlankNodes = false)
        {
            //Cargamos datos del RDF con inferencias
            pDataInferenceGraph = pDataGraph.Clone();
            pReasoner.Apply(pDataInferenceGraph);
            pEntitiesRdfTypes = LoadEntitiesWithRdfTypes(pDataInferenceGraph, pIncludeBlankNodes);
            pEntitiesRdfType = LoadEntitiesWithRdfType(pDataGraph, pIncludeBlankNodes);
            pDisambiguationDataRdf = GetDisambiguationDataRdf(pEntitiesRdfTypes, pDataGraph.Triples.ToList());
        }

        /// <summary>
        /// Efectua la reconciliación con los identificadores
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pEntitiesRdfType">Diccionario con las entidades encontradas y su rdf:type (sin inferencia)</param>
        /// <param name="pDisambiguationDataRdf">Datos extraidos del grafo para la reconciliación</param>
        /// <param name="pDiscardDissambiguations">Descartes para la desambiguación</param>
        /// <param name="pDataGraph">Grafo en local con los datos a procesar</param>
        /// <returns>Lista con las entidades reconciliadas</returns>
        private static Dictionary<string, string> ReconciliateIDs(ref bool pHasChanges, ref Dictionary<string, string> pListaEntidadesReconciliadas, Dictionary<string, string> pEntitiesRdfType, Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, Dictionary<string, HashSet<string>> pDiscardDissambiguations, ref RohGraph pDataGraph)
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
                    SparqlObject sparqlObject = mSparqlUtility.SelectData(mSPARQLEndpoint, mGraph, consulta, mQueryParam);
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

                                if (coincidenciaBBDD.Key != null && coincidenciaBBDD.Value != null && coincidenciaBBDD.Key != entityID)
                                {
                                    if (pDiscardDissambiguations == null || !pDiscardDissambiguations.ContainsKey(entityID) || !pDiscardDissambiguations[entityID].Contains(coincidenciaBBDD.Key))
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
                                        pHasChanges = true;

                                    }
                                }
                            }
                        }
                    }
                }
            }
            return entidaesReconciliadas;
        }


        /// <summary>
        /// Reconcilia el RDF con la BBDD
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pListaEntidadesReconciliadasDudosas">Lista con las entidades dudosas</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pReasoner">Razonador</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <param name="pDiscardDissambiguations">Descartes de desambiguación</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <returns>Diccioario con las entidades reconciliadas</returns>
        private static Dictionary<string, KeyValuePair<string, float>> ReconciliateBBDD(ref bool pHasChanges, ref Dictionary<string, string> pListaEntidadesReconciliadas, out Dictionary<string, Dictionary<string, float>> pListaEntidadesReconciliadasDudosas, ref RohGraph pDataGraph, RohRdfsReasoner pReasoner, Dictionary<string, Dictionary<string, float>> pNamesScore, Dictionary<string, HashSet<string>> pDiscardDissambiguations, DiscoverCache pDiscoverCache)
        {
            Dictionary<string, KeyValuePair<string, float>> discoveredEntityList = new Dictionary<string, KeyValuePair<string, float>>();
            Dictionary<string, HashSet<string>> entitiesRdfTypes;
            Dictionary<string, string> entitiesRdfType;
            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;
            RohGraph dataInferenceGraph;
            pListaEntidadesReconciliadasDudosas = new Dictionary<string, Dictionary<string, float>>();
            PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf);
            bool hayQueReprocesar = true;
            while (hayQueReprocesar)
            {
                Dictionary<string, string> entitiesRdfTypeBBDD;
                Dictionary<string, List<DisambiguationData>> disambiguationDataBBDD = GetDisambiguationDataBBDD(out entitiesRdfTypeBBDD, entitiesRdfType.ToDictionary(x => x.Key, x => new HashSet<string>() { x.Value }), new HashSet<string>(pListaEntidadesReconciliadas.Keys.Union(pListaEntidadesReconciliadas.Values)), disambiguationDataRdf, pNamesScore, pDiscoverCache);

                hayQueReprocesar = false;
                Dictionary<string, KeyValuePair<string, float>> listaEntidadesReconciliadasAux = ReconciliateData(ref pHasChanges, ref pListaEntidadesReconciliadas, out pListaEntidadesReconciliadasDudosas, entitiesRdfType, disambiguationDataRdf, entitiesRdfTypeBBDD, disambiguationDataBBDD, ref pDataGraph, false, pDiscardDissambiguations, pDiscoverCache);
                if (listaEntidadesReconciliadasAux.Count > 0)
                {
                    hayQueReprocesar = true;
                    foreach (string id in listaEntidadesReconciliadasAux.Keys)
                    {
                        discoveredEntityList.Add(id, listaEntidadesReconciliadasAux[id]);
                    }

                }
                PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf);
            }
            return discoveredEntityList;
        }

        /// <summary>
        /// Reconcilia el RDF con datos del propio RDF
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pReasoner">Razonador</param>
        /// <param name="pDiscardDissambiguations">Descartes de desambiguación</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns>Diccioario con las entidades reconciliadas</returns>
        public static void ReconciliateRDF(ref bool pHasChanges, ref Dictionary<string, string> pListaEntidadesReconciliadas, ref RohGraph pDataGraph, RohRdfsReasoner pReasoner, Dictionary<string, HashSet<string>> pDiscardDissambiguations, DiscoverCache pDiscoverCache)
        {
            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;
            Dictionary<string, HashSet<string>> entitiesRdfTypes;
            Dictionary<string, string> entitiesRdfType;
            RohGraph dataInferenceGraph;
            PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf);
            bool hayQueReprocesar = true;
            while (hayQueReprocesar)
            {
                hayQueReprocesar = false;
                Dictionary<string, Dictionary<string, float>> listaEntidadesReconciliadasDudosas = new Dictionary<string, Dictionary<string, float>>();
                Dictionary<string, KeyValuePair<string, float>> listaEntidadesReconciliadasAux = ReconciliateData(ref pHasChanges, ref pListaEntidadesReconciliadas, out listaEntidadesReconciliadasDudosas, entitiesRdfType, disambiguationDataRdf, entitiesRdfType, disambiguationDataRdf, ref pDataGraph, false, pDiscardDissambiguations, pDiscoverCache);
                if (listaEntidadesReconciliadasAux.Count > 0)
                {
                    hayQueReprocesar = true;
                }
                PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf);
            }
            //TODO aplicar monovaluados ontología
        }


        /// <summary>
        /// Reconcilia el RDF con datos de los APIs externos
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pDiscoveredEntitiesProbability">Diccionario con los candidatos para las entidades</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pListaEntidadesRDFEnriquecer">Diccionario con las entidades en común del RDF y de los APIs externos</param>
        /// <param name="pExternalGraph">Grafo en local con los datos de los APIs externos</param>
        /// <param name="pReasoner">Razonador</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <param name="pClasesConSubclases">Diccionario con las clases de la ontología junto con sus subclases</param>
        /// <param name="pDiscardDissambiguations">Descartes para la desambiguación</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns>Diccioario con las entidades reconciliadas</returns>
        private static Dictionary<string, KeyValuePair<string, float>> ReconciliateExternalIntegration(ref bool pHasChanges, ref Dictionary<string, string> pListaEntidadesReconciliadas,
            ref Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability, ref RohGraph pDataGraph, Dictionary<string, string> pListaEntidadesRDFEnriquecer, RohGraph pExternalGraph,
            RohRdfsReasoner pReasoner, Dictionary<string, Dictionary<string, float>> pNamesScore, Dictionary<string, HashSet<string>> pClasesConSubclases, Dictionary<string, HashSet<string>> pDiscardDissambiguations, DiscoverCache pDiscoverCache)
        {
            Dictionary<string, KeyValuePair<string, float>> discoveredEntityList = new Dictionary<string, KeyValuePair<string, float>>();

            //Preparamos los datos del RDF
            Dictionary<string, HashSet<string>> entitiesRdfTypes;
            Dictionary<string, string> entitiesRdfType;
            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;
            RohGraph dataInferenceGraph;
            PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf);

            //Preparamos los datos del RDF externo
            Dictionary<string, HashSet<string>> externalEntitiesRdfTypes;
            Dictionary<string, string> externalEntitiesRdfType;
            Dictionary<string, List<DisambiguationData>> externalDisambiguationDataRdf;
            RohGraph externalDataInferenceGraph;
            PrepareData(pExternalGraph, pReasoner, out externalDataInferenceGraph, out externalEntitiesRdfTypes, out externalEntitiesRdfType, out externalDisambiguationDataRdf);

            EnriquecerDisambiguationDataRdfConExternalDisambiguationDataRdf(pListaEntidadesRDFEnriquecer, ref entitiesRdfType, ref disambiguationDataRdf, ref externalEntitiesRdfType, ref externalDisambiguationDataRdf);

            //Escogemos todas las sublcases de las entidades que vamos a intentar reconciliar
            Dictionary<string, HashSet<string>> entitiesRdfTypeSubclases = new Dictionary<string, HashSet<string>>();
            foreach (string entity in entitiesRdfType.Keys)
            {
                entitiesRdfTypeSubclases.Add(entity, new HashSet<string>() { entitiesRdfType[entity] });
                if (pClasesConSubclases.ContainsKey(entitiesRdfType[entity]))
                {
                    entitiesRdfTypeSubclases[entity].UnionWith(pClasesConSubclases[entitiesRdfType[entity]]);
                }
            }


            Dictionary<string, string> entitiesRdfTypeBBDD;
            Dictionary<string, List<DisambiguationData>> disambiguationDataBBDD = GetDisambiguationDataBBDD(out entitiesRdfTypeBBDD, entitiesRdfTypeSubclases, new HashSet<string>(pListaEntidadesReconciliadas.Keys.Union(pListaEntidadesReconciliadas.Values)), disambiguationDataRdf, pNamesScore, pDiscoverCache);


            Dictionary<string, Dictionary<string, float>> candidatos;
            ReconciliateData(ref pHasChanges, ref pListaEntidadesReconciliadas, out candidatos, entitiesRdfType, disambiguationDataRdf, entitiesRdfTypeBBDD, disambiguationDataBBDD, ref pDataGraph, true, pDiscardDissambiguations, pDiscoverCache);

            foreach (string entity in candidatos.Keys)
            {
                if (!pListaEntidadesReconciliadas.ContainsKey(entity) && entitiesRdfTypes.ContainsKey(entity))
                {
                    List<string> canditosSeguros = candidatos[entity].Where(x => x.Value == 1).ToList().Select(x => x.Key).Except(new List<string>() { entity }).ToList();
                    //Candidatos que superan el umbral máximo (excluyendo los anteriores)
                    List<string> canditosUmbralMaximo = candidatos[entity].Where(x => x.Value >= mMaxScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).Except(new List<string>() { entity }).ToList();
                    //Candidatos que superan el umbral mínimo (excluyendo los anteriores)
                    List<string> canditosUmbralMinimo = candidatos[entity].Where(x => x.Value >= mMinScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).Except(canditosUmbralMaximo).Except(new List<string>() { entity }).ToList();


                    if (canditosSeguros.Count == 1 || canditosUmbralMaximo.Count == 1)
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

                        if (pDiscardDissambiguations == null || !pDiscardDissambiguations.ContainsKey(entity) || !pDiscardDissambiguations[entity].Contains(urlReconciliada))
                        {
                            TripleStore store = new TripleStore();
                            store.Add(pDataGraph);

                            //Cambiamos candidato.Key por entityID
                            SparqlUpdateParser parser = new SparqlUpdateParser();
                            //Actualizamos los sujetos
                            SparqlUpdateCommandSet updateSubject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                    INSERT{<" + urlReconciliada + @"> ?p ?o.}
                                                                    WHERE 
                                                                    {
                                                                        ?s ?p ?o.   FILTER(?s = <" + entity + @">)
                                                                    }");
                            //Actualizamos los objetos
                            SparqlUpdateCommandSet updateObject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                    INSERT{?s ?p <" + urlReconciliada + @">.}
                                                                    WHERE 
                                                                    {
                                                                        ?s ?p ?o.   FILTER(?o = <" + entity + @">)
                                                                    }");
                            LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                            processor.ProcessCommandSet(updateSubject);
                            processor.ProcessCommandSet(updateObject);

                            pListaEntidadesReconciliadas.Add(entity, urlReconciliada);
                            float score = 0;
                            if (canditosSeguros.Count == 1)
                            {
                                score = 1;
                            }
                            else
                            {
                                score = candidatos[entity][canditosUmbralMaximo[0]];
                            }
                            discoveredEntityList.Add(entity, new KeyValuePair<string, float>(urlReconciliada, score));
                            pHasChanges = true;
                            if (pListaEntidadesRDFEnriquecer.ContainsKey(entity))
                            {
                                pListaEntidadesRDFEnriquecer.Add(urlReconciliada, pListaEntidadesRDFEnriquecer[entity]);
                                pListaEntidadesRDFEnriquecer.Remove(entity);
                            }
                        }
                    }
                    else if (canditosUmbralMaximo.Count > 1 || canditosUmbralMinimo.Count > 0)
                    {
                        //Si para alguna entidad hay más de un candidato que supere el umbral máximo 
                        //o hay alguna entidad que supere el umbral mínimo pero no alcance el máximo
                        //Lo marcamos para que lo decida el usuario
                        pDiscoveredEntitiesProbability[entity] = candidatos[entity];
                    }
                }
            }

            return discoveredEntityList;
        }

        /// <summary>
        /// Enriquece los datos de desambiguación del RDF con los datos extraídos de las integraciones externas
        /// </summary>
        /// <param name="pListaEntidadesRDFEnriquecer">Diccionario con las entidades en común del RDF y de los APIs externos</param>
        /// <param name="pEntitiesRdfType">Dicionario con las entidades del RDF y sus clases</param>
        /// <param name="pDisambiguationDataRdf">Datos de desambiguación del RDF</param>
        /// <param name="pExternalEntitiesRdfType">Dicionario con las integraciones externas y sus clases</param>
        /// <param name="pExternalDisambiguationDataRdf">Datos de desambiguación de las integraciones externas</param>
        private static void EnriquecerDisambiguationDataRdfConExternalDisambiguationDataRdf(Dictionary<string, string> pListaEntidadesRDFEnriquecer, ref Dictionary<string, string> pEntitiesRdfType,
            ref Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, ref Dictionary<string, string> pExternalEntitiesRdfType,
            ref Dictionary<string, List<DisambiguationData>> pExternalDisambiguationDataRdf)
        {
            //1º Cambiamos los datos de externalDisambiguationDataRdf
            foreach (string entityRDF in pListaEntidadesRDFEnriquecer.Keys)
            {
                string entityExternal = pListaEntidadesRDFEnriquecer[entityRDF];
                foreach (string idExternal in pExternalDisambiguationDataRdf.Keys.ToList())
                {
                    List<DisambiguationData> dissambiguationdatas = pExternalDisambiguationDataRdf[idExternal];
                    if (idExternal == entityExternal)
                    {
                        pExternalDisambiguationDataRdf[entityRDF] = pExternalDisambiguationDataRdf[idExternal];
                        pExternalDisambiguationDataRdf.Remove(idExternal);
                    }
                    foreach (DisambiguationData dissambiguationdata in dissambiguationdatas.ToList())
                    {
                        foreach (DisambiguationData.DataProperty dataProperty in dissambiguationdata.properties.ToList())
                        {
                            if (dataProperty.values.Contains(entityExternal))
                            {
                                dataProperty.values.Remove(entityExternal);
                                dataProperty.values.Add(entityRDF);
                            }
                        }
                    }
                }
            }

            //2º Añadimos a disambiguationDataRdf todos los datos relevantes
            bool cambios = true;
            while (cambios)
            {
                cambios = false;
                HashSet<string> entitiesRDF = new HashSet<string>();
                foreach (string id in pDisambiguationDataRdf.Keys)
                {
                    entitiesRDF.Add(id);
                    foreach (DisambiguationData dissambiguationdata in pDisambiguationDataRdf[id])
                    {
                        foreach (DisambiguationData.DataProperty dataProperty in dissambiguationdata.properties.ToList())
                        {
                            entitiesRDF.UnionWith(dataProperty.values);
                        }
                    }
                }

                foreach (string idExternal in pExternalDisambiguationDataRdf.Keys)
                {
                    List<DisambiguationData> dissambiguationdatas = pExternalDisambiguationDataRdf[idExternal];
                    if (entitiesRDF.Contains(idExternal))
                    {
                        if (!pDisambiguationDataRdf.ContainsKey(idExternal))
                        {
                            pDisambiguationDataRdf.Add(idExternal, dissambiguationdatas);
                            pEntitiesRdfType[idExternal] = pExternalEntitiesRdfType[idExternal];
                            cambios = true;
                        }
                        else
                        {
                            foreach (DisambiguationData disambiguationData in dissambiguationdatas)
                            {
                                DisambiguationData DisambiguationDataRdf = pDisambiguationDataRdf[idExternal].FirstOrDefault(x => x.disambiguation.rdfType == disambiguationData.disambiguation.rdfType);
                                if (DisambiguationDataRdf == null)
                                {
                                    pDisambiguationDataRdf[idExternal].Add(disambiguationData);
                                    cambios = true;
                                }
                                else
                                {
                                    foreach (DisambiguationData.DataProperty dataProperty in disambiguationData.properties)
                                    {
                                        DisambiguationData.DataProperty dataPropertyRdf = DisambiguationDataRdf.properties.FirstOrDefault(x => x.property == dataProperty.property);
                                        if (dataPropertyRdf == null)
                                        {
                                            DisambiguationDataRdf.properties.Add(dataProperty);
                                            cambios = true;
                                        }
                                        else
                                        {
                                            int numAntes = dataPropertyRdf.values.Count;
                                            dataPropertyRdf.values.UnionWith(dataProperty.values);
                                            int numDespues = dataPropertyRdf.values.Count;
                                            if (numAntes != numDespues)
                                            {
                                                cambios = true;
                                            }
                                        }
                                    }

                                    foreach (string propidentifier in disambiguationData.identifiers.Keys)
                                    {
                                        if (!DisambiguationDataRdf.identifiers.ContainsKey(propidentifier))
                                        {
                                            DisambiguationDataRdf.identifiers.Add(propidentifier, disambiguationData.identifiers[propidentifier]);
                                            cambios = true;
                                        }
                                        else
                                        {
                                            int numAntes = DisambiguationDataRdf.identifiers[propidentifier].Count;
                                            DisambiguationDataRdf.identifiers[propidentifier].UnionWith(disambiguationData.identifiers[propidentifier]);
                                            int numDespues = DisambiguationDataRdf.identifiers[propidentifier].Count;
                                            if (numAntes != numDespues)
                                            {
                                                cambios = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Efectua la reconciliación con los datos proporcionados
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pListaEntidadesReconciliadasDudosas"></param>
        /// <param name="pEntitiesRdfType">Diccionario con las entidades encontradas y sus rdf:type</param>
        /// <param name="pEntitiesRdfTypeCandidate">Diccionario con los candidatos y sus rdf:type</param>
        /// <param name="pDisambiguationDataRdf">Datos extraidos del grafo en local para la reconciliación</param>
        /// <param name="pDisambiguationDataCandidate">Datos de los candidatos para actualizar el rdf en local</param>
        /// <param name="pDataGraph">Grafo en local con los datos a procesar</param>
        /// <param name="pExternalIntegration">Indica si es para una integración externa, en ese caso no se hace efectiva la reconciliación y no se aplica la coincidencia de rdftypes incluye la herencia</param>
        /// <param name="pDiscardDissambiguations">Descartes de desambiguación</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns>Diccionario de entidades reconciliadas</returns>
        private static Dictionary<string, KeyValuePair<string, float>> ReconciliateData(ref bool pHasChanges, ref Dictionary<string, string> pListaEntidadesReconciliadas, out Dictionary<string, Dictionary<string, float>> pListaEntidadesReconciliadasDudosas, Dictionary<string, string> pEntitiesRdfType, Dictionary<string, List<DisambiguationData>> pDisambiguationDataRdf, Dictionary<string, string> pEntitiesRdfTypeCandidate, Dictionary<string, List<DisambiguationData>> pDisambiguationDataCandidate, ref RohGraph pDataGraph, bool pExternalIntegration, Dictionary<string, HashSet<string>> pDiscardDissambiguations, DiscoverCache pDiscoverCache)
        {
            Dictionary<string, KeyValuePair<string, float>> discoveredEntityList = new Dictionary<string, KeyValuePair<string, float>>();
            pListaEntidadesReconciliadasDudosas = new Dictionary<string, Dictionary<string, float>>();

            //Clave ID rdf, valor ID BBDD,score
            Dictionary<string, Dictionary<string, float>> candidatos = new Dictionary<string, Dictionary<string, float>>();

            Dictionary<Disambiguation, List<Disambiguation.Property>> dicPropiedadesObligatoriasDissambiguation = new Dictionary<Disambiguation, List<Disambiguation.Property>>();
            Dictionary<DisambiguationData, List<Disambiguation.Property>> dicPropiedadesDisambiguationData = new Dictionary<DisambiguationData, List<Disambiguation.Property>>();
            foreach (Disambiguation disambiguation in mDisambiguationConfigs)
            {
                dicPropiedadesObligatoriasDissambiguation[disambiguation] = disambiguation.properties.Where(x => x.mandatory).ToList();
            }
            foreach (DisambiguationData disambiguationData in pDisambiguationDataRdf.Values.SelectMany(x => x))
            {
                dicPropiedadesDisambiguationData[disambiguationData] = disambiguationData.properties.Select(x => x.property).ToList();
            }
            foreach (DisambiguationData disambiguationData in pDisambiguationDataCandidate.Values.SelectMany(x => x))
            {
                dicPropiedadesDisambiguationData[disambiguationData] = disambiguationData.properties.Select(x => x.property).ToList();
            }

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
                    //Si no ha sido ya reconciliada o es una integración externa y contiene datos para su desambiguación procedemos
                    if ((!pListaEntidadesReconciliadas.ContainsKey(entityID_RDF) || pExternalIntegration) && pDisambiguationDataRdf.ContainsKey(entityID_RDF))
                    {
                        foreach (DisambiguationData disambiguationData in pDisambiguationDataRdf[entityID_RDF])
                        {
                            //Recorremos los datos de desambiguación del resto de elementos con el mismo rdf:type
                            foreach (KeyValuePair<string, List<DisambiguationData>> candidato in pDisambiguationDataCandidate.Where(x => (pExternalIntegration) || (!pExternalIntegration && pEntitiesRdfType[entityID_RDF] == pEntitiesRdfTypeCandidate[x.Key] && entityID_RDF != x.Key)).ToList())
                            {
                                //y la misma configuración de desambiguación
                                DisambiguationData disambiguationDataCandidato = candidato.Value.FirstOrDefault(x => x.disambiguation == disambiguationData.disambiguation);
                                if (disambiguationDataCandidato != null)
                                {
                                    bool mismaEntidad = false;
                                    bool puedeSerMismaEntidad = true;

                                    if (pDiscardDissambiguations != null)
                                    {
                                        //No intentamos la desambiguación con los descartes
                                        if (pDiscardDissambiguations.ContainsKey(entityID_RDF) && pDiscardDissambiguations[entityID_RDF].Contains(candidato.Key))
                                        {
                                            continue;
                                        }
                                        if (pDiscardDissambiguations.ContainsKey(candidato.Key) && pDiscardDissambiguations[candidato.Key].Contains(entityID_RDF))
                                        {
                                            continue;
                                        }
                                    }

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
                                        float similarity;
                                        if (mismaEntidad)
                                        {
                                            similarity = 1;
                                        }
                                        else
                                        {
                                            similarity = GetSimilarity(disambiguationData, disambiguationDataCandidato, dicPropiedadesObligatoriasDissambiguation, dicPropiedadesDisambiguationData, candidatos, GetOnlyPositiveScore, pDiscoverCache);
                                        }
                                        if (similarity >= mMinScore)
                                        {
                                            //No hay que almacenar los candidatos 'inversos' (se guardaría duplicado)
                                            if (candidatosAux.ContainsKey(candidato.Key) && candidatosAux[candidato.Key].ContainsKey(entityID_RDF))
                                            {
                                                continue;
                                            }
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
                List<string> canditosSeguros = candidatos[entityRDF].Where(x => x.Value == 1).ToList().Select(x => x.Key).Except(new List<string>() { entityRDF }).ToList();
                //Candidatos que superan el umbral máximo (excluyendo los anteriores)
                List<string> canditosUmbralMaximo = candidatos[entityRDF].Where(x => x.Value >= mMaxScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).Except(new List<string>() { entityRDF }).ToList();
                //Candidatos que superan el umbral mínimo (excluyendo los anteriores)
                List<string> canditosUmbralMinimo = candidatos[entityRDF].Where(x => x.Value >= mMinScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).Except(canditosUmbralMaximo).Except(new List<string>() { entityRDF }).ToList();


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
                    if (pDiscardDissambiguations == null || !pDiscardDissambiguations.ContainsKey(entityRDF) || !pDiscardDissambiguations[entityRDF].Contains(urlReconciliada))
                    {
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
                        pHasChanges = true;
                    }
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
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static float GetSimilarity(DisambiguationData pDisambiguationDataOriginal, DisambiguationData pDisambiguationDataCandidate,
            Dictionary<Disambiguation, List<Disambiguation.Property>> pDicPropiedadesObligatoriasDissambiguation, Dictionary<DisambiguationData, List<Disambiguation.Property>> pDicPropiedadesDisambiguationData, Dictionary<string, Dictionary<string, float>> pCandidates, bool pOnlyPositive, DiscoverCache pDiscoverCache)
        {
            List<Disambiguation.Property> propieadesObligatorias = pDicPropiedadesObligatoriasDissambiguation[pDisambiguationDataOriginal.disambiguation];
            List<Disambiguation.Property> propieadesComunes = pDicPropiedadesDisambiguationData[pDisambiguationDataOriginal].Intersect(pDicPropiedadesDisambiguationData[pDisambiguationDataCandidate]).OrderByDescending(x => x.mandatory).ToList();

            //Tiene alguna propiedad en común y tiene al menos todas las obligatorias
            if (propieadesComunes.Count > 0 && propieadesObligatorias.Intersect(propieadesComunes).Count() == propieadesObligatorias.Count)
            {
                float similitudGlobal = 0;
                List<float> similitudesNegativas = new List<float>();
                foreach (Disambiguation.Property propiedad in propieadesComunes)
                {
                    float similitudActual = 0;
                    HashSet<string> valorPropiedadesOriginal = pDisambiguationDataOriginal.properties.First(x => x.property == propiedad).values;
                    HashSet<string> valorPropiedadesCandidato = pDisambiguationDataCandidate.properties.First(x => x.property == propiedad).values;

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
                                            float aux = GetSimilarity(original, candidato, propiedad.type, pDiscoverCache);
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
                                                        float aux2 = GetSimilarity(originalCandidato, candidato, propiedad.type, pDiscoverCache) * pCandidates[original][originalCandidato];
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
                                                float aux = GetSimilarity(original, candidato, propiedad.type, pDiscoverCache);
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
                                            float aux = GetSimilarity(propOriginal, propCandidato, propiedad.type, pDiscoverCache);
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
                                            float aux = GetSimilarity(propOriginal, propCandidato, propiedad.type, pDiscoverCache, propiedad.maxNumWordsTitle);
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
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static float GetSimilarity(string pOriginal, string pCandidato, Disambiguation.Property.Type pType, DiscoverCache pDiscoverCache, int? pMaxNumWordsTitle = null)
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
                        if (originalAux.StartsWith("\"") && originalAux.Contains("\"^^"))
                        {
                            originalAux = originalAux.Substring(1, originalAux.LastIndexOf("\"^^") - 1).Trim(new char[] { ' ', '-' });
                        }
                        string candidatoAux = pCandidato;
                        if (candidatoAux.StartsWith("\"") && candidatoAux.Contains("\"^^"))
                        {
                            candidatoAux = candidatoAux.Substring(1, candidatoAux.LastIndexOf("\"^^") - 1).Trim(new char[] { ' ', '-' });
                        }
                        similarity = GetNameSimilarity(originalAux, candidatoAux, pDiscoverCache);
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

                        string originalAux = pOriginal.ToLower();
                        if (originalAux.StartsWith("\"") && originalAux.Contains("\"^^"))
                        {
                            originalAux = originalAux.Substring(1, originalAux.LastIndexOf("\"^^") - 1);
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
                        string candidatoAux = pCandidato.ToLower();
                        if (candidatoAux.StartsWith("\"") && candidatoAux.Contains("\"^^"))
                        {
                            candidatoAux = candidatoAux.Substring(1, candidatoAux.LastIndexOf("\"^^") - 1);
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
        /// Método para el descubrimiento con los APIs externos
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pDiscoveredEntitiesProbability">Lista con las entidades dudosas</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pReasoner">Razonador</param>
        /// <param name="pNamesScore">Diccionario con los nombres del RDF y las entidades de la BB con sus scores</param>
        /// <param name="pOntologyGraph">Grafo en local con los datos de la ontología</param>
        /// <param name="pEntidadesReconciliadasConIntegracionExterna">Entidades reconciliadas con la integración externa</param>
        /// <param name="pDiscardDissambiguations">Descartes para la desambiguación</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <returns>Diccionario con las entidades y los identificadores extraídos, junto con su provenencia</returns>
        public static Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> ExternalIntegration(ref bool pHasChanges,
            ref Dictionary<string, string> pListaEntidadesReconciliadas, ref Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability, ref RohGraph pDataGraph, RohRdfsReasoner pReasoner,
            Dictionary<string, Dictionary<string, float>> pNamesScore, RohGraph pOntologyGraph, out Dictionary<string, KeyValuePair<string, float>> pEntidadesReconciliadasConIntegracionExterna, Dictionary<string, HashSet<string>> pDiscardDissambiguations, DiscoverCache pDiscoverCache)
        {
            RohGraph dataInferenceGraph;
            Dictionary<string, HashSet<string>> entitiesRdfTypes;
            Dictionary<string, string> entitiesRdfType;
            Dictionary<string, List<DisambiguationData>> disambiguationDataRdf;
            PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out entitiesRdfTypes, out entitiesRdfType, out disambiguationDataRdf, false);


            //Identificadores descubiertos con las integraciones externas
            Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> identifiersDiscover;
            
            RohGraph dataGraphClone = pDataGraph.Clone();
            Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbabilityClone = new Dictionary<string, Dictionary<string, float>>(pDiscoveredEntitiesProbability);


            HashSet<RohGraph> externalGraphs = new HashSet<RohGraph>();
            HashSet<RohGraph> provenanceGraphs = new HashSet<RohGraph>();
            List<Thread> hilosIntegracionesExternas = new List<Thread>();

            Exception exception = null;
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph,RohGraph> data=ExternalIntegrationORCID(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key);provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph, RohGraph> data = ExternalIntegrationSCOPUS(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key); provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph, RohGraph> data = ExternalIntegrationDBLP(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key); provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            //De momento lo omitimos, es muy lento y da timeout casi siempre
            //hilosIntegracionesExternas.Add(new Thread(() => {try{  KeyValuePair<RohGraph,RohGraph> data=ExternalIntegrationCROSSREF(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key);provenanceGraphs.Add(data.Value);} catch (Exception ex) { exception = ex; }}));
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph, RohGraph> data = ExternalIntegrationPUBMED(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key); provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph, RohGraph> data = ExternalIntegrationWOS(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key); provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph, RohGraph> data = ExternalIntegrationRECOLECTA(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key); provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            hilosIntegracionesExternas.Add(new Thread(() => { try { KeyValuePair<RohGraph, RohGraph> data = ExternalIntegrationDOAJ(entitiesRdfTypes, dataGraphClone, pDiscoverCache, discoveredEntitiesProbabilityClone); externalGraphs.Add(data.Key); provenanceGraphs.Add(data.Value); } catch (Exception ex) { exception = ex; } }));
            foreach (Thread thread in hilosIntegracionesExternas)
            {
                thread.Start();
            }
            foreach (Thread thread in hilosIntegracionesExternas)
            {
                thread.Join();
                if (exception != null)
                {
                    throw exception;
                }
            }

            RohGraph externalGraph = new RohGraph();
            foreach (RohGraph graph in externalGraphs)
            {
                if (graph != null)
                {
                    bool externalHasChangesAUX = true;
                    Dictionary<string, string> externalListaEntidadesReconciliadasAUX = new Dictionary<string, string>();
                    externalGraph.Merge(graph);
                    ReconciliateRDF(ref externalHasChangesAUX, ref externalListaEntidadesReconciliadasAUX, ref externalGraph, pReasoner, pDiscardDissambiguations, pDiscoverCache);
                }
            }

            foreach (RohGraph graph in provenanceGraphs)
            {
                if (graph != null)
                {
                    pDataGraph.Merge(graph);
                }
            }

            //Agregamos al RDF los identificadores encontrados en las fuentes externas y almacenamos en 'listaEntidadesRDFEnriquecer' aquellas entidades del RDF 
            //para las que hemos obtenido información adicional con las integraciones externas
            Dictionary<string, string> listaEntidadesRDFEnriquecer;
            identifiersDiscover = ExternalIntegrationExtractIdentifiers(ref pHasChanges, externalGraph, ref pDataGraph, pListaEntidadesReconciliadas, pReasoner, out listaEntidadesRDFEnriquecer, pDiscardDissambiguations, pDiscoverCache);

            pEntidadesReconciliadasConIntegracionExterna = new Dictionary<string, KeyValuePair<string, float>>();
            if (listaEntidadesRDFEnriquecer.Count > 0)
            {
                //Cargamos todas las subclases que hay dentro de cada clase
                Dictionary<string, HashSet<string>> clasesConSubclases = ExtractClassWithSubclass(pOntologyGraph);

                //Aplicamos la reconciliación con el RDF 'enriquecido'
                pEntidadesReconciliadasConIntegracionExterna = ReconciliateExternalIntegration(ref pHasChanges, ref pListaEntidadesReconciliadas, ref pDiscoveredEntitiesProbability, ref pDataGraph, listaEntidadesRDFEnriquecer, externalGraph, pReasoner, pNamesScore, clasesConSubclases, pDiscardDissambiguations, pDiscoverCache);
            }
            foreach (string entityID in pDiscoveredEntitiesProbability.Keys.ToList())
            {
                if (pListaEntidadesReconciliadas.ContainsKey(entityID))
                {
                    pDiscoveredEntitiesProbability.Remove(entityID);
                }
            }
            return identifiersDiscover;
        }

        /// <summary>
        /// Extrae los identificadores obtenidos de las integraciones externas y los almacena en el Grafo a cargar
        /// </summary>
        /// <param name="pHasChanges">Indica si se han realizado cambios en pDataGraph</param>
        /// <param name="pExternalGraph">Grafo con los datos de las integraciones externas</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pListaEntidadesReconciliadas">Lista con las entidades reconciliadas</param>
        /// <param name="pReasoner">Razonador</param>
        /// <param name="pListaEntidadesRDFEnriquecer">Diccionario con las entidades del RDF a enriquecer con los datos de las integraciones externas</param>
        /// <param name="pDiscardDissambiguations">Descartes de desambiguación</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns>Diccionario con las entidades y los identificadores extraídos y sus provenencias</returns>
        private static Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> ExternalIntegrationExtractIdentifiers(ref bool pHasChanges, RohGraph pExternalGraph, ref RohGraph pDataGraph,
            Dictionary<string, string> pListaEntidadesReconciliadas, RohRdfsReasoner pReasoner, out Dictionary<string, string> pListaEntidadesRDFEnriquecer, Dictionary<string, HashSet<string>> pDiscardDissambiguations, DiscoverCache pDiscoverCache)
        {
            Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> identifiersDiscover = new Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>>();

            //Preparamos los datos del RDF
            RohGraph externalInferenceGraph = new RohGraph();
            Dictionary<string, HashSet<string>> externalEntitiesRdfTypes = new Dictionary<string, HashSet<string>>();
            Dictionary<string, string> externalEntitiesRdfType = new Dictionary<string, string>();
            Dictionary<string, List<DisambiguationData>> externalDisambiguationDataRdf = new Dictionary<string, List<DisambiguationData>>();
            RohGraph dataInferenceGraph = new RohGraph();
            Dictionary<string, HashSet<string>> dataEntitiesRdfTypes = new Dictionary<string, HashSet<string>>();
            Dictionary<string, string> dataEntitiesRdfType = new Dictionary<string, string>();
            Dictionary<string, List<DisambiguationData>> dataDisambiguationDataRdf = new Dictionary<string, List<DisambiguationData>>();
            PrepareData(pDataGraph, pReasoner, out dataInferenceGraph, out dataEntitiesRdfTypes, out dataEntitiesRdfType, out dataDisambiguationDataRdf, false);
            //Preparamos los datos del RDF con los datos externos
            PrepareData(pExternalGraph, pReasoner, out externalInferenceGraph, out externalEntitiesRdfTypes, out externalEntitiesRdfType, out externalDisambiguationDataRdf, false);

            //Obtenemos las equivalencias entre los datos externos y el RDF y los almacenamos en
            bool hasChangesAux = false;
            Dictionary<string, Dictionary<string, float>> listaEntidadesDetectadas;
            ReconciliateData(ref hasChangesAux, ref pListaEntidadesReconciliadas, out listaEntidadesDetectadas, dataEntitiesRdfType, dataDisambiguationDataRdf, externalEntitiesRdfType, externalDisambiguationDataRdf, ref pExternalGraph, true, pDiscardDissambiguations, pDiscoverCache);


            pListaEntidadesRDFEnriquecer = new Dictionary<string, string>();

            //Añadimos los identificadores a las entidades que corrspondan
            foreach (string entityRDF in listaEntidadesDetectadas.Keys)
            {
                //Candidatos con un 1 de probabilidad
                List<string> canditosSeguros = listaEntidadesDetectadas[entityRDF].Where(x => x.Value == 1).ToList().Select(x => x.Key).ToList();
                //Candidatos que superan el umbral máximo (excluyendo los anteriores)
                List<string> canditosUmbralMaximo = listaEntidadesDetectadas[entityRDF].Where(x => x.Value >= mMaxScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).ToList();
                //Candidatos que superan el umbral mínimo (excluyendo los anteriores)
                List<string> canditosUmbralMinimo = listaEntidadesDetectadas[entityRDF].Where(x => x.Value >= mMinScore).ToList().Select(x => x.Key).ToList().Except(canditosSeguros).Except(canditosUmbralMaximo).ToList();


                if (canditosSeguros.Count == 1 || canditosUmbralMaximo.Count == 1)
                {
                    //Si sólo hay un candidato seguro
                    //O sólo un candidato supera el umbral máximo realizamos la reconciliación
                    string urlEntidadExterna = null;
                    if (canditosSeguros.Count == 1)
                    {
                        urlEntidadExterna = canditosSeguros[0];
                    }
                    else
                    {
                        urlEntidadExterna = canditosUmbralMaximo[0];
                    }

                    //Obtenemos los identificadores de la entidad encontrada
                    foreach (Disambiguation disambiguation in mDisambiguationConfigs)
                    {
                        if (disambiguation.identifiers != null && disambiguation.identifiers.Count > 0)
                        {
                            Dictionary<string, string> identifiersVars = new Dictionary<string, string>();
                            foreach (string identifier in disambiguation.identifiers)
                            {
                                identifiersVars.Add("?prop" + identifiersVars.Count, identifier);
                            }
                            if (externalEntitiesRdfTypes[urlEntidadExterna].Contains(disambiguation.rdfType))
                            {
                                string consulta = @$"select ?s {string.Join(" ", identifiersVars.Keys)}
                                                            where
                                                            {{
                                                                    ?s a ?rdfType. ";
                                foreach (string identifier in identifiersVars.Keys)
                                {
                                    consulta += @$"OPTIONAL{{?s <{identifiersVars[identifier]}> {identifier}}}";
                                }
                                consulta += @$"  Filter(?s = <{urlEntidadExterna}>)
                                                            }}";

                                SparqlResultSet sparqlResultSet = (SparqlResultSet)pExternalGraph.ExecuteQuery(consulta);
                                foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                                {
                                    foreach (string identifier in identifiersVars.Keys)
                                    {
                                        if (sparqlResult.Variables.Contains(identifier.Replace("?", "")) && sparqlResult[identifier.Replace("?", "")] is LiteralNode)
                                        {
                                            int numtriplesAntes = pDataGraph.Triples.Count;
                                            IUriNode t_subject = pDataGraph.CreateUriNode(UriFactory.Create(entityRDF));
                                            IUriNode t_predicate = pDataGraph.CreateUriNode(UriFactory.Create(identifiersVars[identifier]));
                                            ILiteralNode t_object = pDataGraph.CreateLiteralNode(((LiteralNode)sparqlResult[identifier.Replace("?", "")]).Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            pDataGraph.Assert(new Triple(t_subject, t_predicate, t_object));
                                            int numtriplesDespues = pDataGraph.Triples.Count;
                                            if (!identifiersDiscover.ContainsKey(entityRDF))
                                            {
                                                identifiersDiscover[entityRDF] = new Dictionary<string, KeyValuePair<string, HashSet<string>>>();
                                            }
                                            //Obtenemos la provenencia en caso de que esté disponible
                                            SparqlResultSet resultSetProvenance = (SparqlResultSet)pExternalGraph.ExecuteQuery(
                                                $@" select distinct ?provenance where 
                                                    {{
                                                        <{urlEntidadExterna}> <http://purl.org/roh#externalID> ?externalID.   
                                                        ?externalID <{identifiersVars[identifier]}> ""{ ((LiteralNode)sparqlResult[identifier.Replace("?", "")]).Value}""^^<{ ((LiteralNode)sparqlResult[identifier.Replace("?", "")]).DataType}>.
                                                        ?externalID <http://purl.org/roh#provenance> ?provenance.
                                                    }}");
                                            if (resultSetProvenance.Count > 0)
                                            {
                                                identifiersDiscover[entityRDF][identifiersVars[identifier]] = new KeyValuePair<string, HashSet<string>>(((LiteralNode)sparqlResult[identifier.Replace("?", "")]).Value, new HashSet<string>(resultSetProvenance.Results.ToList().Select(x => x["provenance"].ToString()).ToList()));
                                            }
                                            else
                                            {
                                                identifiersDiscover[entityRDF][identifiersVars[identifier]] = new KeyValuePair<string, HashSet<string>>(((LiteralNode)sparqlResult[identifier.Replace("?", "")]).Value, new HashSet<string>());
                                            }

                                            //Si se ha añadido un triple es que se ha insertado
                                            if (numtriplesDespues > numtriplesAntes)
                                            {
                                                pHasChanges = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!pListaEntidadesReconciliadas.ContainsKey(entityRDF))
                    {
                        //Si la entidad no ha sido reconciliada probamos a reconciliarla con los nuevos datos obtenidos
                        pListaEntidadesRDFEnriquecer.Add(entityRDF, urlEntidadExterna);
                    }
                }
            }
            return identifiersDiscover;
        }


        /// <summary>
        /// Integración con el API de ORCID
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de ORCID</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationORCID(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new ORCID_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();
            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todas las obras de las personas que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación


            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }

            Dictionary<string, string> personasNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> personasObras = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/foaf#Person"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?person =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            personasNombres[entityID] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            if (!personasObras.ContainsKey(entityID))
                            {
                                personasObras[entityID] = new Dictionary<string, string>();
                            }
                            personasObras[entityID][sparqlResult["doc"].ToString()] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                    }
                }
            }

            foreach (string idPersonRDF in personasNombres.Keys)
            {
                string nombrePersona = personasNombres[idPersonRDF].Trim();

                //1.-Hacemos una petición a ORCID al método  ‘expanded - search' con el nombre de la persona
                string q = HttpUtility.UrlEncode(NormalizeName(nombrePersona.ToLower(), pDiscoverCache, false, true, true).Trim());
                ORCIDExpandedSearch expandedSearch = SelectORCIDExpandedSearchCache(q, pDiscoverCache);
                if (expandedSearch.expanded_result != null)
                {
                    foreach (ORCIDExpandedSearch.Result result in expandedSearch.expanded_result)
                    {
                        string name = result.given_names + " " + result.family_names;
                        string idPerson = "http://orcid.com/Person/" + result.orcid_id;

                        //comprobamos la similitud del nombre obtenido con el nombre del RDF
                        //Si no alcanza un mínimo de similitud procedemos con el siguiente resultado que habíamos obtenido con el método 'expanded - search’, así hasta llegar al 5º          
                        if (!string.IsNullOrEmpty(result.orcid_id) && GetNameSimilarity(name, nombrePersona, pDiscoverCache) > 0)
                        {
                            bool estaPersonaEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPersonRDF);
                            bool coicidenPulicaciones = false;

                            RohGraph orcidGraph = new RohGraph();
                            //Si sí que se alcanza esa similitud se procede con el siguiente paso.

                            //Insertamos los datos de la persona (nombre + ORCID)
                            IUriNode subjectPerson = orcidGraph.CreateUriNode(UriFactory.Create(idPerson));
                            IUriNode rdftypeProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                            IUriNode rdftypePerson = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                            orcidGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                            IUriNode nameProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                            ILiteralNode namePerson = orcidGraph.CreateLiteralNode(name, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            orcidGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                            IUriNode orcidProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                            ILiteralNode nameOrcid = orcidGraph.CreateLiteralNode(result.orcid_id, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            orcidGraph.Assert(new Triple(subjectPerson, orcidProperty, nameOrcid));
                            AddExternalIDProvenance(orcidGraph, subjectPerson, orcidProperty, nameOrcid, provenanceGraphUri);

                            //Hacemos peticiones al métdo dee ORCID ‘orcid}/ person' y almacenamos en un grafo en local los datos de los identificadores                       
                            ORCIDPerson person = SelectORCIDPersonCache(result.orcid_id, pDiscoverCache);

                            if (person.external_identifiers != null && person.external_identifiers.external_identifier != null)
                            {
                                foreach (ORCIDPerson.ExternalIdentifiers.ExternalIdentifier extIdentifier in person.external_identifiers.external_identifier)
                                {
                                    if (extIdentifier.external_id_type == "ResearcherID")
                                    {
                                        IUriNode researcherProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#researcherId"));
                                        ILiteralNode nameResearcher = orcidGraph.CreateLiteralNode(extIdentifier.external_id_value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        orcidGraph.Assert(new Triple(subjectPerson, researcherProperty, nameResearcher));
                                        AddExternalIDProvenance(orcidGraph, subjectPerson, researcherProperty, nameResearcher, provenanceGraphUri);
                                    }
                                    else if (extIdentifier.external_id_type == "Scopus Author ID")
                                    {
                                        IUriNode scopusProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#scopusId"));
                                        ILiteralNode nameScopus = orcidGraph.CreateLiteralNode(extIdentifier.external_id_value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        orcidGraph.Assert(new Triple(subjectPerson, scopusProperty, nameScopus));
                                        AddExternalIDProvenance(orcidGraph, subjectPerson, scopusProperty, nameScopus, provenanceGraphUri);
                                    }
                                }
                            }


                            //y 'orcid}/ works’ y almacenamos en un grafo en local los datos de los trabajos realizados.
                            ORCIDWorks works = SelectORCIDWorksCache(result.orcid_id, pDiscoverCache);
                            Dictionary<string, KeyValuePair<string, string>> worksData = new Dictionary<string, KeyValuePair<string, string>>();
                            if (works.group != null)
                            {
                                foreach (ORCIDWorks.Group group in works.group)
                                {
                                    if (group.work_summary != null && group.work_summary.Count > 0)
                                    {
                                        ORCIDWorks.Group.WorkSummary workSummary = group.work_summary[0];

                                        if (workSummary.title != null && workSummary.title.title2 != null && workSummary.title.title2.value != null)
                                        {
                                            string code = workSummary.putcode;
                                            string title = workSummary.title.title2.value;
                                            string doi = "";
                                            if (workSummary.externalids != null && workSummary.externalids.externalid != null)
                                            {
                                                foreach (ORCIDWorks.Group.WorkSummary.Externalids.Externalid extIdentifier in workSummary.externalids.externalid)
                                                {
                                                    if (extIdentifier.externalidtype == "doi")
                                                    {
                                                        doi = extIdentifier.externalidvalue;
                                                    }
                                                }
                                            }
                                            worksData[code] = new KeyValuePair<string, string>(title, doi);
                                            if (personasObras[idPersonRDF].Where(x => GetSimilarity(x.Value, title, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0).Count() > 0)
                                            {
                                                //Puede coincidir con alguna publicación del RDF
                                                coicidenPulicaciones = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (estaPersonaEnDuda || coicidenPulicaciones)
                            {
                                foreach (string workCode in worksData.Keys)
                                {
                                    if (estaPersonaEnDuda || personasObras[idPersonRDF].Where(x => GetSimilarity(x.Value, worksData[workCode].Key, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0).Count() > 0)
                                    {
                                        IUriNode subjectWork = orcidGraph.CreateUriNode(UriFactory.Create("http://orcid.com/Work/" + workCode));

                                        IUriNode titleProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                        ILiteralNode nameTitle = orcidGraph.CreateLiteralNode(worksData[workCode].Key, new Uri("http://www.w3.org/2001/XMLSchema#string"));
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

                                        if (!string.IsNullOrEmpty(worksData[workCode].Value))
                                        {
                                            IUriNode doiProperty = orcidGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                            ILiteralNode nameDoi = orcidGraph.CreateLiteralNode(worksData[workCode].Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            orcidGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                            AddExternalIDProvenance(orcidGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                        }
                                    }
                                }
                                externalGraph.Merge(orcidGraph);
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Integración con el API de SCOPUS
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de SCOPUS</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationSCOPUS(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new SCOPUS_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();
            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todos los autores de las obras que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación


            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }


            Dictionary<string, string> publicacionesNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> publicacionesAutores = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/bibo#Document"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?doc =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            publicacionesNombres[entityID] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            if (!publicacionesAutores.ContainsKey(entityID))
                            {
                                publicacionesAutores[entityID] = new Dictionary<string, string>();
                            }
                            publicacionesAutores[entityID][sparqlResult["person"].ToString()] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                    }
                }
            }

            foreach (string idPublicacionRDF in publicacionesNombres.Keys)
            {
                string tituloPublicacion = publicacionesNombres[idPublicacionRDF].Trim();
                string queryScopus = NormalizeName(tituloPublicacion.ToLower(), pDiscoverCache, false, false, false).Trim();
                while (queryScopus.Contains("  "))
                {
                    queryScopus = queryScopus.Replace("  ", " ");
                }
                queryScopus = queryScopus.Replace(" ", "' '");
                queryScopus = "'" + queryScopus + "'";
                string q = HttpUtility.UrlEncode(queryScopus);
                SCOPUSWorks works = SelectSCOPUSWorksCache(q, pDiscoverCache);

                if (works.entry.Count() > 0)
                {
                    foreach (Work work in works.entry)
                    {
                        if (work.author != null)
                        {
                            if (GetSimilarity(work.title, tituloPublicacion, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0)
                            {
                                bool estaPublicacionEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPublicacionRDF);
                                bool coicidenAutores = false;

                                RohGraph scopusGraph = new RohGraph();

                                string idWork = "http://scopus.com/Work/" + work.identifier.Replace("SCOPUS_ID:", "");

                                IUriNode rdftypeProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));

                                IUriNode subjectWork = scopusGraph.CreateUriNode(UriFactory.Create(idWork));

                                IUriNode titleProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                ILiteralNode nameTitle = scopusGraph.CreateLiteralNode(work.title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                scopusGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                                IBlankNode subjectAuthorList = scopusGraph.CreateBlankNode();
                                IUriNode rdftypeAuthorList = scopusGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                                scopusGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                                IUriNode authorListProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                                scopusGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                                IUriNode rdftypeDocument = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                                scopusGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));

                                IUriNode scopusProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#scopusId"));
                                ILiteralNode nameScopus = scopusGraph.CreateLiteralNode(work.identifier.Replace("SCOPUS_ID:", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                scopusGraph.Assert(new Triple(subjectWork, scopusProperty, nameScopus));
                                AddExternalIDProvenance(scopusGraph, subjectWork, scopusProperty, nameScopus, provenanceGraphUri);

                                if (!string.IsNullOrEmpty(work.doi))
                                {
                                    IUriNode doiProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                    ILiteralNode nameDoi = scopusGraph.CreateLiteralNode(work.doi, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    scopusGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                    AddExternalIDProvenance(scopusGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                }

                                foreach (WorkAuthor author in work.author)
                                {
                                    string personName = (author.givenname + " " + author.surname).Trim();
                                    if (publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                    {
                                        //Puede coincidir con alguna persona del RDF
                                        coicidenAutores = true;
                                    }
                                }
                                if (estaPublicacionEnDuda || coicidenAutores)
                                {
                                    foreach (WorkAuthor author in work.author)
                                    {
                                        string personName = (author.givenname + " " + author.surname).Trim();
                                        if (estaPublicacionEnDuda || publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                        {
                                            string idPerson = "http://scopus.com/Person/" + author.authid;
                                            SCOPUSPerson person = SelectSCOPUSPersonCache(author.authid, pDiscoverCache);
                                            IUriNode subjectPerson = scopusGraph.CreateUriNode(UriFactory.Create(idPerson));
                                            IUriNode rdftypePerson = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                            scopusGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                            ILiteralNode nameScopusAuthor = scopusGraph.CreateLiteralNode(author.authid.ToString(), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            scopusGraph.Assert(new Triple(subjectWork, scopusProperty, nameScopusAuthor));

                                            IUriNode nameProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                            ILiteralNode namePerson = scopusGraph.CreateLiteralNode(personName, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            scopusGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                            if (!string.IsNullOrEmpty(person.coredata.orcid))
                                            {
                                                IUriNode orcidProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                                ILiteralNode nameOrcid = scopusGraph.CreateLiteralNode(person.coredata.orcid, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                scopusGraph.Assert(new Triple(subjectPerson, orcidProperty, nameOrcid));
                                                AddExternalIDProvenance(scopusGraph, subjectPerson, orcidProperty, nameOrcid, provenanceGraphUri);
                                            }

                                            IUriNode firstAuthorProperty = scopusGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                            scopusGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));
                                        }
                                    }
                                    externalGraph.Merge(scopusGraph);
                                }
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }



        /// <summary>
        /// Integración con el API de DBLP
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de DBLP</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationDBLP(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new DBLP_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();

            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todas las obras de las personas que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación

            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }

            Dictionary<string, string> personasNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> personasObras = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/foaf#Person"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?person =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            personasNombres[entityID] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            if (!personasObras.ContainsKey(entityID))
                            {
                                personasObras[entityID] = new Dictionary<string, string>();
                            }
                            personasObras[entityID][sparqlResult["doc"].ToString()] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                    }
                }
            }

            foreach (string idPersonRDF in personasNombres.Keys)
            {
                string nombrePersona = personasNombres[idPersonRDF].Trim();

                //1.-Hacemos una petición a DBLP
                string q = nombrePersona.ToLower();

                DBLPAuthors authors = SelectDBLPAuthorsCache(q, pDiscoverCache);
                if (authors != null && authors.hits != null && authors.hits.hit != null)
                {
                    foreach (resultHitsHit result in authors.hits.hit)
                    {
                        string name = result.info.author;
                        string idPerson = result.info.url;

                        //comprobamos la similitud del nombre obtenido con el nombre del RDF
                        //Si no alcanza un mínimo de similitud procedemos con el siguiente resultado que habíamos obtenido con el método 
                        if (GetNameSimilarity(name, nombrePersona, pDiscoverCache) > 0)
                        {
                            bool estaPersonaEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPersonRDF);
                            bool coicidenPulicaciones = false;

                            RohGraph dblpGraph = new RohGraph();

                            //Si sí que se alcanza esa similitud se procede con el siguiente paso.

                            //Insertamos los datos de la persona (nombre + ORCID)
                            IUriNode subjectPerson = dblpGraph.CreateUriNode(UriFactory.Create(idPerson));
                            IUriNode rdftypeProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                            IUriNode rdftypePerson = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                            dblpGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                            IUriNode nameProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                            ILiteralNode namePerson = dblpGraph.CreateLiteralNode(name, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            dblpGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                            IUriNode dblpProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#researcherDBLP"));
                            ILiteralNode nameDBLP = dblpGraph.CreateLiteralNode(idPerson.Replace("https://dblp.org/pid/", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            dblpGraph.Assert(new Triple(subjectPerson, dblpProperty, nameDBLP));
                            AddExternalIDProvenance(dblpGraph, subjectPerson, dblpProperty, nameDBLP, provenanceGraphUri);

                            //Hacemos peticiones al métdo 'person' y almacenamos en un grafo en local los datos de los identificadores                       
                            DBLPPerson person = SelectDBLPPersonCache(idPerson, pDiscoverCache);
                            if (person != null && person.person != null && person.person.url != null)
                            {
                                foreach (string url in person.person.url)
                                {
                                    if (url.StartsWith("https://orcid.org/"))
                                    {
                                        IUriNode orcidProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                        ILiteralNode nameORCID = dblpGraph.CreateLiteralNode(url.Replace("https://orcid.org/", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        dblpGraph.Assert(new Triple(subjectPerson, orcidProperty, nameORCID));
                                        AddExternalIDProvenance(dblpGraph, subjectPerson, orcidProperty, nameORCID, provenanceGraphUri);
                                    }
                                    else if (url.StartsWith("https://www.researcherid.com/rid/"))
                                    {
                                        IUriNode researcherProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#researcherId"));
                                        ILiteralNode nameResearcher = dblpGraph.CreateLiteralNode(url.Replace("https://www.researcherid.com/rid/", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        dblpGraph.Assert(new Triple(subjectPerson, researcherProperty, nameResearcher));
                                        AddExternalIDProvenance(dblpGraph, subjectPerson, researcherProperty, nameResearcher, provenanceGraphUri);
                                    }
                                }
                            }

                            foreach (dblppersonR dblppersonR in person.r)
                            {
                                string title = "";
                                if (dblppersonR.article != null)
                                {
                                    title = dblppersonR.article.title;
                                }
                                if (dblppersonR.collection != null)
                                {
                                    title = dblppersonR.collection.title;
                                }
                                if (dblppersonR.incollection != null)
                                {
                                    title = dblppersonR.incollection.title;
                                }
                                if (dblppersonR.proceedings != null)
                                {
                                    title = dblppersonR.proceedings.title;
                                }
                                if (dblppersonR.inproceedings != null)
                                {
                                    title = dblppersonR.inproceedings.title;
                                }

                                if (personasObras[idPersonRDF].Where(x => GetSimilarity(x.Value, title, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0).Count() > 0)
                                {
                                    //Puede coincidir con alguna publicación del RDF
                                    coicidenPulicaciones = true;
                                }
                            }

                            if (estaPersonaEnDuda || coicidenPulicaciones)
                            {
                                foreach (dblppersonR dblppersonR in person.r)
                                {
                                    string title = "";
                                    string key = "";
                                    HashSet<string> urls = new HashSet<string>();
                                    if (dblppersonR.article != null)
                                    {
                                        key = dblppersonR.article.key;
                                        title = dblppersonR.article.title;
                                        if (dblppersonR.article.ee != null)
                                        {
                                            urls.UnionWith(dblppersonR.article.ee.ToList());
                                        }
                                    }
                                    if (dblppersonR.collection != null)
                                    {
                                        key = dblppersonR.collection.key;
                                        title = dblppersonR.collection.title;
                                        if (dblppersonR.collection.ee != null)
                                        {
                                            urls.UnionWith(dblppersonR.collection.ee.ToList());
                                        }
                                    }
                                    if (dblppersonR.incollection != null)
                                    {
                                        key = dblppersonR.incollection.key;
                                        title = dblppersonR.incollection.title;
                                        if (dblppersonR.incollection.ee != null)
                                        {
                                            urls.UnionWith(dblppersonR.incollection.ee.ToList());
                                        }
                                    }
                                    if (dblppersonR.proceedings != null)
                                    {
                                        key = dblppersonR.proceedings.key;
                                        title = dblppersonR.proceedings.title;
                                        if (dblppersonR.proceedings.ee != null)
                                        {
                                            urls.UnionWith(dblppersonR.proceedings.ee.ToList());
                                        }
                                    }
                                    if (dblppersonR.inproceedings != null)
                                    {
                                        key = dblppersonR.inproceedings.key;
                                        title = dblppersonR.inproceedings.title;
                                        if (dblppersonR.inproceedings.ee != null)
                                        {
                                            urls.UnionWith(dblppersonR.inproceedings.ee.ToList());
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(key))
                                    {
                                        if (estaPersonaEnDuda || personasObras[idPersonRDF].Where(x => GetSimilarity(x.Value, title, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0).Count() > 0)
                                        {
                                            IUriNode subjectWork = dblpGraph.CreateUriNode(UriFactory.Create("https://dblp.org/rec/" + key));

                                            IUriNode titleProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                            ILiteralNode nameTitle = dblpGraph.CreateLiteralNode(title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            dblpGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                                            IBlankNode subjectAuthorList = dblpGraph.CreateBlankNode();
                                            IUriNode rdftypeAuthorList = dblpGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                                            dblpGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                                            IUriNode authorListProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                                            dblpGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                                            IUriNode firstAuthorProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                            dblpGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));

                                            IUriNode rdftypeDocument = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                                            dblpGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));

                                            IUriNode dblpPropertyResearchObject = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#roDBLP"));
                                            ILiteralNode nameResearchObjectDBLP = dblpGraph.CreateLiteralNode(key, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            dblpGraph.Assert(new Triple(subjectWork, dblpPropertyResearchObject, nameResearchObjectDBLP));
                                            AddExternalIDProvenance(dblpGraph, subjectWork, dblpPropertyResearchObject, nameResearchObjectDBLP, provenanceGraphUri);

                                            foreach (string url in urls)
                                            {
                                                if (url.StartsWith("https://doi.org/"))
                                                {
                                                    IUriNode doiProperty = dblpGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                                    ILiteralNode nameDoi = dblpGraph.CreateLiteralNode(url.Replace("https://doi.org/", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                    dblpGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                                    AddExternalIDProvenance(dblpGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                                }
                                            }
                                        }
                                    }
                                }
                                externalGraph.Merge(dblpGraph);
                            }


                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Integración con el API de CROSSREF
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de CROSSREF</returns>
        private static KeyValuePair< RohGraph,RohGraph> ExternalIntegrationCROSSREF(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph= CreateProvenanceGraph(new CROSSREF_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();

            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todas las obras de las personas que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación
            
            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }

            Dictionary<string, string> personasNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> personasObras = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/foaf#Person"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?person =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            personasNombres[entityID] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            if (!personasObras.ContainsKey(entityID))
                            {
                                personasObras[entityID] = new Dictionary<string, string>();
                            }
                            personasObras[entityID][sparqlResult["doc"].ToString()] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                    }
                }
            }

            foreach (string idPersonRDF in personasNombres.Keys)
            {
                string nombrePersona = personasNombres[idPersonRDF].Trim();

                //1.-Hacemos una petición a CROSSREF
                string q = HttpUtility.UrlEncode(nombrePersona.ToLower());

                CROSSREF_Works works = SelectCROSSREF_WorksCache(q, pDiscoverCache);
                if (works != null && works.message != null && works.message.items != null)
                {
                    //Cargamos los autores con su orcid y sus obras
                    Dictionary<string, string> authorsORCID = new Dictionary<string, string>();
                    Dictionary<string, List<Item>> authorsWorks = new Dictionary<string, List<Item>>();
                    foreach (Item item in works.message.items)
                    {
                        if (item.author != null)
                        {
                            foreach (Author author in item.author)
                            {
                                string nombre = $"{author.given} {author.family}";
                                string orcid = author.ORCID;
                                if (!string.IsNullOrEmpty(orcid))
                                {
                                    orcid = orcid.Replace("http://orcid.org/", "");
                                }
                                if (!authorsORCID.ContainsKey(nombre))
                                {
                                    authorsORCID[nombre] = orcid;
                                }
                                else if (!string.IsNullOrEmpty(orcid))
                                {
                                    authorsORCID[nombre] = orcid;
                                }
                                if (!authorsWorks.ContainsKey(nombre))
                                {
                                    authorsWorks.Add(nombre, new List<Item>());
                                }
                                authorsWorks[nombre].Add(item);
                            }
                        }
                    }
                    if (authorsWorks.Count > 0)
                    {
                        foreach (string name in authorsORCID.Keys)
                        {
                            string idPerson = "http://crossref.org/" + HttpUtility.UrlEncode(name);

                            //comprobamos la similitud del nombre obtenido con el nombre del RDF
                            //Si no alcanza un mínimo de similitud procedemos con el siguiente resultado que habíamos obtenido con el método 
                            if (GetNameSimilarity(name, nombrePersona, pDiscoverCache) > 0)
                            {
                                bool estaPersonaEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPersonRDF);
                                bool coicidenPulicaciones = false;

                                RohGraph crossrefGraph = new RohGraph();

                                //Si sí que se alcanza esa similitud se procede con el siguiente paso.

                                //Insertamos los datos de la persona (nombre + ORCID)
                                IUriNode subjectPerson = crossrefGraph.CreateUriNode(UriFactory.Create(idPerson));
                                IUriNode rdftypeProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                                IUriNode rdftypePerson = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                crossrefGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                IUriNode nameProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                ILiteralNode namePerson = crossrefGraph.CreateLiteralNode(name, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                crossrefGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                if (!string.IsNullOrEmpty(authorsORCID[name]))
                                {
                                    IUriNode orcidProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                    ILiteralNode nameORCID = crossrefGraph.CreateLiteralNode(authorsORCID[name], new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    crossrefGraph.Assert(new Triple(subjectPerson, orcidProperty, nameORCID));
                                    AddExternalIDProvenance(crossrefGraph, subjectPerson, orcidProperty, nameORCID, provenanceGraphUri);
                                }


                                foreach (Item item in authorsWorks[name])
                                {
                                    if (personasObras[idPersonRDF].Where(x => GetSimilarity(x.Value, item.title[0], Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0).Count() > 0)
                                    {
                                        //Puede coincidir con alguna publicación del RDF
                                        coicidenPulicaciones = true;
                                    }
                                }

                                if (estaPersonaEnDuda || coicidenPulicaciones)
                                {
                                    foreach (Item item in authorsWorks[name])
                                    {
                                        string title = item.title[0];
                                        string url = "http://crossref.org/item/" + HttpUtility.UrlEncode(title);
                                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
                                        {
                                            if (estaPersonaEnDuda || personasObras[idPersonRDF].Where(x => GetSimilarity(x.Value, title, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0).Count() > 0)
                                            {
                                                IUriNode subjectWork = crossrefGraph.CreateUriNode(UriFactory.Create(url));

                                                IUriNode titleProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                                ILiteralNode nameTitle = crossrefGraph.CreateLiteralNode(title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                crossrefGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                                                IBlankNode subjectAuthorList = crossrefGraph.CreateBlankNode();
                                                IUriNode rdftypeAuthorList = crossrefGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                                                crossrefGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                                                IUriNode authorListProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                                                crossrefGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                                                IUriNode firstAuthorProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                                crossrefGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));

                                                IUriNode rdftypeDocument = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                                                crossrefGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));


                                                if (!string.IsNullOrEmpty(item.DOI))
                                                {
                                                    IUriNode doiProperty = crossrefGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                                    ILiteralNode nameDoi = crossrefGraph.CreateLiteralNode(item.DOI, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                    crossrefGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                                    AddExternalIDProvenance(crossrefGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                                }
                                            }
                                        }
                                    }
                                    externalGraph.Merge(crossrefGraph);
                                }
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Integración con el API de PUBMED
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de SCOPUS</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationPUBMED(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new PUBMED_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();
            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todos los autores de las obras que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación


            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }


            Dictionary<string, string> publicacionesNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> publicacionesAutores = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/bibo#Document"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?doc =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            publicacionesNombres[entityID] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            if (!publicacionesAutores.ContainsKey(entityID))
                            {
                                publicacionesAutores[entityID] = new Dictionary<string, string>();
                            }
                            publicacionesAutores[entityID][sparqlResult["person"].ToString()] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                    }
                }
            }

            foreach (string idPublicacionRDF in publicacionesNombres.Keys)
            {
                string tituloPublicacion = publicacionesNombres[idPublicacionRDF].Trim();
                string q = HttpUtility.UrlEncode(tituloPublicacion);
                uint[] ids_works = SelectPUBMED_WorkSearchByTitle(q, pDiscoverCache);
                if (ids_works != null && ids_works.Count() > 0)
                {
                    foreach (uint idWorkInt in ids_works)
                    {
                        PubmedArticleSet pubmedArticleSet = SelectPUBMED_GetWorkByID(idWorkInt, pDiscoverCache);
                        if (pubmedArticleSet != null && pubmedArticleSet.PubmedArticle != null && pubmedArticleSet.PubmedArticle.MedlineCitation != null && pubmedArticleSet.PubmedArticle.MedlineCitation.Article != null)
                        {
                            PubmedArticleSetPubmedArticleMedlineCitationArticle article = pubmedArticleSet.PubmedArticle.MedlineCitation.Article;
                            string title = article.ArticleTitle.Text[0];
                            PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor[] authors = article.AuthorList.Author;
                            if (authors != null && authors.Count() > 0)
                            {
                                if (GetSimilarity(title, tituloPublicacion, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0)
                                {
                                    bool estaPublicacionEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPublicacionRDF);
                                    bool coicidenAutores = false;

                                    RohGraph pubmedGraph = new RohGraph();

                                    string idWork = "https://pubmed.ncbi.nlm.nih.gov/" + idWorkInt;

                                    IUriNode rdftypeProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));

                                    IUriNode subjectWork = pubmedGraph.CreateUriNode(UriFactory.Create(idWork));

                                    IUriNode titleProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                    ILiteralNode nameTitle = pubmedGraph.CreateLiteralNode(title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    pubmedGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                                    IBlankNode subjectAuthorList = pubmedGraph.CreateBlankNode();
                                    IUriNode rdftypeAuthorList = pubmedGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                                    pubmedGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                                    IUriNode authorListProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                                    pubmedGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                                    IUriNode rdftypeDocument = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                                    pubmedGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));

                                    IUriNode pubmedProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#roPubmed"));
                                    ILiteralNode namePubmed = pubmedGraph.CreateLiteralNode(idWorkInt.ToString(), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    pubmedGraph.Assert(new Triple(subjectWork, pubmedProperty, namePubmed));
                                    AddExternalIDProvenance(pubmedGraph, subjectWork, pubmedProperty, namePubmed, provenanceGraphUri);

                                    if (article.ELocationID != null)
                                    {
                                        foreach (PubmedArticleSetPubmedArticleMedlineCitationArticleELocationID elocation in article.ELocationID)
                                        {
                                            if (elocation.EIdType == "doi")
                                            {
                                                IUriNode doiProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                                ILiteralNode nameDoi = pubmedGraph.CreateLiteralNode(elocation.Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                pubmedGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                                AddExternalIDProvenance(pubmedGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                            }
                                        }
                                    }

                                    foreach (PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor author in article.AuthorList.Author)
                                    {
                                        string personName = (author.ForeName + " " + author.LastName).Trim();
                                        if (publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                        {
                                            //Puede coincidir con alguna persona del RDF
                                            coicidenAutores = true;
                                        }
                                    }
                                    if (estaPublicacionEnDuda || coicidenAutores)
                                    {
                                        foreach (PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor author in article.AuthorList.Author)
                                        {
                                            string personName = (author.ForeName + " " + author.LastName).Trim();
                                            if (estaPublicacionEnDuda || publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                            {
                                                string idPerson = "http://pubmed.org/" + HttpUtility.UrlEncode(personName);
                                                IUriNode subjectPerson = pubmedGraph.CreateUriNode(UriFactory.Create(idPerson));
                                                IUriNode rdftypePerson = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                                pubmedGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                                IUriNode nameProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                                ILiteralNode namePerson = pubmedGraph.CreateLiteralNode(personName, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                pubmedGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                                if (author.Identifier != null && author.Identifier.Source == "ORCID")
                                                {
                                                    IUriNode orcidProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                                    ILiteralNode nameOrcid = pubmedGraph.CreateLiteralNode(author.Identifier.Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                    pubmedGraph.Assert(new Triple(subjectPerson, orcidProperty, nameOrcid));
                                                    AddExternalIDProvenance(pubmedGraph, subjectPerson, orcidProperty, nameOrcid, provenanceGraphUri);
                                                }

                                                IUriNode firstAuthorProperty = pubmedGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                                pubmedGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));
                                            }
                                        }
                                        externalGraph.Merge(pubmedGraph);
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Integración con el API de WOS
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de SCOPUS</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationWOS(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new SCOPUS_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();

            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todos los autores de las obras que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación

            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }


            Dictionary<string, string> publicacionesNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> publicacionesAutores = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/bibo#Document"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?doc =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            publicacionesNombres[entityID] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            if (!publicacionesAutores.ContainsKey(entityID))
                            {
                                publicacionesAutores[entityID] = new Dictionary<string, string>();
                            }
                            publicacionesAutores[entityID][sparqlResult["person"].ToString()] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                    }
                }
            }

            foreach (string idPublicacionRDF in publicacionesNombres.Keys)
            {
                string tituloPublicacion = publicacionesNombres[idPublicacionRDF].Trim();
                string q = HttpUtility.UrlEncode(tituloPublicacion);
                WOSWorks works = SelectWOSWorksCache(q, pDiscoverCache);


                if (works.Body.searchResponse.@return.records.records.Count() > 0)
                {
                    foreach (recordsREC record in works.Body.searchResponse.@return.records.records)
                    {
                        string title = record.static_data.summary.titles.title.First(x => x.type == "item").Value;
                        if (GetSimilarity(title, tituloPublicacion, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0)
                        {
                            HashSet<string> authors = new HashSet<string>();
                            Dictionary<string, Dictionary<string, string>> authorsIdentifiers = new Dictionary<string, Dictionary<string, string>>();
                            if (record.static_data.summary.names.name != null)
                            {
                                foreach (recordsRECStatic_dataSummaryNamesName name in record.static_data.summary.names.name)
                                {
                                    authors.Add(name.display_name);
                                }
                            }
                            if (record.static_data.contributors != null && record.static_data.contributors.contributor != null)
                            {
                                foreach (recordsRECStatic_dataContributorsContributor contributor in record.static_data.contributors.contributor)
                                {
                                    authors.Add(contributor.name.display_name);
                                    if (!string.IsNullOrEmpty(contributor.name.orcid_id))
                                    {
                                        if (!authorsIdentifiers.ContainsKey(contributor.name.display_name))
                                        {
                                            authorsIdentifiers.Add(contributor.name.display_name, new Dictionary<string, string>());
                                        }
                                        authorsIdentifiers[contributor.name.display_name]["http://purl.org/roh#ORCID"] = contributor.name.orcid_id;
                                    }
                                    if (!string.IsNullOrEmpty(contributor.name.r_id))
                                    {
                                        if (!authorsIdentifiers.ContainsKey(contributor.name.display_name))
                                        {
                                            authorsIdentifiers.Add(contributor.name.display_name, new Dictionary<string, string>());
                                        }
                                        authorsIdentifiers[contributor.name.display_name]["http://purl.org/roh/mirror/vivo#researcherId"] = contributor.name.r_id;
                                    }
                                }
                            }

                            bool estaPublicacionEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPublicacionRDF);
                            bool coicidenAutores = false;

                            RohGraph wosGraph = new RohGraph();

                            string idWork = "http://wos.com/Work/" + record.UID.Replace("WOS:", "");

                            IUriNode rdftypeProperty = wosGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));

                            IUriNode subjectWork = wosGraph.CreateUriNode(UriFactory.Create(idWork));

                            IUriNode titleProperty = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                            ILiteralNode nameTitle = wosGraph.CreateLiteralNode(title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            wosGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                            IBlankNode subjectAuthorList = wosGraph.CreateBlankNode();
                            IUriNode rdftypeAuthorList = wosGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                            wosGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                            IUriNode authorListProperty = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                            wosGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                            IUriNode rdftypeDocument = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                            wosGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));

                            IUriNode wosProperty = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#roWOS"));
                            ILiteralNode nameWos = wosGraph.CreateLiteralNode(record.UID.Replace("WOS:", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            wosGraph.Assert(new Triple(subjectWork, wosProperty, nameWos));
                            AddExternalIDProvenance(wosGraph, subjectWork, wosProperty, nameWos, provenanceGraphUri);

                            foreach (recordsRECDynamic_dataCluster_relatedIdentifier identifier in record.dynamic_data.cluster_related.identifiers)
                            {
                                if (identifier.type == "doi")
                                {
                                    IUriNode doiProperty = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                    ILiteralNode nameDoi = wosGraph.CreateLiteralNode(identifier.value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                    wosGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                    AddExternalIDProvenance(wosGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                }
                            }

                            foreach (string author in authors)
                            {
                                if (publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, author, pDiscoverCache) > 0).Count() > 0)
                                {
                                    //Puede coincidir con alguna persona del RDF
                                    coicidenAutores = true;
                                }
                            }

                            if (estaPublicacionEnDuda || coicidenAutores)
                            {
                                foreach (string personName in authors)
                                {
                                    if (estaPublicacionEnDuda || publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                    {
                                        string idPerson = "http://wos.com/Person/" + HttpUtility.UrlEncode(personName);
                                        IUriNode subjectPerson = wosGraph.CreateUriNode(UriFactory.Create(idPerson));
                                        IUriNode rdftypePerson = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                        wosGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                        IUriNode nameProperty = wosGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                        ILiteralNode namePerson = wosGraph.CreateLiteralNode(personName, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        wosGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                        if (authorsIdentifiers.ContainsKey(personName))
                                        {
                                            foreach (string identifierPropertyWOS in authorsIdentifiers[personName].Keys)
                                            {
                                                IUriNode identifierProperty = wosGraph.CreateUriNode(UriFactory.Create(identifierPropertyWOS));
                                                ILiteralNode nameIdentifier = wosGraph.CreateLiteralNode(authorsIdentifiers[personName][identifierPropertyWOS], new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                wosGraph.Assert(new Triple(subjectPerson, identifierProperty, nameIdentifier));
                                                AddExternalIDProvenance(wosGraph, subjectPerson, identifierProperty, nameIdentifier, provenanceGraphUri);
                                            }
                                        }

                                        IUriNode firstAuthorProperty = wosGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                        wosGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));
                                    }
                                }
                                externalGraph.Merge(wosGraph);
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Integración con el API de RECOLECTA
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de SCOPUS</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationRECOLECTA(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new RECOLECTA_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();
            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todos los autores de las obras que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación

            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }


            Dictionary<string, string> publicacionesNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> publicacionesAutores = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/bibo#Document"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?doc =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            publicacionesNombres[entityID] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            if (!publicacionesAutores.ContainsKey(entityID))
                            {
                                publicacionesAutores[entityID] = new Dictionary<string, string>();
                            }
                            publicacionesAutores[entityID][sparqlResult["person"].ToString()] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                    }
                }
            }

            foreach (string idPublicacionRDF in publicacionesNombres.Keys)
            {
                string tituloPublicacion = publicacionesNombres[idPublicacionRDF].Trim();
                string q = HttpUtility.UrlEncode(NormalizeName(tituloPublicacion, pDiscoverCache, false, false, false));

                List<RecolectaDocument> works = SelectRECOLECTAWorksCache(q, pDiscoverCache);

                if (works.Count() > 0)
                {
                    foreach (RecolectaDocument work in works)
                    {
                        if (work.authorList != null)
                        {
                            if (GetSimilarity(work.title, tituloPublicacion, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0)
                            {
                                bool estaPublicacionEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPublicacionRDF);
                                bool coicidenAutores = false;

                                RohGraph recolectaGraph = new RohGraph();
                                string idWork = "http://recolecta.com/Work/" + HttpUtility.UrlEncode(work.title);

                                IUriNode rdftypeProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));

                                IUriNode subjectWork = recolectaGraph.CreateUriNode(UriFactory.Create(idWork));

                                IUriNode titleProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                                ILiteralNode nameTitle = recolectaGraph.CreateLiteralNode(work.title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                recolectaGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                                IBlankNode subjectAuthorList = recolectaGraph.CreateBlankNode();
                                IUriNode rdftypeAuthorList = recolectaGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                                recolectaGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                                IUriNode authorListProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                                recolectaGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                                IUriNode rdftypeDocument = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                                recolectaGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));

                                if (work.linkList.Count > 0)
                                {
                                    foreach (string link in work.linkList)
                                    {
                                        if (link.Trim().StartsWith("http://dx.doi.org/") || link.Trim().StartsWith("http://doi.org/"))
                                        {
                                            IUriNode doiProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                            ILiteralNode nameDoi = recolectaGraph.CreateLiteralNode(link.Trim().Replace("http://dx.doi.org/", "").Replace("http://doi.org/", ""), new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            recolectaGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                            AddExternalIDProvenance(recolectaGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                        }
                                    }
                                }

                                foreach (string personName in work.authorList.Keys)
                                {
                                    if (publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                    {
                                        //Puede coincidir con alguna persona del RDF
                                        coicidenAutores = true;
                                    }
                                }
                                if (estaPublicacionEnDuda || coicidenAutores)
                                {
                                    foreach (string personName in work.authorList.Keys)
                                    {
                                        if (estaPublicacionEnDuda || publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, personName, pDiscoverCache) > 0).Count() > 0)
                                        {
                                            string idPerson = "http://recolecta.com/Person/" + HttpUtility.UrlEncode(work.title);
                                            IUriNode subjectPerson = recolectaGraph.CreateUriNode(UriFactory.Create(idPerson));
                                            IUriNode rdftypePerson = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                            recolectaGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                            IUriNode nameProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                            ILiteralNode namePerson = recolectaGraph.CreateLiteralNode(personName, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                            recolectaGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                            if (!string.IsNullOrEmpty(work.authorList[personName]))
                                            {
                                                IUriNode orcidProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#ORCID"));
                                                ILiteralNode nameOrcid = recolectaGraph.CreateLiteralNode(work.authorList[personName], new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                                recolectaGraph.Assert(new Triple(subjectPerson, orcidProperty, nameOrcid));
                                                AddExternalIDProvenance(recolectaGraph, subjectPerson, orcidProperty, nameOrcid, provenanceGraphUri);
                                            }

                                            IUriNode firstAuthorProperty = recolectaGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                            recolectaGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));
                                        }
                                    }
                                    externalGraph.Merge(recolectaGraph);
                                }
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Integración con el API de DOAJ
        /// </summary>
        /// <param name="pEntitiesRdfTypes">Diccionario con las entidades y sus clases (con herencia)</param>
        /// <param name="pDiscoverCache">Caché de discover</param>
        /// <param name="pDataGraph">Grafo en local con los datos del RDF</param>
        /// <param name="pDiscoveredEntitiesProbability">Entidades con probabilidades</param>
        /// <returns>Grafo con los datos obtenidos de SCOPUS</returns>
        private static KeyValuePair<RohGraph, RohGraph> ExternalIntegrationDOAJ(Dictionary<string, HashSet<string>> pEntitiesRdfTypes, RohGraph pDataGraph, DiscoverCache pDiscoverCache, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability)
        {
            KeyValuePair<string, RohGraph> dataGraph = CreateProvenanceGraph(new DOAJ_API());
            string provenanceGraphUri = dataGraph.Key;
            RohGraph provenanceGraph = dataGraph.Value;
            RohGraph externalGraph = new RohGraph();
            //Sólo debemos obtener datos de las entidades cargadas en el grafo, tanto para las personas como para las obras
            //Adicionalmente obtendremos todos los autores de las obras que estén en duda (aparecen en 'pDiscoveredEntitiesProbability') pra ayudar en su reconciliación


            int? numWordsTitle = null;
            if (mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title") != null &&
                mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle.HasValue)
            {
                numWordsTitle = mDisambiguationConfigs.FirstOrDefault(x => x.rdfType == "http://purl.org/roh/mirror/bibo#Document").properties.FirstOrDefault(x => x.property == "http://purl.org/roh#title").maxNumWordsTitle;
            }
            else
            {
                //Si no hay configurada similitud de titulo entre documentos no procedemos
                return new KeyValuePair<RohGraph, RohGraph>(null, provenanceGraph);
            }


            Dictionary<string, string> publicacionesNombres = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> publicacionesAutores = new Dictionary<string, Dictionary<string, string>>();
            foreach (string entityID in pEntitiesRdfTypes.Keys)
            {
                if (pEntitiesRdfTypes[entityID].Contains("http://purl.org/roh/mirror/bibo#Document"))
                {
                    string query = @$"select distinct ?person ?name ?doc ?title
                                    where
                                    {{
                                        ?person <http://purl.org/roh/mirror/foaf#name> ?name. 
                                        ?doc <http://purl.org/roh/mirror/bibo#authorList> ?list. 
                                        ?doc <http://purl.org/roh#title> ?title. 
                                        ?list ?enum ?person.
                                        FILTER(?doc =<{entityID}>)
                                    }}";
                    SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
                    foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                    {
                        if (sparqlResult["title"] is LiteralNode)
                        {
                            publicacionesNombres[entityID] = ((LiteralNode)sparqlResult["title"]).Value;
                        }
                        if (sparqlResult["name"] is LiteralNode)
                        {
                            if (!publicacionesAutores.ContainsKey(entityID))
                            {
                                publicacionesAutores[entityID] = new Dictionary<string, string>();
                            }
                            publicacionesAutores[entityID][sparqlResult["person"].ToString()] = ((LiteralNode)sparqlResult["name"]).Value;
                        }
                    }
                }
            }

            foreach (string idPublicacionRDF in publicacionesNombres.Keys)
            {
                string tituloPublicacion = publicacionesNombres[idPublicacionRDF].Trim();
                string q = HttpUtility.UrlEncode(tituloPublicacion);
                DOAJWorks works = SelectDOAJWorksCache(q, pDiscoverCache);
                if (works.results.Count() > 0)
                {
                    foreach (Result record in works.results)
                    {
                        string title = record.bibjson.title;
                        if (GetSimilarity(title, tituloPublicacion, Disambiguation.Property.Type.title, pDiscoverCache, numWordsTitle) > 0)
                        {

                            bool estaPublicacionEnDuda = pDiscoveredEntitiesProbability.ContainsKey(idPublicacionRDF);
                            bool coicidenAutores = false;

                            RohGraph doajGraph = new RohGraph();

                            string idWork = "https://doaj.org/" + record.id;

                            IUriNode rdftypeProperty = doajGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));

                            IUriNode subjectWork = doajGraph.CreateUriNode(UriFactory.Create(idWork));

                            IUriNode titleProperty = doajGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#title"));
                            ILiteralNode nameTitle = doajGraph.CreateLiteralNode(title, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                            doajGraph.Assert(new Triple(subjectWork, titleProperty, nameTitle));

                            IBlankNode subjectAuthorList = doajGraph.CreateBlankNode();
                            IUriNode rdftypeAuthorList = doajGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                            doajGraph.Assert(new Triple(subjectAuthorList, rdftypeProperty, rdftypeAuthorList));

                            IUriNode authorListProperty = doajGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#authorList"));
                            doajGraph.Assert(new Triple(subjectWork, authorListProperty, subjectAuthorList));

                            IUriNode rdftypeDocument = doajGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#Document"));
                            doajGraph.Assert(new Triple(subjectWork, rdftypeProperty, rdftypeDocument));

                            if (record.bibjson.identifier != null)
                            {
                                foreach (Identifier identifier in record.bibjson.identifier)
                                {
                                    if (identifier.type == "doi")
                                    {
                                        IUriNode doiProperty = doajGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/bibo#doi"));
                                        ILiteralNode nameDoi = doajGraph.CreateLiteralNode(identifier.id, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        doajGraph.Assert(new Triple(subjectWork, doiProperty, nameDoi));
                                        AddExternalIDProvenance(doajGraph, subjectWork, doiProperty, nameDoi, provenanceGraphUri);
                                    }
                                }
                            }

                            foreach (DoajAuthor author in record.bibjson.author)
                            {
                                if (publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, author.name, pDiscoverCache) > 0).Count() > 0)
                                {
                                    //Puede coincidir con alguna persona del RDF
                                    coicidenAutores = true;
                                }
                            }
                            if (estaPublicacionEnDuda || coicidenAutores)
                            {
                                foreach (DoajAuthor author in record.bibjson.author)
                                {
                                    if (estaPublicacionEnDuda || publicacionesAutores[idPublicacionRDF].Where(x => GetNameSimilarity(x.Value, author.name, pDiscoverCache) > 0).Count() > 0)
                                    {
                                        string idPerson = "http://doaj.org/" + HttpUtility.UrlEncode(author.name);
                                        IUriNode subjectPerson = doajGraph.CreateUriNode(UriFactory.Create(idPerson));
                                        IUriNode rdftypePerson = doajGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Person"));
                                        doajGraph.Assert(new Triple(subjectPerson, rdftypeProperty, rdftypePerson));

                                        IUriNode nameProperty = doajGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                                        ILiteralNode namePerson = doajGraph.CreateLiteralNode(author.name, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                                        doajGraph.Assert(new Triple(subjectPerson, nameProperty, namePerson));

                                        IUriNode firstAuthorProperty = doajGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#_1"));
                                        doajGraph.Assert(new Triple(subjectAuthorList, firstAuthorProperty, subjectPerson));
                                    }
                                }
                                externalGraph.Merge(doajGraph);
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<RohGraph, RohGraph>(externalGraph, provenanceGraph);
        }

        /// <summary>
        /// Añade los triples para conocer la proveniencia de los enriquecimientos con APIs externos
        /// </summary>
        /// <param name="pDataGraph">Grafo con los datos</param>
        /// <param name="pSubject">Sujeto</param>
        /// <param name="pProperty">Propiedad</param>
        /// <param name="pValue">Valor</param>
        /// <param name="pProvenanceUri">Grafo de prevenencia</param>
        private static void AddExternalIDProvenance(RohGraph pDataGraph, IUriNode pSubject, IUriNode pProperty, ILiteralNode pValue, string pProvenanceUri)
        {
            if (!string.IsNullOrEmpty(pProvenanceUri))
            {
                IUriNode externalIdProperty = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#externalID"));
                IBlankNode bnodeExternalId = pDataGraph.CreateBlankNode();
                pDataGraph.Assert(new Triple(pSubject, externalIdProperty, bnodeExternalId));
                pDataGraph.Assert(new Triple(bnodeExternalId, pProperty, pValue));
                IUriNode provenanceProperty = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh#provenance"));
                pDataGraph.Assert(new Triple(bnodeExternalId, provenanceProperty, pDataGraph.CreateUriNode(UriFactory.Create(pProvenanceUri))));
            }
        }

        private static KeyValuePair<string, RohGraph> CreateProvenanceGraph(I_ExternalAPI pExternalAPI)
        {
            //TODO integrar uris factory
            string provenanceGraph = "http://graph.um.es/graph/" + pExternalAPI.Id;
            RohGraph rohApi = new RohGraph();
            IUriNode subjectOrganization = rohApi.CreateUriNode(UriFactory.Create("http://graph.um.es/res/organization/" + pExternalAPI.Id));
            IUriNode rdftypeProperty = rohApi.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
            IUriNode rdftypeOrganization = rohApi.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#Organization"));
            rohApi.Assert(new Triple(subjectOrganization, rdftypeProperty, rdftypeOrganization));
            IUriNode nameProperty = rohApi.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
            ILiteralNode nameValue = rohApi.CreateLiteralNode(pExternalAPI.Name, new Uri("http://www.w3.org/2001/XMLSchema#string"));
            rohApi.Assert(new Triple(subjectOrganization, nameProperty, nameValue));
            IUriNode homepageProperty = rohApi.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#homePage"));
            ILiteralNode homepageValue = rohApi.CreateLiteralNode(pExternalAPI.HomePage, new Uri("http://www.w3.org/2001/XMLSchema#string"));
            rohApi.Assert(new Triple(subjectOrganization, homepageProperty, homepageValue));
            IUriNode subjectGraph = rohApi.CreateUriNode(UriFactory.Create(provenanceGraph));
            IUriNode wasAttributedToProperty = rohApi.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#wasAttributedTo"));
            rohApi.Assert(new Triple(subjectGraph, wasAttributedToProperty, subjectOrganization));
            return new KeyValuePair<string, RohGraph>(provenanceGraph, rohApi);
        }

        #endregion

        #region Auxiliares

        /// <summary>
        /// Extra las clases con todas las subclases de la ontología pasada como parámetro
        /// </summary>
        /// <param name="pOntology">Ontología</param>
        /// <returns>Diccionario con las clases y sus subclases (en las subclases está la propia clase)</returns>
        private static Dictionary<string, HashSet<string>> ExtractClassWithSubclass(RohGraph pOntology)
        {
            Dictionary<string, HashSet<string>> classWithSubclass = new Dictionary<string, HashSet<string>>();
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pOntology.ExecuteQuery("SELECT * WHERE {?class a <http://www.w3.org/2002/07/owl#Class>.OPTIONAL{ ?child <http://www.w3.org/2000/01/rdf-schema#subClassOf> ?class }}");


            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                string Class = sparqlResult["class"].ToString();
                if (!classWithSubclass.ContainsKey(Class))
                {
                    classWithSubclass.Add(Class, new HashSet<string>() { Class });
                }
                if (sparqlResult.Variables.Contains("child"))
                {
                    string Child = sparqlResult["child"].ToString();
                    classWithSubclass[Class].Add(Child);
                }
            }
            bool cambios = true;
            while (cambios)
            {
                cambios = false;
                foreach (string clase in classWithSubclass.Keys.ToList())
                {
                    foreach (string subclase in classWithSubclass[clase].ToList())
                    {
                        if (classWithSubclass.ContainsKey(subclase))
                        {
                            int numAntes = classWithSubclass[clase].Count;
                            classWithSubclass[clase].UnionWith(classWithSubclass[subclase]);
                            int numDespues = classWithSubclass[clase].Count;
                            if (numAntes != numDespues)
                            {
                                cambios = true;
                            }
                        }
                    }
                }
            }
            foreach (string clase in classWithSubclass.Keys.ToList())
            {
                if (!clase.StartsWith("http"))
                {
                    classWithSubclass.Remove(clase);
                }
                else
                {
                    foreach (string subclase in classWithSubclass[clase].ToList())
                    {
                        if (!subclase.StartsWith("http"))
                        {
                            classWithSubclass[clase].Remove(subclase);
                        }
                    }
                }
            }
            return classWithSubclass;
        }


        /// <summary>
        /// Hace una consulta a la BBDD usando la cache de discover
        /// </summary>
        /// <param name="pConsulta">Consulta</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns>Resultado de la consulta</returns>
        private static SparqlObject SelectDataCache(string pConsulta, DiscoverCache pDiscoverCache)
        {
            int hashCode = pConsulta.GetHashCode();
            if (!pDiscoverCache.Sparql.ContainsKey(hashCode))
            {
                pDiscoverCache.Sparql.Add(hashCode, mSparqlUtility.SelectData(mSPARQLEndpoint, mGraph, pConsulta, mQueryParam));
            }
            return pDiscoverCache.Sparql[hashCode];
        }


        /// <summary>
        /// Hace una consulta al api de ORCID usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static ORCIDExpandedSearch SelectORCIDExpandedSearchCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            ORCIDExpandedSearch expandedSearch = null;
            if (pDiscoverCache.ORCIDExpandedSearch.ContainsKey(hashCode))
            {
                expandedSearch = pDiscoverCache.ORCIDExpandedSearch[hashCode];
            }
            else
            {
                expandedSearch = ORCID_API.ExpandedSearch(q);
                pDiscoverCache.ORCIDExpandedSearch[hashCode] = expandedSearch;
            }
            return expandedSearch;
        }


        /// <summary>
        /// Hace una consulta al api de ORCID usando la cache de discover
        /// </summary>
        /// <param name="orcid_id">Identificador de ORCID de la persona</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static ORCIDPerson SelectORCIDPersonCache(string orcid_id, DiscoverCache pDiscoverCache)
        {
            ORCIDPerson person = null;
            if (pDiscoverCache.ORCIDPerson.ContainsKey(orcid_id))
            {
                person = pDiscoverCache.ORCIDPerson[orcid_id];
            }
            else
            {
                person = ORCID_API.Person(orcid_id);
                pDiscoverCache.ORCIDPerson[orcid_id] = person;
            }
            return person;
        }

        /// <summary>
        /// Hace una consulta al api de ORCID usando la cache de discover
        /// </summary>
        /// <param name="orcid_id">Identificador de ORCID de la persona</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static ORCIDWorks SelectORCIDWorksCache(string orcid_id, DiscoverCache pDiscoverCache)
        {
            ORCIDWorks works = null;
            if (pDiscoverCache.ORCIDWorks.ContainsKey(orcid_id))
            {
                works = pDiscoverCache.ORCIDWorks[orcid_id];
            }
            else
            {
                works = ORCID_API.Works(orcid_id);
                pDiscoverCache.ORCIDWorks.Add(orcid_id, works);
            }
            return works;
        }

        /// <summary>
        /// Hace una consulta al api de DBLP usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static DBLPAuthors SelectDBLPAuthorsCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            DBLPAuthors authors = null;
            if (pDiscoverCache.DBLPAuthors.ContainsKey(hashCode))
            {
                authors = pDiscoverCache.DBLPAuthors[hashCode];
            }
            else
            {
                authors = DBLP_API.AuthorSearch(q);
                pDiscoverCache.DBLPAuthors[hashCode] = authors;
            }
            return authors;
        }

        /// <summary>
        /// Hace una consulta al api de DBLP usando la cache de discover
        /// </summary>
        /// <param name="idPerson">Identificador de DBLP de la persona</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static DBLPPerson SelectDBLPPersonCache(string idPerson, DiscoverCache pDiscoverCache)
        {
            DBLPPerson person = null;
            if (pDiscoverCache.DBLPPerson.ContainsKey(idPerson))
            {
                person = pDiscoverCache.DBLPPerson[idPerson];
            }
            else
            {
                person = DBLP_API.Person(idPerson);
                pDiscoverCache.DBLPPerson[idPerson] = person;
            }
            return person;
        }

        /// <summary>
        /// Hace una consulta al api de SCOPUS usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static SCOPUSWorks SelectSCOPUSWorksCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            SCOPUSWorks works = null;
            if (pDiscoverCache.SCOPUSWorks.ContainsKey(hashCode))
            {
                works = pDiscoverCache.SCOPUSWorks[hashCode];
            }
            else
            {
                works = SCOPUS_API.Works(q, mScopusApiKey, mScopusUrl);
                pDiscoverCache.SCOPUSWorks[hashCode] = works;
            }
            return works;
        }

        /// <summary>
        /// Hace una consulta al api de SCOPUS usando la cache de discover
        /// </summary>
        /// <param name="authid">Identificador de SCOPUS de la persona</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static SCOPUSPerson SelectSCOPUSPersonCache(ulong authid, DiscoverCache pDiscoverCache)
        {
            SCOPUSPerson person = null;
            if (pDiscoverCache.SCOPUSPerson.ContainsKey(authid))
            {
                person = pDiscoverCache.SCOPUSPerson[authid];
            }
            else
            {
                person = SCOPUS_API.Person(authid, mScopusApiKey, mScopusUrl);
                pDiscoverCache.SCOPUSPerson[authid] = person;
            }
            return person;
        }

        /// <summary>
        /// Hace una consulta al api de CROSSREF usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static CROSSREF_Works SelectCROSSREF_WorksCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            CROSSREF_Works works;
            if (pDiscoverCache.CROSSREF_Works.ContainsKey(hashCode))
            {
                works = pDiscoverCache.CROSSREF_Works[hashCode];
            }
            else
            {
                works = CROSSREF_API.WorkSearchByContributor(q, mCrossrefUserAgent);
                pDiscoverCache.CROSSREF_Works[hashCode] = works;
            }
            return works;
        }

        /// <summary>
        /// Hace una consulta al api de PUBMED usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static uint[] SelectPUBMED_WorkSearchByTitle(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            uint[] ids;
            if (pDiscoverCache.PUBMED_WorkSearchByTitle.ContainsKey(hashCode))
            {
                ids = pDiscoverCache.PUBMED_WorkSearchByTitle[hashCode];
            }
            else
            {
                ids = PUBMED_API.WorkSearchByTitle(q);
                pDiscoverCache.PUBMED_WorkSearchByTitle[hashCode] = ids;
            }
            return ids;
        }

        /// <summary>
        /// Hace una consulta al api de PUBMED usando la cache de discover
        /// </summary>
        /// <param name="id">identificador del documento</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static PubmedArticleSet SelectPUBMED_GetWorkByID(uint id, DiscoverCache pDiscoverCache)
        {
            PubmedArticleSet article;
            if (pDiscoverCache.PUBMED_WorkByID.ContainsKey(id))
            {
                article = pDiscoverCache.PUBMED_WorkByID[id];
            }
            else
            {
                article = PUBMED_API.GetWorkByID(id);
                pDiscoverCache.PUBMED_WorkByID[id] = article;
            }
            return article;
        }

        /// <summary>
        /// Hace una consulta al api de WOS usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static WOSWorks SelectWOSWorksCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            WOSWorks works = null;
            if (pDiscoverCache.WOSWorks.ContainsKey(hashCode))
            {
                works = pDiscoverCache.WOSWorks[hashCode];
            }
            else
            {
                works = WOS_API.Works(q, mWOSAuthorization);
                pDiscoverCache.WOSWorks[hashCode] = works;
            }
            return works;
        }

        /// <summary>
        /// Hace una consulta al api de RECOLECTA usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static List<RecolectaDocument> SelectRECOLECTAWorksCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            List<RecolectaDocument> works = null;
            if (pDiscoverCache.RECOLECTAWorks.ContainsKey(hashCode))
            {
                works = pDiscoverCache.RECOLECTAWorks[hashCode];
            }
            else
            {
                works = RECOLECTA_API.Works(q);
                pDiscoverCache.RECOLECTAWorks[hashCode] = works;
            }
            return works;
        }

        /// <summary>
        /// Hace una consulta al api de DOAJ usando la cache de discover
        /// </summary>
        /// <param name="q">texto</param>
        /// <param name="pDiscoverCache">Caché de Discover</param>
        /// <returns></returns>
        private static DOAJWorks SelectDOAJWorksCache(string q, DiscoverCache pDiscoverCache)
        {
            string hashCode = q.GetHashCode().ToString();
            DOAJWorks works = null;
            if (pDiscoverCache.DOAJWorks.ContainsKey(hashCode))
            {
                works = pDiscoverCache.DOAJWorks[hashCode];
            }
            else
            {
                works = DOAJ_API.GetWorks(q);
                pDiscoverCache.DOAJWorks[hashCode] = works;
            }
            return works;
        }


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
        /// <param name="pDiscoverCache">Caché de descubrimiento</param>
        /// <returns>Scores</returns>
        private static float GetNameSimilarity(string pNombreA, string pNombreB, DiscoverCache pDiscoverCache)
        {
            float indice_desplazamiento = 5;
            float scoreMin = 0.5f;

            string nombreANormalizado = NormalizeName(pNombreA, pDiscoverCache, true, true, true);
            string nombreBNormalizado = NormalizeName(pNombreB, pDiscoverCache, true, true, true);

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

                if (score > 0)
                {
                    float coefJaccardGNOSS = score * (indice_desplazamiento / (desplazamiento + indice_desplazamiento));
                    scores.Add(coefJaccardGNOSS);
                }

            }
            if (scores.Count > 0)
            {
                float similarity = scores.Sum() / Math.Max(source.Length, target.Length);
                if (similarity > 0.5f)
                {
                    return similarity;
                }
            }
            return 0;
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
        /// Normaliza un texto (Nombre) eliminando acentos y caracteres que no sean letras
        /// </summary>
        /// <param name="text">Texto a normalizar</param>
        /// <param name="pDiscoverCache">Caché de descubrimiento</param>
        /// <param name="eliminaracentos">Indica si deben eliminarse los acentos</param>
        /// <param name="pPropername">Indica si es un nombre propio (personas)</param>
        /// <param name="pReplaceNumbers">Reemplazar numeros</param>
        /// <returns>Texto normalizado</returns>
        private static string NormalizeName(string text, DiscoverCache pDiscoverCache, bool eliminaracentos = true, bool pPropername = true, bool pReplaceNumbers = true)
        {
            string key = $"{eliminaracentos}{pPropername}{pReplaceNumbers}{text}";
            if (pDiscoverCache.NormalizedNames.ContainsKey(key))
            {
                return pDiscoverCache.NormalizedNames[key];
            }
            //Si tiene ',' lo reordenamos
            if (text.Contains(",") && pPropername)
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
                else if (!pReplaceNumbers && char.IsNumber(charin))
                {
                    sb.Append(charin);
                }
                else
                {
                    sb.Append(' ');
                }
            }
            string stringreturn;
            if (eliminaracentos)
            {
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

                stringreturn = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            }
            else
            {
                stringreturn = sb.ToString();
            }
            pDiscoverCache.NormalizedNames[key] = stringreturn;
            return stringreturn;
        }

        #endregion
    }
}
