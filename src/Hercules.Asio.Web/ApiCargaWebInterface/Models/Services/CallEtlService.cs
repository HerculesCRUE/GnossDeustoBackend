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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VDS.RDF;

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
        RohGraph ontologia;
        string hash;

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
        /// Comprueba si la ontología ha cambiado. Si es así devuelve la nueva.
        /// </summary>
        /// <returns>La ontología actualizada</returns>
        public RohGraph CallGetOntology()
        {
            string response = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/getontologyhash", _token);
            if (response == hash)
            {
                return ontologia;
            }
            else
            {
                string response2 = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/getontology", _token);
                ontologia = new RohGraph();
                ontologia.LoadFromString(response2);
                hash = response;
                return ontologia;
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
        /// <param name="rdfFile">rdf a pasar</param>
        /// <param name="jobId">Identificador de la tarea</param>
        /// <param name="discoverProcessed">Indica si ya está procesado el descubrimiento</param>
        /// <param name="idDiscoverItem">Identificador del discoverItem, en caso de que se quiera actualizar</param>
        public void CallDataPublish(IFormFile rdfFile, string jobId, bool discoverProcessed, string idDiscoverItem = null)
        {
            string response = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/data-publish?jobId={jobId}&discoverProcessed={discoverProcessed}&idDiscoverItem={idDiscoverItem}", rdfFile, _token, true);
        }
        /// <summary>
        /// Llama al método del api de carga de getRecord
        /// </summary>
        /// <param name="repoIdentifier">Identificador del repositorio OAI-PMH</param>
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
        /// Llama al método del API ListMetadataFormats.
        /// </summary>
        /// <param name="identifierRepo">Identificador del repositorio OAI-PMH.</param>
        /// <returns></returns>
        public List<string> CallListMetadataFormats(Guid identifierRepo)
        {
            List<string> listMetadataFormats = new List<string>();
            string xml = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/ListMetadataFormats/{identifierRepo}", _token);
            XDocument respuestaXML = XDocument.Load(new StringReader(xml));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            XElement listIdentifierElement = respuestaXML.Root.Element(nameSpace + "ListMetadataFormats");
            IEnumerable<XElement> listMetadataFormat = listIdentifierElement.Descendants(nameSpace + "metadataFormat");
            foreach (var metadataFormat in listMetadataFormat)
            {
                listMetadataFormats.Add(metadataFormat.Element(nameSpace + "metadataPrefix").Value);
            }
            return listMetadataFormats;
        }
        /// <summary>
        /// Llama al método del API ListSets.
        /// </summary>
        /// <param name="identifierRepo">Identificador del repositorio OAI-PMH.</param>
        /// <returns></returns>
        public List<string> CallListSets(Guid identifierRepo)
        {
            List<string> listMetadataFormats = new List<string>();
            string xml = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/ListSets/{identifierRepo}", _token);
            XDocument respuestaXML = XDocument.Load(new StringReader(xml));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            XElement listIdentifierElement = respuestaXML.Root.Element(nameSpace + "ListSets");
            IEnumerable<XElement> listMetadataFormat = listIdentifierElement.Descendants(nameSpace + "set");
            foreach (var metadataFormat in listMetadataFormat)
            {
                listMetadataFormats.Add(metadataFormat.Element(nameSpace + "setSpec").Value);
            }
            return listMetadataFormats;
        }
        /// <summary>
        /// Obtiene la lista de identificadores.
        /// </summary>
        /// <returns></returns>
        public string CallGetListIdentifiers(Guid identifierRepo, string metadataPrefix, string set, string from, string until)
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/ListIdentifiers/{identifierRepo}?metadataPrefix={metadataPrefix}&&set={set}&&from={from}&&until={until}", _token);
            return result;
        }
        /// <summary>
        /// Sube una ontologia
        /// </summary>
        /// <param name="ontologyUri">Ontologia a subir</param>
        public void PostOntology(IFormFile ontologyUri)
        {
            _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/load-ontology", ontologyUri, _token, true, "ontology");
        }

        /// <summary>
        /// Obtiene una ontologia
        /// </summary>
        /// <returns></returns>
        public string GetOntology()
        {
            string result = _serviceApi.CallGetApi(_serviceUrl.GetUrl(), $"etl/GetOntology", _token);
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
            if(validationRdf != null)
            {
                CallDataValidatePersonalized(rdfToValidate, validationRdf);
            }
            if(validationShapesList!=null)
            {
                foreach (Guid idShape in validationShapesList)
                {
                    string shape = repositoryShapes.Where(item => item.ShapeConfigID.Equals(idShape)).Select(item => item.Shape).FirstOrDefault();
                    if (!string.IsNullOrEmpty(shape))
                    {
                        var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(shape));
                        IFormFile fileValidation = new FormFile(content, 0, content.Length, "validationShape", "validationShape.rdf");
                        CallDataValidatePersonalized(rdfToValidate, fileValidation);
                    }
                }
            }
        }

        public string CallRemover(string pEntity)
        {
            string result = _serviceApi.CallPostApi(_serviceUrl.GetUrl(), $"etl/data-delete?pEntityUrl={pEntity}", null, _token);
            return result;
        }
    }
}
