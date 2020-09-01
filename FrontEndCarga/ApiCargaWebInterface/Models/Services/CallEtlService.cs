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
    /// <summary>
    /// Clase para llamar a los métodos que ofrece el controlador etl del API_CARGA 
    /// </summary>
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

        /// <summary>
        /// Valida un rdf
        /// </summary>
        /// <param name="rdf">Rdf a validar</param>
        /// <param name="repositoryIdentifier">Repositorio en el que están configurados los shapes para validar</param>
        public void CallDataValidate(IFormFile rdf, Guid repositoryIdentifier)
        {
            string response = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/data-validate?repositoryIdentifier={ repositoryIdentifier.ToString()} ", rdf , _token,true);
            ShapeReportModel shapeReport = JsonConvert.DeserializeObject<ShapeReportModel>(response);
            if (!shapeReport.conforms && shapeReport.severity == "http://www.w3.org/ns/shacl#Violation")
            {
                throw new ValidationException(shapeReport);
            }
        }

        /// <summary>
        /// Valida un rdf
        /// </summary>
        /// <param name="rdfToValidate">RDF a validar</param>
        /// <param name="validationRDF">Validación a pasar</param>
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

        /// <summary>
        /// Llama al método del api de carga de publicación
        /// </summary>
        /// <param name="rdf">rdf a pasar</param>
        public void CallDataPublish(IFormFile rdf)
        {
            string response = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/data-publish", rdf, _token, true);
        }
        /// <summary>
        /// Llama al método del api de carga de getRecord
        /// </summary>
        /// <param name="repoIdentifier">Identificador del repositorio OAI-PMH </param>
        /// <param name="identifier">Identificador de la entidad a recolectar (Los identificadores se obtienen con el metodo /etl/ListIdentifiers/{repositoryIdentifier}).</param>
        /// <param name="type">metadata que se desea recuperar (rdf). Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud /etl/ListMetadataFormats/{repositoryIdentifier}.</param>
        /// <returns></returns>
        public string CallGetRecord(Guid repoIdentifier, string identifier, string type)
        {
            string respuesta = _serviceApi.CallGetApi(_serviceUrl.GetUrl(),$"etl/GetRecord/{repoIdentifier}?identifier={identifier}&&metadataPrefix=rdf", _token);
            XDocument respuestaXML = XDocument.Parse(respuesta);
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string rdf = respuestaXML.Root.Element(nameSpace + "GetRecord").Descendants(nameSpace + "metadata").First().FirstNode.ToString();
            return rdf;
        }
        /// <summary>
        /// Sube una ontologia
        /// </summary>
        /// <param name="ontology">Ontologia a subir</param>
        /// <param name="ontologyType">tipo de ontologia; siendo el 0 la ontología roh, el 1 la ontología rohes y el 2 la ontología rohum </param>
        public void PostOntology(IFormFile ontology, int ontologyType)
        {
            _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/load-ontology?ontologyType={ontologyType}", ontology, _token, true, "ontology");
        }

        /// <summary>
        /// Obtiene una ontologia
        /// </summary>
        /// <param name="ontologyType">tipo de ontologia; siendo el 0 la ontología roh, el 1 la ontología rohes y el 2 la ontología rohum </param>
        /// <returns></returns>
        public string GetOntology(int ontologyType)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/GetOntology?ontology={ontologyType}", _token);
            return result;
        }

        /// <summary>
        /// Valida un rdf tanto con un rdf de validación personalizado como por una lista de shapes configuradas en el repositorio
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio OAIPMH</param>
        /// <param name="rdfToValidate">RDF a validar</param>
        /// <param name="validationRdf">RDF de validación</param>
        /// <param name="validationShapesList">Lista de shapes de validación</param>
        /// <param name="repositoryShapes">Lista de shapes configuradas en el repositorio</param>
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
