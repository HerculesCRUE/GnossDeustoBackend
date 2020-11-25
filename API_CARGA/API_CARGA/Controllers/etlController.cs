// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using API_CARGA.Models.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_CARGA.Controllers
{
    /// <summary>
    /// Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]

    public class etlController : Controller
    {
        private IRepositoriesConfigService _repositoriesConfigService;
        private DiscoverItemBDService _discoverItemService;
        private IShapesConfigService _shapeConfigService;
        readonly ConfigSparql _configSparql;
        readonly CallUri _callUri;
        readonly ConfigUrlService _configUrlService;
        readonly IRabbitMQService _amqpService;

        public etlController(DiscoverItemBDService iIDiscoverItemService, IRepositoriesConfigService iRepositoriesConfigService, IShapesConfigService iShapeConfigService, ConfigSparql configSparql, CallUri callUri, ConfigUrlService configUrlService, IRabbitMQService amqpService)
        {
            _discoverItemService = iIDiscoverItemService;
            _repositoriesConfigService = iRepositoriesConfigService;
            _shapeConfigService = iShapeConfigService;
            _configSparql = configSparql;
            _callUri = callUri;
            _configUrlService = configUrlService;
            _amqpService = amqpService;
        }

        /// <summary>
        /// Ejecuta el penúltimo paso del proceso de carga, por el que el RDF generado se encola en una cola de Rabbit MQ para que posteriormente el servicio de descubimiento lo procese y lo almacene en el Triple Store. Permite cargar una fuente RDF arbitraria.
        /// Aquí se encuentra un RDF de Ejemplo: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_CARGA/API_CARGA/Samples/rdfSample.xml
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <param name="jobId">Identificador de la tarea</param>
        /// <param name="discoverProcessed">Indica si ya está procesado el descubrimiento</param>
        /// <returns></returns>
        [HttpPost("data-publish")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataPublish(IFormFile rdfFile, string jobId, bool discoverProcessed)
        {
            try
            {
                string idDiscoverItem = Request.Query["idDiscoverItem"].ToString();
                if (!string.IsNullOrEmpty(idDiscoverItem))
                {
                    //Si viene el parametro 'idDiscoverItem' actualizamos un DiscoverItem ya existente.
                    XmlDocument rdf = SparqlUtility.GetRDFFromFile(rdfFile);

                    DiscoverItem discoverItem = _discoverItemService.GetDiscoverItemById(new Guid(idDiscoverItem));
                    discoverItem.DissambiguationProcessed = discoverProcessed;
                    discoverItem.Publish = true;
                    discoverItem.Status = "Pending";
                    discoverItem.DiscoverRdf = rdf.InnerXml;

                    _amqpService.PublishMessage(idDiscoverItem);
                    return Ok();
                }
                else
                {
                    XmlDocument rdf = SparqlUtility.GetRDFFromFile(rdfFile);
                    DiscoverItem discoverItem = new DiscoverItem() { JobID = jobId, Rdf = rdf.InnerXml, Publish = true, DissambiguationProcessed = discoverProcessed, Status = "Pending" };
                    Guid addedID = _discoverItemService.AddDiscoverItem(discoverItem);
                    _amqpService.PublishMessage(addedID);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>
        /// Valida un RDF mediante el shape SHACL configurado
        /// Aquí se encuentra un RDF de Ejemplo: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_CARGA/API_CARGA/Samples/rdfSample.xml
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio para seleccionar los Shapes (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns></returns>
        [HttpPost("data-validate")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ShapeReport))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataValidate(IFormFile rdfFile, Guid repositoryIdentifier)
        {
            try
            {
                string rdfFileContent = SparqlUtility.GetTextFromFile(rdfFile);
                return Ok(SparqlUtility.ValidateRDF(rdfFileContent, _shapeConfigService.GetShapesConfigs().FindAll(x => x.RepositoryID == repositoryIdentifier)));
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }

        }

        /// <summary>
        /// Valida un RDF mediante el fichero de validación pasado
        /// Aquí se encuentra un RDF de Ejemplo: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_CARGA/API_CARGA/Samples/rdfSample.xml
        /// </summary>
        /// <param name="rdfFile">Fichero RDF a validar</param>
        /// <param name="validationFile">Fichero de validación</param>
        /// <returns></returns>
        [HttpPost("data-validate-personalized")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ShapeReport))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataValidate(IFormFile rdfFile, IFormFile validationFile)
        {
            try
            {
                string rdfFileContent = SparqlUtility.GetTextFromFile(rdfFile);
                string validation = SparqlUtility.GetTextFromFile(validationFile);
                return Ok(SparqlUtility.ValidateRDF(rdfFileContent, validation, validationFile.FileName));
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }

        }

        /// <summary>
        /// Elimina la ontologia cargada y la reemplaza por la nueva
        /// </summary>
        /// <param name="ontology">Fichero de la nueva ontologia</param>
        /// <returns></returns>
        [HttpPost("load-ontology")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult LoadOntology(IFormFile ontology)
        {
            try
            {
                OntologyService.SetOntology(ontology);
                string ontologyGraph = "";
                ontologyGraph = _configSparql.GetGraphRoh();
                
                SparqlUtility.LoadOntology(_configSparql.GetEndpoint(), ontologyGraph, $"{_configUrlService.GetUrl()}/etl/GetOntology", _configSparql.GetQueryParam());
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }

        }


        /// <summary>
        /// Aplica el descubrimiento sobre un RDF
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <returns></returns>
        [HttpPost("data-discover")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataDiscover(IFormFile rdfFile)
        {
            try
            {
                XmlDocument rdf = SparqlUtility.GetRDFFromFile(rdfFile);
                DiscoverItem discoverItem = new DiscoverItem() { Rdf = rdf.InnerXml, Publish = false, DissambiguationProcessed = false, Status = "Pending" };
                Guid addedID = _discoverItemService.AddDiscoverItem(discoverItem);
                _amqpService.PublishMessage(addedID);
                return Ok(addedID);
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        /// <summary>
        /// Obtiene el estado de una tarea de descubrimiento descubrimiento
        /// </summary>
        /// <param name="identifier">Identificador de la tarea de descubrimiento</param>
        /// <returns></returns>
        [HttpGet("data-discover-state/{identifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(DiscoverStateResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataDiscoverState(Guid identifier)
        {
            try
            {
                DiscoverItem item = _discoverItemService.GetDiscoverItemById(identifier);
                if (item != null)
                {
                    DiscoverStateResult discoverStateResult = new DiscoverStateResult()
                    {
                        ID = item.ID,
                        Status = (DiscoverStateResult.DiscoverItemStatus)Enum.Parse(typeof(DiscoverStateResult.DiscoverItemStatus), item.Status),
                        Rdf = item.Rdf,
                        DiscoverRdf = item.DiscoverRdf,
                        Error = item.Error,
                        DiscoverReport = item.DiscoverReport,

                        DissambiguationProblems = new List<DiscoverStateResult.DiscoverDissambiguation>()
                    };

                    foreach (DiscoverItem.DiscoverDissambiguation problem in item.DissambiguationProblems)
                    {
                        DiscoverStateResult.DiscoverDissambiguation discoverDissambiguation = new DiscoverStateResult.DiscoverDissambiguation()
                        {
                            IDOrigin = problem.IDOrigin,
                            DissambiguationCandiates = new List<DiscoverStateResult.DiscoverDissambiguation.DiscoverDissambiguationCandiate>()
                        };
                        foreach (DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate candidate in problem.DissambiguationCandiates)
                        {
                            discoverDissambiguation.DissambiguationCandiates.Add(new DiscoverStateResult.DiscoverDissambiguation.DiscoverDissambiguationCandiate()
                            {
                                IDCandidate = candidate.IDCandidate,
                                Score = candidate.Score
                            });
                        }
                        discoverStateResult.DissambiguationProblems.Add(discoverDissambiguation);
                    }
                    return Ok(discoverStateResult);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }


        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera un registro de metadatos individual del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="identifier">Identificador de la entidad a recolectar (Los identificadores se obtienen con el metodo /etl/ListIdentifiers/{repositoryIdentifier}).</param>
        /// <param name="metadataPrefix">Prefijo del metadata que se desea recuperar (rdf). Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud /etl/ListMetadataFormats/{repositoryIdentifier}.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("GetRecord/{repositoryIdentifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult GetRecord(Guid repositoryIdentifier, string identifier, string metadataPrefix)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=GetRecord&identifier={identifier}&metadataPrefix={metadataPrefix}";
            byte[] array = _callUri.GetUri(uri);
            //byte[] array = getByte(uri);
            return File(array, "application/xml");
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Obtiene la información del repositorio OAI-PMH configurado en formato XML OAI-PMH.
        /// </summary>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("Identify/{repositoryIdentifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult Identify(Guid repositoryIdentifier)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=Identify";
            //byte[] array = getByte(uri);
            byte[] array = _callUri.GetUri(uri);
            return File(array, "application/xml");
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Es una forma abreviada de ListRecords, que recupera solo encabezados en formato XML OAI-PMH en lugar de registros.        
        /// </summary>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades (Codificado con ISO8601 y expresado en UTC, YYYY-MM-DD o YYYY-MM-DDThh:mm:ssZ)</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades (Codificado con ISO8601 y expresado en UTC, YYYY-MM-DD o YYYY-MM-DDThh:mm:ssZ)</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva. Los formatos de sets admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListSets.</param>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListIdentifiers anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListIdentifiers/{repositoryIdentifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult ListIdentifiers(Guid repositoryIdentifier, string metadataPrefix = null, DateTime? from = null, DateTime? until = null, string set = null, string resumptionToken = null)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=ListIdentifiers";
            if (metadataPrefix != null)
            {
                uri += $"&metadataPrefix={metadataPrefix}";
            }
            if (from.HasValue)
            {
                uri += $"&from={from.Value.ToString("u", CultureInfo.InvariantCulture)}";
            }
            if (until.HasValue)
            {
                uri += $"&until={until.Value.ToString("u", CultureInfo.InvariantCulture)}";
            }
            if (set != null)
            {
                uri += $"&set={set}";
            }
            if (resumptionToken != null)
            {
                uri += $"&resumptionToken={resumptionToken}";
            }
            //byte[] array = getByte(uri);
            byte[] array = _callUri.GetUri(uri);
            return File(array, "application/xml");
        }


        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera los formatos de metadatos disponibles del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="identifier">Argumento opcional que especifica el identificador único del elemento para el que se solicitan los formatos de metadatos disponibles. Si se omite este argumento, la respuesta incluye todos los formatos de metadatos admitidos por este repositorio. </param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListMetadataFormats/{repositoryIdentifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult ListMetadataFormats(Guid repositoryIdentifier, string identifier = null)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=ListMetadataFormats";
            if (identifier != null)
            {
                uri += $"&identifier={identifier}";
            }
            //byte[] array = getByte(uri);
            byte[] array = _callUri.GetUri(uri);
            return File(array, "application/xml");
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera registros del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades (Codificado con ISO8601 y expresado en UTC, YYYY-MM-DD o YYYY-MM-DDThh:mm:ssZ)</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades (Codificado con ISO8601 y expresado en UTC, YYYY-MM-DD o YYYY-MM-DDThh:mm:ssZ)</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva. Los formatos de sets admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListSets.</param>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListRecords anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListRecords/{repositoryIdentifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult ListRecords(Guid repositoryIdentifier, string metadataPrefix, DateTime? from = null, DateTime? until = null, string set = null, string resumptionToken = null)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=ListRecords";
            if (metadataPrefix != null)
            {
                uri += $"&metadataPrefix={metadataPrefix}";
            }
            if (from.HasValue)
            {
                uri += $"&from={from.Value.ToString("u", CultureInfo.InvariantCulture)}";
            }
            if (until.HasValue)
            {
                uri += $"&until={until.Value.ToString("u", CultureInfo.InvariantCulture)}";
            }
            if (set != null)
            {
                uri += $"&set={set}";
            }
            if (resumptionToken != null)
            {
                uri += $"&resumptionToken={resumptionToken}";
            }
            //byte[] array = getByte(uri);
            byte[] array = _callUri.GetUri(uri);
            return File(array, "application/xml");
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recuperar la estructura establecida de un repositorio en formato XML OAI-PMH, útil para la recolección selectiva.        
        /// </summary>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListSets anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListSets/{repositoryIdentifier}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult ListSets(Guid repositoryIdentifier, string resumptionToken = null)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=ListSets";
            if (resumptionToken != null)
            {
                uri += $"&resumptionToken={resumptionToken}";
            }
            //byte[] array = getByte(uri);
            byte[] array = _callUri.GetUri(uri);
            return File(array, "application/xml");
        }

        /// <summary>
        /// Devuelve la ontologia cargada     
        /// </summary>
        /// <returns>Ontologia</returns>
        [HttpGet("GetOntology")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetOntology()
        {
            return Ok(OntologyService.GetOntology());
        }

        /// <summary>
        /// Devuelve el hash de la ontologia cargada     
        /// </summary>
        /// <returns>Hash</returns>
        [HttpGet("GetOntologyHash")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string GetOntologyHash()
        {
            string ontology = OntologyService.GetOntology();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string hash = GetHash(sha256Hash, ontology);
                return hash;
            }
        }

        /// <summary>
        /// Generación de Hash
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private byte[] getByte(string URL)
        {
            HttpWebRequest wrGETURL = (HttpWebRequest)WebRequest.Create(URL);
            System.Net.HttpWebResponse webresponse = (HttpWebResponse)wrGETURL.GetResponse();
            string ct = webresponse.ContentType;
            Stream objStream = webresponse.GetResponseStream();
            BinaryReader breader = new BinaryReader(objStream);
            byte[] buffer = breader.ReadBytes((int)webresponse.ContentLength);
            return buffer;
        }
    }
}