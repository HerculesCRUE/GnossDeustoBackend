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
                

        public static void LoadOntology(string pSPARQLEndpoint, string pGraph, string pOntologyURL, string pQueryParam)
        {
            string query = "";
            query += $" sparql clear graph <{pGraph}>";
            

            string url = pSPARQLEndpoint;
            //if (string.IsNullOrEmpty(url))
            //{
            //    Graph graph = new Graph(); 
            //    graph.LoadFromUri(pOntologyURL,);
            //}
            if (!string.IsNullOrEmpty(url))
            {
                NameValueCollection parametros = new NameValueCollection();
                parametros.Add(pQueryParam, query);
                WebClient webClient = new WebClient();
                try
                {
                    webClient.UploadValues(url, "POST", parametros);
                    query = "";
                    query += $"sparql load {pOntologyURL} into {pGraph}";
                    parametros = new NameValueCollection();
                    parametros.Add(pQueryParam, query);
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
