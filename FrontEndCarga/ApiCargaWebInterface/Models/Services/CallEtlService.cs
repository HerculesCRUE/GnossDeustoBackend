// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para llamar a los métodos que ofrece el controlador etl del API_CARGA 
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallEtlService : ICallEtlService
    {
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        readonly ICallService _serviceApi;
        public CallEtlService(ICallService serviceApi, CallTokenService tokenService, ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCarga();
            }
        }

        public void CallDataValidate(IFormFile rdf, Guid repositoryIdentifier)
        {
            string response = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/data-validate?repositoryIdentifier={ repositoryIdentifier.ToString()} ", rdf , _token,true);
            ShapeReportModel shapeReport = JsonConvert.DeserializeObject<ShapeReportModel>(response);
            if (!shapeReport.conforms && shapeReport.severity == "http://www.w3.org/ns/shacl#Violation")
            {
                throw new ValidationException(shapeReport);
            }
        }

        public void CallDataValidatePersonalized(IFormFile rdfToValidate, IFormFile validationRDF)
        {
            Dictionary<string, IFormFile> fileList = new Dictionary<string, IFormFile>();
            fileList.Add("rdfFile", rdfToValidate);
            fileList.Add("validationFile", validationRDF);
            string response = _serviceApi.CallPostApiFiles(_serviceUrl.GetUrl(), $"etl/data-validate-personalized", fileList, _token);
            ShapeReportModel shapeReport = JsonConvert.DeserializeObject<ShapeReportModel>(response);
            if (!shapeReport.conforms && shapeReport.severity == "http://www.w3.org/ns/shacl#Violation")
            {
                throw new ValidationException(shapeReport);
            }
        }

        public void CallDataPublish(IFormFile rdf)
        {
            string response = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/data-publish", rdf, _token, true);
        }
        public string CallGetRecord(Guid repoIdentifier, string identifier, string type)
        {
            string respuesta = _serviceApi.CallGetApi(_serviceUrl.GetUrl(),$"etl/GetRecord/{repoIdentifier}?identifier={identifier}&&metadataPrefix=rdf", _token);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string rdf = respuestaXML.Root.Element(nameSpace + "GetRecord").Descendants(nameSpace + "metadata").First().FirstNode.ToString();
            return rdf;
        }

        public void PostOntology(IFormFile ontology, int ontologyType)
        {
            _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/load-ontology?ontologyType={ontologyType}", ontology, _token, true, "ontology");
        }

        public string GetOntology(int ontologyType)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/GetOntology?ontology={ontologyType}", _token);
            return result;
        }

        public void ValidateRDFPersonalized(Guid repositoryId, IFormFile rdfToValidate, IFormFile validationRdf, List<Guid> validationShapesList, List<ShapeConfigViewModel> repositoryShapes)
        {
            if (validationRdf == null && (validationShapesList != null && repositoryShapes.Select(item => item.ShapeConfigID).SequenceEqual(validationShapesList)))
            {
                CallDataValidate(rdfToValidate, repositoryId);
            }
            else if ((validationShapesList == null || validationShapesList.Count == 0) && validationRdf != null)
            {
                CallDataValidatePersonalized(rdfToValidate, validationRdf);
            }
            else if (validationShapesList != null && repositoryShapes.Select(item => item.ShapeConfigID).SequenceEqual(validationShapesList) && validationRdf != null)
            {
                CallDataValidatePersonalized(rdfToValidate, validationRdf);
                CallDataValidate(rdfToValidate, repositoryId);
            }
            else if (validationRdf != null && validationShapesList != null && validationShapesList.Count > 0)
            {
                CallDataValidatePersonalized(rdfToValidate, validationRdf);
                foreach (Guid idShape in validationShapesList)
                {
                    string shape = repositoryShapes.Where(item => item.ShapeConfigID.Equals(idShape)).Select(item => item.Shape).FirstOrDefault();
                    if (!string.IsNullOrEmpty(shape))
                    {
                        var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(shape));
                        IFormFile fileValidation = new FormFile(content, 0, content.Length,"validationShape", "validationShape.rdf");
                        CallDataValidatePersonalized(rdfToValidate, fileValidation);
                    }
                }
            }
        }
    }
}
