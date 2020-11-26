// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using API_DISCOVER.Models.Entities;

namespace API_DISCOVER.Utility
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
            return GetTriplesFromGraph(g);
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

        public static SparqlObject SelectData(string pSPARQLEndpoint, string pGraph, string pConsulta, string pQueryParam)
        {
            SparqlObject datosDBpedia = null;
            string urlConsulta = pSPARQLEndpoint;
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");


            NameValueCollection parametros = new NameValueCollection();
            parametros.Add("default-graph-uri", pGraph);
            parametros.Add(pQueryParam, pConsulta);
            parametros.Add("format", "application/sparql-results+json");
           
            byte[] responseArray = null;
            int numIntentos = 0;
            Exception exception = null;
            while (responseArray == null && numIntentos < 5)
            {
                numIntentos++;
                try
                {
                    responseArray = webClient.UploadValues(urlConsulta, "POST", parametros);
                    exception = null;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            if(exception!=null)
            {
                throw exception;
            }
            string jsonRespuesta = System.Text.Encoding.UTF8.GetString(responseArray);

            if (!string.IsNullOrEmpty(jsonRespuesta))
            {
                datosDBpedia = JsonConvert.DeserializeObject<SparqlObject>(jsonRespuesta);
            }
            return datosDBpedia;
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
