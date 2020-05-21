using API_CARGA.Models.Entities;
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
        /// Obtiene los triples de un RDF
        /// </summary>
        /// <param name="pXMLRDF">XML RDF</param>
        /// <returns>Lista de triples</returns>
        public static List<string> GetTriplesFromRDF(XmlDocument pXMLRDF)
        {
            RohGraph g = new RohGraph();
            g.LoadFromString(pXMLRDF.InnerXml, new RdfXmlParser());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            NTriplesWriter nTriplesWriter = new NTriplesWriter();
            nTriplesWriter.Save(g, sw);
            return sw.ToString().Split("\n").ToList().Select(x => Regex.Replace(
                    x,
                    @"\\u(?<Value>[a-zA-Z0-9]{4})",
                    m => {
                        return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                    })).ToList();
        }


        /// <summary>
        /// Valida un RDF
        /// </summary>
        /// <param name="pRdfFileContent">XML RDF</param>
        /// <param name="pShapesConfig">Lista de Shapes de validación</param>
        /// <returns>Lista de triples</returns>
        public static ShapeReport ValidateRDF(string pRdfFileContent, List<ShapeConfig> pShapesConfig)
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
        /// Carga los triples en un PARQL endpoint
        /// </summary>
        /// <param name="pTriples">Triples a inertar</param>
        /// <param name="pSPARQLEndpoint">Endpoint SPARQL</param>
        /// <param name="pQueryParam">Query param</param>
        /// <param name="pGraph">Grafo</param>
        public static void LoadTriples(List<string> pTriples, string pSPARQLEndpoint, string pQueryParam, string pGraph, string pGraphUnidata)
        {
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


                int maxTriples = 500;

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
                    InsertData(pSPARQLEndpoint, pGraphUnidata, triplesInsert, pQueryParam);
                }
            }


            //NotBlankNodes
            if (listNotBlankNodeTriples.Count > 0)
            {
                InsertData(pSPARQLEndpoint, pGraph, listNotBlankNodeTriples, pQueryParam);
                InsertData(pSPARQLEndpoint, pGraphUnidata, listNotBlankNodeTriples, pQueryParam);
            }
        }

        private static void InsertData(string pSPARQLEndpoint, string pGraph, List<string> triplesInsert,  string pQueryParam)
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
    }
}
