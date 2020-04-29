using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VDS.RDF;
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

        /// <summary>
        /// Obtiene los triples de un RDF
        /// </summary>
        /// <param name="pXMLRDF">XML RDF</param>
        /// <returns>Lista de triples</returns>
        public static List<string> GetTriplesFromRDF(XmlDocument pXMLRDF)
        {
            IGraph g = new Graph();
            g.LoadFromString(pXMLRDF.InnerXml);

            System.IO.StringWriter sw = new System.IO.StringWriter();
            NTriplesWriter nTriplesWriter = new NTriplesWriter();
            nTriplesWriter.Save(g, sw);

            return sw.ToString().Split("\r\n").ToList();
        }


        /// <summary>
        /// Valida un RDF
        /// </summary>
        /// <param name="pXMLRDF">XML RDF</param>
        /// <param name="pShapesConfig">Lista de Shapes de validación</param>
        /// <returns>Lista de triples</returns>
        public static ShapeReport ValidateRDF(XmlDocument pXMLRDF,List<ShapeConfig> pShapesConfig)
        {
            //IGraph shapeGraph = new Graph();
            //shapeGraph.LoadFromString(File.ReadAllText("c:\\cargas\\shapes_prado.ttl"));

            IGraph dataGraph = new Graph();
            dataGraph.LoadFromString(pXMLRDF.InnerXml);

            ShapeReport response = new ShapeReport();
            response.conforms = true;
            response.results = new List<ShapeReport.Result>();
            foreach (ShapeConfig shape in pShapesConfig)
            {
                IGraph shapeGraph = new Graph();
                //shapeGraph.LoadFromString(shape.Shape);
                shapeGraph.LoadFromString(File.ReadAllText("c:\\cargas\\shapes_prado.ttl"));
                ShapesGraph shapesGraph = new ShapesGraph(shapeGraph);
                
                Report report = shapesGraph.Validate(dataGraph);
                if(!report.Conforms)
                {
                    response.conforms = false;
                    response.results.AddRange(report.Results.ToList().Select(x => new ShapeReport.Result()
                    {
                        severity = (x.Severity != null) ? x.Severity.ToString() : null,
                        focusNode = (x.FocusNode != null) ? x.FocusNode.ToString() : null,
                        resultValue = (x.ResultValue != null) ? x.ResultValue.ToString() : null,
                        message = (x.Message != null) ? x.Message.ToString() : null,
                        resultPath = (x.ResultPath != null) ? x.ResultPath.ToString() : null
                    }).ToList());
                }
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
        public static void LoadTriples(List<string> pTriples, string pSPARQLEndpoint, string pQueryParam, string pGraph)
        {
            int maxTriples = 500;

            List<List<string>> list = new List<List<string>>();
            for (int i = 0; i < pTriples.Count; i += maxTriples)
            {
                list.Add(pTriples.GetRange(i, Math.Min(maxTriples, pTriples.Count - i)));
            }

            foreach (List<string> sublist in list)
            {
                string query = "";
                query += $" INSERT INTO <{pGraph}>";
                query += " { ";
                query += string.Join(" ", sublist);
                query += " } ";

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
            }
        }
    }
}
