// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using API_DISCOVER.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using System.Diagnostics.CodeAnalysis;
using API_DISCOVER.Models.Entities.Discover;
using VDS.RDF.Query.Inference;
using API_DISCOVER.Models.Services;

namespace API_DISCOVER.Utility
{
    [ExcludeFromCodeCoverage]
    public class AsioPublication
    {
        private string _SPARQLEndpoint { get; set; }
        private string _QueryParam { get; set; }
        private string _Graph { get; set; }
        private string _Username { get; set; }
        private string _Password { get; set; }
        private RabbitMQService _RabbitMQService { get; set; }

        private readonly I_SparqlUtility _SparqlUtility = new SparqlUtility();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pSPARQLEndpoint">SPARQL endpoint</param>
        /// <param name="pQueryParam">Parámetros para las queries</param>
        /// <param name="pGraph">Grafo de carga</param>
        /// <param name="pUsername">Usuario SPARQL</param>
        /// <param name="pPassword">Password SPARQL</param>
        public AsioPublication(string pSPARQLEndpoint, string pQueryParam, string pGraph, string pUsername, string pPassword, RabbitMQService pRabbitMQService)
        {
            _SPARQLEndpoint = pSPARQLEndpoint;
            _QueryParam = pQueryParam;
            _Graph = pGraph;
            _Username = pUsername;
            _Password = pPassword;
            _RabbitMQService = pRabbitMQService;
        }

