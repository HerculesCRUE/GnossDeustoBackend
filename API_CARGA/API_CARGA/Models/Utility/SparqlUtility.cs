// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;
using VDS.RDF.Shacl;
using VDS.RDF.Shacl.Validation;
using VDS.RDF.Writing;

namespace API_CARGA.Models.Utility
{
    public static class SparqlUtility
    {
        public static XmlDocument GetRDFFromFile(IFormFile pRDFFile)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(pRDFFile.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            string xml = result.ToString();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc;
        }

        public static string GetTextFromFile(IFormFile pRDFFile)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(pRDFFile.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            return result.ToString();
        }


        /// <summary>
        /// Valida un RDF
        /// </summary>
        /// <param name="pRdfFileContent">XML RDF</param>
        /// <param name="pShapesConfig">Lista de Shapes de validación</param>
        /// <param name="pOntologyGraph">Grafo con la ontología</param>
        /// <returns>Lista de triples</returns>
        public static ShapeReport ValidateRDF(string pRdfFileContent, List<ShapeConfig> pShapesConfig, RohGraph pOntologyGraph)
        {
            //Cargamos datos a validar
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(pRdfFileContent, new RdfXmlParser());

            //Aplicamos inferencias de la ontologia
            RohRdfsReasoner reasoner = new RohRdfsReasoner();
            reasoner.Initialise(pOntologyGraph);
            reasoner.Apply(dataGraph);

            ShapeReport response = new ShapeReport();
            response.conforms = true;
            response.results = new List<ShapeReport.Result>();
            foreach (ShapeConfig shape in pShapesConfig)
            {
                IGraph shapeGraph = new Graph();
                shapeGraph.LoadFromString(shape.Shape);
                ShapesGraph shapesGraph = new ShapesGraph(shapeGraph);

                Report report = shapesGraph.Validate(dataGraph);
                if (!report.Conforms)
                {
                    response.conforms = false;
                    response.results.AddRange(report.Results.ToList().Select(x => new ShapeReport.Result()
                    {
                        severity = (x.Severity != null) ? x.Severity.ToString() : null,
                        focusNode = (x.FocusNode != null) ? x.FocusNode.ToString() : null,
                        resultValue = (x.ResultValue != null) ? x.ResultValue.ToString() : null,
                        message = (x.Message != null) ? x.Message.ToString() : null,
                        resultPath = (x.ResultPath != null) ? x.ResultPath.ToString() : null,
                        shapeID = shape.ShapeConfigID,
                        shapeName = shape.Name,
                        sourceShape = (x.SourceShape != null) ? x.SourceShape.ToString() : null,
                    }).ToList());
                }
            }

            if (response.results.Exists(x => x.severity == "http://www.w3.org/ns/shacl#Violation"))
            {
                response.severity = "http://www.w3.org/ns/shacl#Violation";
            }
            else if (response.results.Exists(x => x.severity == "http://www.w3.org/ns/shacl#Warning"))
            {
                response.severity = "http://www.w3.org/ns/shacl#Warning";
            }
            else if (response.results.Exists(x => x.severity == "http://www.w3.org/ns/shacl#Info"))
            {
                response.severity = "http://www.w3.org/ns/shacl#Info";
            }
            return response;
        }

        /// <summary>
        /// Valida un RDF
        /// </summary>
        /// <param name="pRdfFileContent">XML RDF</param>
        /// <param name="pValidation">Validación a realizar</param>
        /// <param name="pValidatoinFileName">Nombre del fichero de validación</param>
        /// <returns>Lista de triples</returns>
        public static ShapeReport ValidateRDF(string pRdfFileContent, string pValidation, string pValidatoinFileName)
        {
            //Cargamos la ontología
            RohGraph ontologyGraph = new RohGraph();
            ontologyGraph.LoadFromFile("Config/Ontology/roh-v2.owl");

            //Cargamos datos a validar
            RohGraph dataGraph = new RohGraph();
            dataGraph.LoadFromString(pRdfFileContent, new RdfXmlParser());

            //Aplicamos inferencias de la ontologia
            RohRdfsReasoner reasoner = new RohRdfsReasoner();
            reasoner.Initialise(ontologyGraph);
            reasoner.Apply(dataGraph);

            ShapeReport response = new ShapeReport();
            response.conforms = true;
            response.results = new List<ShapeReport.Result>();

            IGraph shapeGraph = new Graph();
            shapeGraph.LoadFromString(pValidation);
            ShapesGraph shapesGraph = new ShapesGraph(shapeGraph);

            Report report = shapesGraph.Validate(dataGraph);
            if (!report.Conforms)
            {
                response.conforms = false;
                response.results.AddRange(report.Results.ToList().Select(x => new ShapeReport.Result()
                {
                    severity = (x.Severity != null) ? x.Severity.ToString() : null,
                    focusNode = (x.FocusNode != null) ? x.FocusNode.ToString() : null,
                    resultValue = (x.ResultValue != null) ? x.ResultValue.ToString() : null,
                    message = (x.Message != null) ? x.Message.ToString() : null,
                    resultPath = (x.ResultPath != null) ? x.ResultPath.ToString() : null,
                    shapeID = Guid.Empty,
                    shapeName = $"{pValidatoinFileName}",
                    sourceShape = (x.SourceShape != null) ? x.SourceShape.ToString() : null,
                }).ToList());
            }


            if (response.results.Exists(x => x.severity == "http://www.w3.org/ns/shacl#Violation"))
            {
                response.severity = "http://www.w3.org/ns/shacl#Violation";
            }
            else if (response.results.Exists(x => x.severity == "http://www.w3.org/ns/shacl#Warning"))
            {
                response.severity = "http://www.w3.org/ns/shacl#Warning";
            }
            else if (response.results.Exists(x => x.severity == "http://www.w3.org/ns/shacl#Info"))
            {
                response.severity = "http://www.w3.org/ns/shacl#Info";
            }
            return response;
        }

