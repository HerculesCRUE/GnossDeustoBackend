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

namespace API_DISCOVER.Utility
{
    public class AsioPublication
    {
        private string _SPARQLEndpoint { get; set; }
        private string _QueryParam { get; set; }
        private string _Graph { get; set; }

        private I_SparqlUtility _SparqlUtility { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pSparqlUtility">SPARQL Utility</param>
        /// <param name="pSPARQLEndpoint">SPARQL endpoint</param>
        /// <param name="pQueryParam">Parámetros para las queries</param>
        /// <param name="pGraph">Grafo de carga</param>
        public AsioPublication(I_SparqlUtility pSparqlUtility,string pSPARQLEndpoint, string pQueryParam, string pGraph)
        {
            _SPARQLEndpoint = pSPARQLEndpoint;
            _QueryParam = pQueryParam;
            _Graph = pGraph;
            _SparqlUtility = pSparqlUtility;
        }

        /// <summary>
        /// Publica un RDF en Asio aplicado todos losprocedimientos pertinentes
        /// </summary>
        /// <param name="dataGraph">Grafo con los datos a cargar</param>
        /// <param name="dataInferenceGraph">Grafo con los datos a cargar (con inferencia)</param>
        /// <param name="ontologyGraph">Grafo con la ontología</param>
        /// <param name="pAttributedTo">Sujeto y nombre para atribuir los triples de los apis externos</param>
        /// <param name="pActivityStartedAtTime">Inicio del proceso</param>
        /// <param name="pActivityEndedAtTime">Fin del proceso</param>
        /// <param name="externalIntegration">Datos extraídos de las integracinoes externas sujeto, propiedad, valor, grafos</param>
        public void PublishRDF(RohGraph dataGraph, RohGraph dataInferenceGraph, RohGraph ontologyGraph, KeyValuePair<string, string>? pAttributedTo, DateTime pActivityStartedAtTime, DateTime pActivityEndedAtTime, Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> externalIntegration)
        {
            // 1º Eliminamos de la BBD las entidades principales que aparecen en el RDF
            HashSet<string> graphs= RemovePrimaryTopics(ref dataGraph);
            graphs.Add(_Graph);

            // 2º Eliminamos todos los triples de la BBDD cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
            RemoveMonovaluatedProperties(ontologyGraph, dataInferenceGraph);

            //3º Insertamos los triples en la BBDD
            if (pAttributedTo.HasValue)
            {
                //Añadimos triples del softwareagent
                IUriNode t_subject = dataGraph.CreateUriNode(UriFactory.Create(pAttributedTo.Value.Key));
                IUriNode t_predicate_rdftype = dataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                IUriNode t_object_rdftype = dataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#SoftwareAgent"));
                dataGraph.Assert(new Triple(t_subject, t_predicate_rdftype, t_object_rdftype));
                IUriNode t_predicate_name = dataGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                ILiteralNode t_object_name = dataGraph.CreateLiteralNode(pAttributedTo.Value.Value, new Uri("http://www.w3.org/2001/XMLSchema#string"));
                dataGraph.Assert(new Triple(t_subject, t_predicate_name, t_object_name));
            }
            SparqlUtility.LoadTriples(SparqlUtility.GetTriplesFromGraph(dataGraph), _SPARQLEndpoint, _QueryParam, _Graph);

            //4º Insertamos los triples con provenance en la BBDD
            if (externalIntegration != null)
            {
                Dictionary<string, List<string>> graphDeletes = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> graphTriples = new Dictionary<string, List<string>>();
                foreach (string t_subject in externalIntegration.Keys)
                {
                    foreach (string t_property in externalIntegration[t_subject].Keys)
                    {
                        string t_object = externalIntegration[t_subject][t_property].Key;
                        HashSet<string> t_graphs = externalIntegration[t_subject][t_property].Value;
                        foreach (string graph in t_graphs)
                        {
                            if (!graphTriples.ContainsKey(graph))
                            {
                                graphTriples.Add(graph, new List<string>());
                            }
                            string bNodeid = "_:" + Guid.NewGuid().ToString();

                            graphTriples[graph].Add($@"<{t_subject}> <http://www.w3.org/ns/prov#wasUsedBy> {bNodeid} .");
                            graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/ns/prov#Activity> .");
                            graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <{t_property}>.");
                            graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> ""{ t_object.Replace("\"", "\\\"").Replace("\n", "\\n") }""^^<http://www.w3.org/2001/XMLSchema#string>.");
                            graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#startedAtTime> ""{ pActivityStartedAtTime }""^^<http://www.w3.org/2001/XMLSchema#datetime>.");
                            graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#endedAtTime> ""{ pActivityEndedAtTime }""^^<http://www.w3.org/2001/XMLSchema#datetime>.");
                            if (pAttributedTo.HasValue)
                            {
                                graphTriples[graph].Add($@"{bNodeid} <http://www.w3.org/ns/prov#wasAssociatedWith> <{pAttributedTo.Value.Key}>.");
                            }

                            if (!graphDeletes.ContainsKey(graph))
                            {
                                graphDeletes.Add(graph, new List<string>());
                            }


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

                //Eliminamos aquellos triples de provenance que ya estén cargados
                foreach (string graph in graphDeletes.Keys)
                {
                    graphs.Add(graph);
                    string queryDeleteProvenance = $@"  DELETE {{ ?s ?p ?o. }}
                                                        WHERE 
                                                        {{
                                                            {{{string.Join("}UNION{", graphDeletes[graph])}}}
                                                        }}";
                    _SparqlUtility.SelectData(_SPARQLEndpoint, graph, queryDeleteProvenance, _QueryParam);
                }

                //Cargamos los nuevos triples
                foreach (string graph in graphTriples.Keys)
                {
                    SparqlUtility.LoadTriples(graphTriples[graph], _SPARQLEndpoint, _QueryParam, graph);
                }
            }

            //5º Limpiamos los blanknodes huerfanos, o que no tengan triples
            DeleteOrphanNodes(graphs);
        }

        /// <summary>
        /// Elimina los triples http://purl.org/roh/mirror/foaf#primaryTopic del RDF a cargar y los triples que tenían cagados en la BBDD
        /// </summary>
        /// <param name="dataGraph">Grafo con los datos a cargar</param>
        /// <returns>Lista de grafos afectados</returns>
        private HashSet<string> RemovePrimaryTopics(ref RohGraph dataGraph)
        {
            HashSet<string> graphs = new HashSet<string>();
            List<string> mainEntities = new List<string>();
            string query = @"select distinct * where{?s <http://purl.org/roh/mirror/foaf#primaryTopic> ""true""^^<http://www.w3.org/2001/XMLSchema#boolean>}";
            SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery(query.ToString());
            foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
            {
                mainEntities.Add(sparqlResult["s"].ToString());
            }

            //Se eliminan todas las referncias a las entidades principales junto con sus blanknodes de forma recursiva
            if (mainEntities.Count > 0)
            {
                foreach (string mainEntity in mainEntities)
                {
                    graphs.UnionWith(DeleteEntity(mainEntity));
                }
            }

            //Eliminamos el triple que marca las entidades principales para que no se inserte en la BBDD
            {
                TripleStore store = new TripleStore();
                store.Add(dataGraph);
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
        /// Elimina todos los triples de la BBDD cuyo sujeto y predicado estén en el RDF a cargar y estén marcados como monovaluados.
        /// </summary>
        /// <param name="ontologyGraph">Grafo con la ontología</param>
        /// <param name="dataInferenceGraph">Grafo con los datos a cargar (con inferencia)</param>
        private void RemoveMonovaluatedProperties(RohGraph ontologyGraph, RohGraph dataInferenceGraph)
        {
            //1º Obtnemos las propiedades monovaluadas de las clases
            Dictionary<string, HashSet<string>> classMonovaluateProperty = new Dictionary<string, HashSet<string>>();

            SparqlResultSet sparqlResultSet2 = (SparqlResultSet)ontologyGraph.ExecuteQuery(
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
                SparqlResultSet sparqlResultSet3 = (SparqlResultSet)dataInferenceGraph.ExecuteQuery(
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
                _SparqlUtility.SelectData(_SPARQLEndpoint, _Graph, queryDeleteMainEntities, _QueryParam);
            }
        }

        /// <summary>
        /// Elimina una entidad de la BBDD (y sus blank nodes de forma recursiva)
        /// </summary>
        /// <param name="pEntity">Entida</param>
        /// <returns>Lista de grafos afectados</returns>
        private HashSet<string> DeleteEntity(string pEntity)
        {
            //Obtenemos todos los blanknodes a los que apunta la entidad para luego borrarlos
            HashSet<string> bnodeChildrens = new HashSet<string>();
            SparqlObject sparqlObject = _SparqlUtility.SelectData(_SPARQLEndpoint, _Graph, $"select distinct ?bnode where{{<{pEntity}> ?p ?bnode. FILTER(isblank(?bnode))}}", _QueryParam);
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {
                bnodeChildrens.Add(row["bnode"].value);
            }

            //Obtenemos todos los grafos en los que está la entidad para eliminar sus triples
            HashSet<string> listGraphs = new HashSet<string>();
            SparqlObject sparqlObjectGraphs = _SparqlUtility.SelectData(_SPARQLEndpoint, "", $"select distinct ?g where{{graph ?g{{?s ?p ?o. FILTER(?s=<{pEntity}> OR ?o=<{pEntity}>)}}}}", _QueryParam);
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
                _SparqlUtility.SelectData(_SPARQLEndpoint, graph, queryDeleteS, _QueryParam);
                string queryDeleteO = $@"DELETE {{ ?s ?p <{pEntity}>. }}
                                    WHERE 
                                    {{
                                        ?s ?p <{pEntity}>. 
                                    }}";
                _SparqlUtility.SelectData(_SPARQLEndpoint, graph, queryDeleteO, _QueryParam);
            }
            foreach (string bnode in bnodeChildrens)
            {
                DeleteEntity(bnode);
            }
            return listGraphs;
        }

        /// <summary>
        /// Limpiamos los blanknodes huerfanos, o que no tengan triples (sólo rdftype)
        /// </summary>
        /// <param name="graphs">Lista de grafos en los que ejecutar</param>
        private void DeleteOrphanNodes(HashSet<string> graphs)
        {
            foreach (string graph in graphs)
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
                    if (_SparqlUtility.SelectData(_SPARQLEndpoint, graph, queryASKOrphan, _QueryParam).boolean)
                    {
                        existeNodosHuerfanos = true;
                        string deleteOrphanNodes = $@"DELETE {{ ?s ?p ?o. }}
                                        WHERE 
                                        {{
                                            ?s ?p ?o.
                                            MINUS{{?x ?y ?s. FILTER(isblank(?s))}}
                                            MINUS{{?s ?p ?o. FILTER(!isblank(?s))}}
                                        }}";
                        _SparqlUtility.SelectData(_SPARQLEndpoint, graph, deleteOrphanNodes, _QueryParam);
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
                    if (_SparqlUtility.SelectData(_SPARQLEndpoint, graph, queryASKEmpty, _QueryParam).boolean)
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
                        _SparqlUtility.SelectData(_SPARQLEndpoint, graph, deleteEmptyNodes, _QueryParam);
                    }
                }
            }
        }
    }
}