        /// <summary>
        /// Publica un RDF en Asio aplicado todos losprocedimientos pertinentes
        /// </summary>
        /// <param name="pDataGraph">Grafo con los datos a cargar</param>
        /// <param name="pOntologyGraph">Grafo con la ontología</param>
        /// <param name="pAttributedTo">Sujeto y nombre para atribuir los triples de los apis externos</param>
        /// <param name="pActivityStartedAtTime">Inicio del proceso</param>
        /// <param name="pActivityEndedAtTime">Fin del proceso</param>
        /// <param name="pDiscoverLinkData">Datos para trabajar con el descubrimiento de enlaces</param>
        /// <param name="pCallUrisFactoryApiService">Servicio para hacer llamadas a los métodos del Uris Factory</param>
        public void PublishRDF(RabbitMQService pRabbitMQService, RohGraph pDataGraph,RohGraph pOntologyGraph, KeyValuePair<string, string>? pAttributedTo, DateTime pActivityStartedAtTime, DateTime pActivityEndedAtTime, DiscoverLinkData pDiscoverLinkData, CallUrisFactoryApiService pCallUrisFactoryApiService)
        {
            RohGraph inferenceDataGraph = null;
            if (pOntologyGraph != null)
            {
                inferenceDataGraph=pDataGraph.Clone();
                RohRdfsReasoner reasoner = new RohRdfsReasoner();
                reasoner.Initialise(pOntologyGraph);
                reasoner.Apply(inferenceDataGraph);
            }

            // 1º Eliminamos de la BBDD las entidades principales que aparecen en el RDF
            HashSet<string> graphs = RemovePrimaryTopics(ref pDataGraph);
            graphs.Add(_Graph);

            // 2º Eliminamos todos los triples de la BBDD cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
            if (pOntologyGraph != null && inferenceDataGraph != null)
            {
                RemoveMonovaluatedProperties(pOntologyGraph, inferenceDataGraph);
            }

            //Actualizamos la propiedad http://www.w3.org/ns/prov#endedAtTime de todos los recursos con IRI que vamos a cargar
            UpdateEndedAtTime(ref pDataGraph, pActivityEndedAtTime);

            //3º Insertamos los triples en la BBDD
            if (pAttributedTo.HasValue)
            {
                //Añadimos triples del softwareagent
                IUriNode t_subject = pDataGraph.CreateUriNode(UriFactory.Create(pAttributedTo.Value.Key));
                IUriNode t_predicate_rdftype = pDataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                IUriNode t_object_rdftype = pDataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#SoftwareAgent"));
                pDataGraph.Assert(new Triple(t_subject, t_predicate_rdftype, t_object_rdftype));
                IUriNode t_predicate_name = pDataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                ILiteralNode t_object_name = pDataGraph.CreateLiteralNode(pAttributedTo.Value.Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                pDataGraph.Assert(new Triple(t_subject, t_predicate_name, t_object_name));
            }
            SparqlUtility.LoadTriples(pRabbitMQService, SparqlUtility.GetTriplesFromGraph(pDataGraph), _SPARQLEndpoint, _QueryParam, _Graph, _Username, _Password);

            //4º Insertamos los triples con provenance en la BBDD
            if (pDiscoverLinkData != null && pDiscoverLinkData.entitiesProperties != null)
            {
                Dictionary<string, List<string>> graphDeletes = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> graphTriples = new Dictionary<string, List<string>>();
                foreach (string t_subject in pDiscoverLinkData.entitiesProperties.Keys)
                {
                    foreach (DiscoverLinkData.PropertyData property in pDiscoverLinkData.entitiesProperties[t_subject])
                    {
                        string t_property = property.property;
                        foreach (var prop in property.valueProvenance)
                        {
                            string t_object = prop.Key;
                            HashSet<string> t_sourceids = prop.Value;
                            foreach (string sourceId in t_sourceids)
                            {
                                string graph = pCallUrisFactoryApiService.GetUri("Graph", sourceId);
                                if (!graphTriples.ContainsKey(graph))
                                {
                                    graphTriples.Add(graph, new List<string>());
                                }
                                string bNodeid = "_:" + Guid.NewGuid().ToString();
                                graphTriples[graph].Add($@"<{t_subject}> <http://www.w3.org/ns/prov#wasUsedBy> {bNodeid} .");
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/ns/prov#Activity> .");
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <{t_property}>.");
                                if (Uri.IsWellFormedUriString(t_object, UriKind.Absolute))
                                {
                                    graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> <{ t_object}>.");
                                }
                                else
                                {
                                    graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> ""{ t_object.Replace("\"", "\\\"").Replace("\n", "\\n") }""^^<http://www.w3.org/2001/XMLSchema#string>.");
                                }
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#startedAtTime> ""{ pActivityStartedAtTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") }""^^<http://www.w3.org/2001/XMLSchema#datetime>.");
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#endedAtTime> ""{ pActivityEndedAtTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") }""^^<http://www.w3.org/2001/XMLSchema#datetime>.");
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#wasAssociatedWith> <{pAttributedTo.Value.Key}>.");
                            
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#wasAssociatedWith> <{pCallUrisFactoryApiService.GetUri("http://purl.org/roh/mirror/foaf#Organization", sourceId)}>.");

                                if (pAttributedTo.HasValue)
                                {
                                    graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#wasAssociatedWith> <{pAttributedTo.Value.Key}>.");
                                }

                                if (!graphDeletes.ContainsKey(graph))
                                {
                                    graphDeletes.Add(graph, new List<string>());
                                }

                                if (!Uri.IsWellFormedUriString(t_object, UriKind.Absolute))
                                {
                                    string stringDelete = $@"   {{
                                                                ?s ?p ?o. 
                                                                ?o <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <{t_property}>.
                                                                ?o <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> ""{ t_object.Replace("\"", "\\\"").Replace("\n", "\\n") }""^^<http://www.w3.org/2001/XMLSchema#string>.
                                                                FILTER(?s = <{t_subject}>)
                                                            }}";
                                    graphDeletes[graph].Add(stringDelete);
                                }

                            }
                        }
                    }
                }

                //Eliminamos aquellos triples de provenance que ya estén cargados
                foreach (string graph in graphDeletes.Keys)
                {
                    graphs.Add(graph);
                    string queryDeleteProvenance = $@"  DELETE {{ ?s ?p ?o. }}
                                                        WHERE 
                                                        {{
                                                            {{{string.Join("}UNION{", graphDeletes[graph])}}}
                                                        }}";
                    _SparqlUtility.SelectData(pRabbitMQService, _SPARQLEndpoint, graph, queryDeleteProvenance, _QueryParam, _Username, _Password);
                }

                //Cargamos los nuevos triples
                foreach (string graph in graphTriples.Keys)
                {
                    SparqlUtility.LoadTriples(pRabbitMQService, graphTriples[graph], _SPARQLEndpoint, _QueryParam, graph, _Username, _Password);
                }
            }

            //5º Limpiamos los blanknodes huerfanos, o que no tengan triples
            //TODO mover a una tarea que se ejecute continuamente
            //DeleteOrphanNodes(graphs);
        }