        /// <summary>
        /// Carga una ontología en un SPARQL endpoint
        /// </summary>
        /// <param name="pOntology">Ontología</param>
        /// <param name="pSPARQLEndpoint">Endpoint SPARQL</param>
        /// <param name="pQueryParam">Query param</param>
        /// <param name="pGraph">Grafo</param>
        public static void LoadOntology(RohGraph pOntology, string pSPARQLEndpoint, string pQueryParam, string pGraph)
        {
            //Eliminamos los datos anteriores
            string query = "";
            query += $" clear graph <{pGraph}>";

            string url = pSPARQLEndpoint;
            NameValueCollection parametros = new NameValueCollection();
            parametros.Add(pQueryParam, query);
            WebClient webClient = new WebClient();
            try
            {
                webClient.UploadValues(url, "POST", parametros);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new Exception(response);
                }
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                webClient.Dispose();
            }

            //Cargamos la ontología
            SparqlUtility.LoadTriples(SparqlUtility.GetTriplesFromGraph(pOntology), pSPARQLEndpoint, pQueryParam, pGraph);
        }

        /// <summary>
        /// Carga los triples en un PARQL endpoint
        /// </summary>
        /// <param name="pTriples">Triples a inertar</param>
        /// <param name="pSPARQLEndpoint">Endpoint SPARQL</param>
        /// <param name="pQueryParam">Query param</param>
        /// <param name="pGraph">Grafo</param>
        public static void LoadTriples(List<string> pTriples, string pSPARQLEndpoint, string pQueryParam, string pGraph)
        {
            int maxTriples = 500;

            List<string> listNotBlankNodeTriples = new List<string>();
            List<string> listBlankNodeTriples = new List<string>();
            foreach (string triple in pTriples)
            {
                string[] tripleSplit = triple.Split();
                if (tripleSplit.Count() >= 4 && (tripleSplit[0].StartsWith("_:") || tripleSplit[2].StartsWith("_:")))
                {
                    listBlankNodeTriples.Add(triple);
                }
                else
                {
                    listNotBlankNodeTriples.Add(triple);
                }
            }

            //NotBlankNodes
            if (listNotBlankNodeTriples.Count > 0)
            {
                List<List<string>> listaListasTriples = SplitList(listNotBlankNodeTriples, maxTriples).ToList();
                foreach (List<string> listaTriples in listaListasTriples)
                {
                    InsertData(pSPARQLEndpoint, pGraph, listaTriples, pQueryParam);
                }
            }

            //BlankNodes
            if (listBlankNodeTriples.Count > 0)
            {
                Dictionary<string, HashSet<int>> blankNodeTriples = new Dictionary<string, HashSet<int>>();
                for (int i = 0; i < listBlankNodeTriples.Count; i++)
                {
                    string[] tripleSplit = listBlankNodeTriples[i].Split();
                    if (tripleSplit[0].StartsWith("_:"))
                    {
                        if (!blankNodeTriples.ContainsKey(tripleSplit[0]))
                        {
                            blankNodeTriples.Add(tripleSplit[0], new HashSet<int>());
                        }
                        blankNodeTriples[tripleSplit[0]].Add(i);
                    }
                    if (tripleSplit[2].StartsWith("_:"))
                    {
                        if (!blankNodeTriples.ContainsKey(tripleSplit[2]))
                        {
                            blankNodeTriples.Add(tripleSplit[2], new HashSet<int>());
                        }
                        blankNodeTriples[tripleSplit[2]].Add(i);
                    }
                }

                List<HashSet<int>> list = new List<HashSet<int>>();
                HashSet<int> listTriples = new HashSet<int>();
                foreach (string blankNode in blankNodeTriples.Keys.ToList())
                {
                    HashSet<int> triples = new HashSet<int>(blankNodeTriples[blankNode]);
                    bool added = true;
                    while (added)
                    {
                        added = false;
                        foreach (int triple in triples.ToList())
                        {
                            foreach (HashSet<int> triplesAux in blankNodeTriples.Where(x => x.Value.Contains(triple)).ToList().Select(x => x.Value).ToList())
                            {
                                int numPrev = triples.Count;
                                triples.UnionWith(triplesAux);
                                if (triples.Count > numPrev)
                                {
                                    added = true;
                                }
                            }
                        }
                    }
                    if (triples.Count > 0)
                    {
                        if (listTriples.Count == 0 && triples.Count > maxTriples)
                        {
                            throw new Exception("No se puden insertar " + triples.Count + " triples simultáneos con blank nodes");
                        }
                        else if ((listTriples.Count + triples.Count) < maxTriples)
                        {
                            listTriples.UnionWith(triples);
                        }
                        else
                        {
                            list.Add(listTriples);
                            listTriples = new HashSet<int>();
                            listTriples.UnionWith(triples);
                        }
                        foreach (string blankNodeAux in blankNodeTriples.Keys.ToList())
                        {
                            blankNodeTriples[blankNodeAux].ExceptWith(triples);
                        }
                    }
                }
                if (listTriples.Count > 0)
                {
                    list.Add(listTriples);
                }


                foreach (HashSet<int> sublist in list)
                {
                    List<string> triplesInsert = new List<string>();
                    foreach (int i in sublist)
                    {
                        triplesInsert.Add(listBlankNodeTriples[i]);
                    }
                    InsertData(pSPARQLEndpoint, pGraph, triplesInsert, pQueryParam);
                }
            }
        }

        private static void InsertData(string pSPARQLEndpoint, string pGraph, List<string> triplesInsert, string pQueryParam)
        {
            string query = "";
            query += $" INSERT INTO <{pGraph}>";
            query += " { ";
            query += string.Join(" ", triplesInsert);
            query += " } ";

            string url = pSPARQLEndpoint;
            if (string.IsNullOrEmpty(url))
            {
                Graph graph = new Graph();
                graph.LoadFromString(string.Join(" ", triplesInsert));
            }
            else
            {
                NameValueCollection parametros = new NameValueCollection();
                parametros.Add(pQueryParam, query);
                WebClient webClient = new WebClient();
                try
                {
                    webClient.UploadValues(url, "POST", parametros);
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        string response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        throw new Exception(response);
                    }
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    webClient.Dispose();
                }
            }
        }


        /// <summary>
        /// Obtiene los triples de un RDF
        /// </summary>
        /// <param name="pXMLRDF">XML RDF</param>
        /// <returns>Lista de triples</returns>
        public static List<string> GetTriplesFromGraph(RohGraph pGraph)
        {
            List<string> triples = new List<string>();
            foreach (Triple triple in pGraph.Triples)
            {
                string tripleString = "";
                if (triple.Subject is BlankNode)
                {
                    tripleString += triple.Subject.ToString();
                }
                else if (triple.Subject is UriNode)
                {
                    tripleString += "<" + triple.Subject.ToString() + ">";
                }

                tripleString += " <" + ((UriNode)triple.Predicate).Uri.ToString() + ">";

                if (triple.Object is LiteralNode)
                {
                    Uri datatype = ((LiteralNode)triple.Object).DataType;
                    string lang = ((LiteralNode)triple.Object).Language;
                    if (datatype != null)
                    {
                        tripleString += " \"" + ((LiteralNode)triple.Object).Value.Replace("\"", "\\\"").Replace("\n", "\\n") + "\"^^<" + datatype + ">";
                    }
                    else
                    {
                        tripleString += " \"" + ((LiteralNode)triple.Object).Value.Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
                    }
                    if (!string.IsNullOrEmpty(lang))
                    {
                        tripleString += "@" + lang;
                    }
                }
                else if (triple.Object is BlankNode)
                {
                    tripleString += " " + triple.Object.ToString();
                }
                else if (triple.Object is UriNode)
                {
                    tripleString += " <" + triple.Object.ToString() + ">";
                }
                tripleString += " .\r";
                triples.Add(tripleString);
            }

            return triples;
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
    }
}