        /// <summary>
        /// Elimina los triples http://purl.org/roh/mirror/foaf#primaryTopic del RDF a cargar y los triples que tenían cagados en la BBDD
        /// </summary>
        /// <param name="pDataGraph">Grafo con los datos a cargar</param>
        /// <returns>Lista de grafos afectados</returns>
        private HashSet<string> RemovePrimaryTopics(ref RohGraph pDataGraph)
        {
            HashSet<string> graphs = new HashSet<string>();
            List<string> mainEntities = new List<string>();
            string query = @"select distinct * where{?s <http://purl.org/roh/mirror/foaf#primaryTopic> ""true""^^<http://www.w3.org/2001/XMLSchema#boolean>}";
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                mainEntities.Add(sparqlResult["s"].ToString());
            }

            //Se eliminan todas las referncias a las entidades principales
            if (mainEntities.Count > 0)
            {
                foreach (string mainEntity in mainEntities)
                {
                    graphs.UnionWith(DeleteUpdatedEntity(mainEntity));
                }
            }

            //Eliminamos el triple que marca las entidades principales para que no se inserte en la BBDD
            {
                TripleStore store = new TripleStore();
                store.Add(pDataGraph);
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
            return graphs;
        }

        /// <summary>
        /// Actualiza la propiedad http://www.w3.org/ns/prov#endedAtTime para las entidades a cargar
        /// </summary>
        /// <param name="pDataGraph">Grafo</param>
        /// <param name="pActivityEndedAtTime">Fecha de carga</param>
        private void UpdateEndedAtTime(ref RohGraph pDataGraph, DateTime pActivityEndedAtTime)
        {
            string endedAtTimeProperty = "http://www.w3.org/ns/prov#endedAtTime";

            //Eliminamos de la BBDD
            HashSet<string> subjects = new HashSet<string>();
            {
                SparqlResultSet sparqlResultSet = (SparqlResultSet)pDataGraph.ExecuteQuery("select distinct ?s where{?s ?p ?o. FILTER(isURI(?s))}");
                foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                {
                    string s = sparqlResult["s"].ToString();
                    subjects.Add(s);
                }
            }

            //Ejecutamos las eliminaciones de 100 en 100 y añadimos los triples al Grafo de carga 
            while (subjects.Count > 0)
            {
                List<string> deletes = new List<string>();
                foreach (string entityID in subjects.ToList())
                {
                    IUriNode t_subject = pDataGraph.CreateUriNode(UriFactory.Create(entityID));
                    IUriNode t_predicate = pDataGraph.CreateUriNode(UriFactory.Create(endedAtTimeProperty));
                    ILiteralNode t_object = pDataGraph.CreateLiteralNode(pActivityEndedAtTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                    pDataGraph.Assert(new Triple(t_subject, t_predicate, t_object));
                   
                    string stringDelete = $@"   {{
                                                                ?s ?p ?o. 
                                                                FILTER(?s = <{entityID}> AND ?p =<{endedAtTimeProperty}>)
                                                            }}";
                    deletes.Add(stringDelete);
                    subjects.Remove(entityID);
                    if (deletes.Count >= 100)
                    {
                        break;
                    }
                }
                string queryDeleteEndedAtTime = $@"    DELETE {{ ?s ?p ?o. }}
                                                                WHERE 
                                                                {{
                                                                    {{{string.Join("}UNION{", deletes)}}}
                                                                }}";
                _SparqlUtility.SelectData(_SPARQLEndpoint, _Graph, queryDeleteEndedAtTime, _QueryParam, _Username, _Password);
            }
        }

        /// <summary>
        /// Elimina todos los triples de la BBDD cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
        /// </summary>
        /// <param name="pOntologyGraph">Grafo con la ontología</param>
        /// <param name="pDataInferenceGraph">Grafo con los datos a cargar (con inferencia)</param>
        private void RemoveMonovaluatedProperties(RohGraph pOntologyGraph, RohGraph pDataInferenceGraph)
        {
            //1º Obtnemos las propiedades monovaluadas de las clases
            Dictionary<string, HashSet<string>> classMonovaluateProperty = new Dictionary<string, HashSet<string>>();

            SparqlResultSet sparqlResultSet2 = (SparqlResultSet)pOntologyGraph.ExecuteQuery(
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
                SparqlResultSet sparqlResultSet3 = (SparqlResultSet)pDataInferenceGraph.ExecuteQuery(
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
                _SparqlUtility.SelectData(_RabbitMQService,_SPARQLEndpoint, _Graph, queryDeleteMainEntities, _QueryParam, _Username, _Password);
            }
        }

        /// <summary>
        /// Elimina una entidad actualizada de la BBDD (no elimina los triples en los que es objeto)
        /// </summary>
        /// <param name="pEntity">Entida</param>
        /// <returns>Lista de grafos afectados</returns>
        private HashSet<string> DeleteUpdatedEntity(string pEntity)
        {
            //Obtenemos todos los grafos en los que está la entidad como sujeto para eliminar sus triples
            HashSet<string> listGraphs = new HashSet<string>();
            SparqlObject sparqlObjectGraphs = _SparqlUtility.SelectData(_RabbitMQService, _SPARQLEndpoint, "", $"select distinct ?g where{{graph ?g{{?s ?p ?o. FILTER( ?s in(<>,<{pEntity}>) )}}}}", _QueryParam, _Username, _Password);
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObjectGraphs.results.bindings)
            {
                listGraphs.Add(row["g"].value);
            }

            foreach (string graph in listGraphs)
            {
                string queryDeleteS = $@"DELETE {{ <{pEntity}> ?p ?o. }}
                                    WHERE 
                                    {{
                                        <{pEntity}> ?p ?o. 
                                    }}";
                _SparqlUtility.SelectData(_RabbitMQService, _SPARQLEndpoint, graph, queryDeleteS, _QueryParam, _Username, _Password);
            }
            return listGraphs;
        }

        /// <summary>
        /// Limpiamos los blanknodes huerfanos, o que no tengan triples (sólo rdftype)
        /// </summary>
        /// <param name="pGraphs">Lista de grafos en los que ejecutar</param>
        public void DeleteOrphanNodes(HashSet<string> pGraphs)
        {
            foreach (string graph in pGraphs)
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
                                            MINUS{{?x ?y ?s. FILTER(isblank(?s))}}
                                            MINUS{{?s ?p ?o. FILTER(!isblank(?s))}}
                                        }}";
                    if (_SparqlUtility.SelectData(_RabbitMQService, _SPARQLEndpoint, graph, queryASKOrphan, _QueryParam, _Username, _Password).boolean)
                    {
                        existeNodosHuerfanos = true;
                        string deleteOrphanNodes = $@"DELETE {{ ?s ?p ?o. }}
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            MINUS{{?x ?y ?s. FILTER(isblank(?s))}}
                                            MINUS{{?s ?p ?o. FILTER(!isblank(?s))}}
                                        }}";
                        _SparqlUtility.SelectData(_RabbitMQService, _SPARQLEndpoint, graph, deleteOrphanNodes, _QueryParam, _Username, _Password);
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
                    if (_SparqlUtility.SelectData(_RabbitMQService, _SPARQLEndpoint, graph, queryASKEmpty, _QueryParam, _Username, _Password).boolean)
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
                        _SparqlUtility.SelectData(_RabbitMQService, _SPARQLEndpoint, graph, deleteEmptyNodes, _QueryParam, _Username, _Password);
                    }
                }
            }
        }

        /// <summary>
        /// Agrega los SameAs hacia unidata para las entidades que no lo tengan creado
        /// </summary>
        /// <param name="pGraph">Grafo</param>
        /// <param name="pUnidataDomain">Uri para transformar las URLs de las entidades antes de cargar en Unidata</param>
        /// <param name="pUnidataUriTransform">Uri para transformar las URLs de las entidades antes de cargar en Unidata</param>
        /// <returns></returns>
        public static RohGraph CreateUnidataSameAs(RohGraph pGraph, string pUnidataDomain, string pUnidataUriTransform)
        {
            HashSet<string> entities = new HashSet<string>();
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pGraph.ExecuteQuery(@"select ?s where{?s a ?rdftype. FILTER(!isBlank(?rdftype)) FILTER(!isBlank(?s))}");
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                entities.Add(sparqlResult["s"].ToString());
            }

            RohGraph graph = pGraph.Clone();
            foreach (string entity in entities)
            {
                //Si no tiene un sameAs apuntando a Unidata lo creamos
                if (graph.Triples.ToList().Where(x => x.Subject.ToString() == entity && x.Predicate.ToString() == "http://www.w3.org/2002/07/owl#sameAs" && x.Object is UriNode && x.Object.ToString().StartsWith(pUnidataDomain)).Count() == 0)
                {
                    IUriNode t_subject = graph.CreateUriNode(UriFactory.Create(entity));
                    IUriNode t_predicate = graph.CreateUriNode(UriFactory.Create("http://www.w3.org/2002/07/owl#sameAs"));
                    Uri oldUri = new Uri(entity);
                    string uriUnidata = entity.Replace(oldUri.Scheme + "://" + oldUri.Host, pUnidataUriTransform);
                    IUriNode t_object = graph.CreateUriNode(UriFactory.Create(uriUnidata));
                    graph.Assert(new Triple(t_subject, t_predicate, t_object));
                }
            }
            return graph;
        }

        /// <summary>
        /// Prepara el grafo para su carga en Unidata, para ello coge las URIs de Unidata del SameAs y la aplica a los sujetos y los antiguos sujetos se agregan al SameAs
        /// </summary>
        /// <param name="pGraph">Grafo</param>
        /// <param name="pUnidataDomain">Dominio de Unidata</param>
        /// <param name="pUnidataUriTransform">Uri para transformar las URLs de las entidades antes de cargar en Unidata en caso de que no exisa la URI de Unidata</param>
        /// <returns></returns>
        public static RohGraph TransformUrisToUnidata(RohGraph pGraph, string pUnidataDomain, string pUnidataUriTransform)
        {
            HashSet<string> entities = new HashSet<string>();
            SparqlResultSet sparqlResultSet = (SparqlResultSet)pGraph.ExecuteQuery(@"select ?s where{?s a ?rdftype. FILTER(!isBlank(?rdftype)) FILTER(!isBlank(?s))}");
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                entities.Add(sparqlResult["s"].ToString());
            }

            RohGraph unidataGraph = pGraph.Clone();
            foreach (string entity in entities)
            {
                Uri oldUri = new Uri(entity);
                string uriUnidata = entity.Replace(oldUri.Scheme + "://" + oldUri.Host, pUnidataUriTransform);
                SparqlResultSet sparqlResultSetSameAs = (SparqlResultSet)unidataGraph.ExecuteQuery(@"select ?sameAs where{?s <http://www.w3.org/2002/07/owl#sameAs> ?sameAs. FILTER(?s=<"+entity+">)}");
                foreach (SparqlResult sparqlResult in sparqlResultSetSameAs.Results)
                {
                    if(sparqlResult["sameAs"].ToString().StartsWith(pUnidataDomain))
                    {
                        uriUnidata = sparqlResult["sameAs"].ToString();
                    }
                }

                
                TripleStore store = new TripleStore();
                store.Add(unidataGraph);
                LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                SparqlUpdateParser parser = new SparqlUpdateParser();

                #region 1º Cambiamos las URLs por las URLs de Unidata
                //Actualizamos los sujetos
                SparqlUpdateCommandSet updateSubject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                                INSERT{<" + uriUnidata + @"> ?p ?o.}
                                                                                WHERE 
                                                                                {
                                                                                    ?s ?p ?o.   FILTER(?s = <" + entity + @">)
                                                                                }");
                //Actualizamos los objetos
                SparqlUpdateCommandSet updateObject = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                                INSERT{?s ?p <" + uriUnidata + @">.}
                                                                                WHERE 
                                                                                {
                                                                                    ?s ?p ?o.   FILTER(?o = <" + entity + @">)
                                                                                }");
                processor.ProcessCommandSet(updateSubject);
                processor.ProcessCommandSet(updateObject);
                #endregion

                #region 2º Añadimos el sujeto actual como sameAs
                IUriNode t_subject = unidataGraph.CreateUriNode(UriFactory.Create(uriUnidata));
                IUriNode t_predicate = unidataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/2002/07/owl#sameAs"));
                IUriNode t_object = unidataGraph.CreateUriNode(UriFactory.Create(entity));
                unidataGraph.Assert(new Triple(t_subject, t_predicate, t_object));
                #endregion

                #region 3º Eliminamos el SameAs de Unidata
                //Eliminamos SameAs de Unidata
                SparqlUpdateCommandSet deleteUnidataSameAs = parser.ParseFromString(@"DELETE { ?s ?p ?o. }
                                                                                WHERE 
                                                                                {
                                                                                    ?s ?p ?o.   FILTER( ?p =<http://www.w3.org/2002/07/owl#sameAs>) FILTER( ?o = <" + uriUnidata + @">)
                                                                                }");

                processor.ProcessCommandSet(deleteUnidataSameAs);
                #endregion
            }
            return unidataGraph;
        }

    }
}
