// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using API_CARGA.Models.Utility;
using Hercules.Asio.Api.Carga.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using VDS.RDF;

namespace API_CARGA.Controllers
{
    /// <summary>
    /// Contiene los procesos ETL (Extract, Transform and Load) necesarios para la carga de datos.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("[controller]")]

    public class etlController : Controller
    {
        readonly private IRepositoriesConfigService _repositoriesConfigService;
        readonly private IDiscoverItemService _discoverItemService;
        readonly private IShapesConfigService _shapeConfigService;
        readonly ConfigSparql _configSparql;
        readonly CallOAIPMH _callOAIPMH;
        readonly IRabbitMQService _amqpService;

        public etlController(IDiscoverItemService iDiscoverItemService, IRepositoriesConfigService iRepositoriesConfigService, IShapesConfigService iShapeConfigService, ConfigSparql configSparql, CallOAIPMH callOAIPMH, CallConversor callConversor, ConfigUrlService configUrlService, IRabbitMQService amqpService)
        {
            _discoverItemService = iDiscoverItemService;
            _repositoriesConfigService = iRepositoriesConfigService;
            _shapeConfigService = iShapeConfigService;
            _configSparql = configSparql;
            _callOAIPMH = callOAIPMH;
            _amqpService = amqpService;
        }

        [ExcludeFromCodeCoverage]
        //No se puede ejecuar el test desde gitHub

        /// <summary>
        /// Ejecuta el penúltimo paso del proceso de carga, por el que el RDF generado se encola en una cola de Rabbit MQ para que posteriormente el servicio de descubimiento lo procese y lo almacene en el Triple Store. Permite cargar una fuente RDF arbitraria.
        /// Aquí se encuentra un RDF de Ejemplo: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/API_CARGA/API_CARGA/Samples/rdfSample.xml
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <param name="jobId">Identificador de la tarea</param>
        /// <param name="discoverProcessed">Indica si ya está procesado el descubrimiento</param>
        /// <returns></returns>
        [HttpPost("data-publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataPublish(IFormFile rdfFile, string jobId, bool discoverProcessed)
        {
            try
            {
                string idDiscoverItem = null;
                if (Request != null)
                {
                    idDiscoverItem = Request.Query["idDiscoverItem"].ToString();
                }
                if (!string.IsNullOrEmpty(idDiscoverItem))
                {
                    //Si viene el parametro 'idDiscoverItem' actualizamos un DiscoverItem ya existente.
                    XmlDocument rdf = SparqlUtility.GetRDFFromFile(rdfFile);

                    DiscoverItem discoverItem = _discoverItemService.GetDiscoverItemById(new Guid(idDiscoverItem));
                    discoverItem.DissambiguationProcessed = discoverProcessed;
                    discoverItem.Publish = true;
                    discoverItem.Status = "Pending";
                    discoverItem.DiscoverRdf = rdf.InnerXml;

                    _amqpService.PublishMessage(idDiscoverItem,((RabbitMQService)_amqpService).queueName);
                    return Ok();
                }
                else
                {
                    XmlDocument rdf = SparqlUtility.GetRDFFromFile(rdfFile);
                    DiscoverItem discoverItem = new DiscoverItem() { JobID = jobId, Rdf = rdf.InnerXml, Publish = true, DissambiguationProcessed = discoverProcessed, Status = "Pending" };
                    Guid addedID = _discoverItemService.AddDiscoverItem(discoverItem);
                    _amqpService.PublishMessage(addedID, ((RabbitMQService)_amqpService).queueName);
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
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ShapeReport))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataValidate(IFormFile rdfFile, Guid repositoryIdentifier)
        {
            try
            {
                string rdfFileContent = SparqlUtility.GetTextFromFile(rdfFile);
                RohGraph ontologyGraph = new RohGraph();
                ontologyGraph.LoadFromString(OntologyService.GetOntology());
                return Ok(SparqlUtility.ValidateRDF(rdfFileContent, _shapeConfigService.GetShapesConfigs().FindAll(x => x.RepositoryID == repositoryIdentifier), ontologyGraph));
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult LoadOntology(IFormFile ontology)
        {
            try
            {
                OntologyService.SetOntology(ontology);
                string ontologyGraph = "";
                ontologyGraph = _configSparql.GetGraphRoh();
                RohGraph graph = new RohGraph();
                graph.LoadFromString(OntologyService.GetOntology());
                SparqlUtility.LoadOntology((RabbitMQService)_amqpService,graph, _configSparql.GetEndpoint(), _configSparql.GetQueryParam(), ontologyGraph, _configSparql.GetUsername(), _configSparql.GetPassword());
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }

        }

        [ExcludeFromCodeCoverage]
        //No se puede ejecuar el test desde gitHub

        /// <summary>
        /// Aplica el descubrimiento sobre un RDF
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <returns></returns>
        [HttpPost("data-discover")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataDiscover(IFormFile rdfFile)
        {
            try
            {
                XmlDocument rdf = SparqlUtility.GetRDFFromFile(rdfFile);
                DiscoverItem discoverItem = new DiscoverItem() { Rdf = rdf.InnerXml, Publish = false, DissambiguationProcessed = false, Status = "Pending" };
                Guid addedID = _discoverItemService.AddDiscoverItem(discoverItem);
                _amqpService.PublishMessage(addedID,((RabbitMQService)_amqpService).queueName);
                return Ok(addedID);
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [ExcludeFromCodeCoverage]
        //No se puede ejecuar el test desde gitHub

        /// <summary>
        /// Obtiene el estado de una tarea de descubrimiento
        /// </summary>
        /// <param name="identifier">Identificador de la tarea de descubrimiento</param>
        /// <returns></returns>
        [HttpGet("data-discover-state/{identifier}")]
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

        [ExcludeFromCodeCoverage]
        //Exluido del analis porque se necesita llamar a una url y no se debe llamar a otro servicio en un test unitario

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera un registro de metadatos individual del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="identifier">Identificador de la entidad a recolectar (Los identificadores se obtienen con el metodo /etl/ListIdentifiers/{repositoryIdentifier}).</param>
        /// <param name="metadataPrefix">Prefijo del metadata que se desea recuperar (rdf). Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud /etl/ListMetadataFormats/{repositoryIdentifier}.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("GetRecord/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult GetRecord(Guid repositoryIdentifier, string identifier, string metadataPrefix)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=GetRecord&identifier={System.Web.HttpUtility.UrlEncode(identifier)}&metadataPrefix={metadataPrefix}";
            byte[] array = _callOAIPMH.GetUri(uri);
            return File(array, "application/xml");
        }

        [ExcludeFromCodeCoverage]
        //Exluido del analis porque se necesita llamar a una url y no se debe llamar a otro servicio en un test unitario

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Obtiene la información del repositorio OAI-PMH configurado en formato XML OAI-PMH.
        /// </summary>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("Identify/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult Identify(Guid repositoryIdentifier)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=Identify";
            byte[] array = _callOAIPMH.GetUri(uri);
            return File(array, "application/xml");
        }

        [ExcludeFromCodeCoverage]
        //Exluido del analis porque se necesita llamar a una url y no se debe llamar a otro servicio en un test unitario

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult ListIdentifiers(Guid repositoryIdentifier, string metadataPrefix = null, string from = null, string until = null, string set = null, string resumptionToken = null)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=ListIdentifiers";
            if (metadataPrefix != null)
            {
                uri += $"&metadataPrefix={metadataPrefix}";
            }
            if (!string.IsNullOrEmpty(from))
            {
                uri += $"&from={from}";
            }
            if (!string.IsNullOrEmpty(until))
            {
                uri += $"&until={until}";
            }
            if (set != null)
            {
                uri += $"&set={set}";
            }
            if (resumptionToken != null)
            {
                uri += $"&resumptionToken={resumptionToken}";
            }
            byte[] array = _callOAIPMH.GetUri(uri);
            return File(array, "application/xml");
        }

        [ExcludeFromCodeCoverage]
        //Exluido del analis porque se necesita llamar a una url y no se debe llamar a otro servicio en un test unitario

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera los formatos de metadatos disponibles del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="identifier">Argumento opcional que especifica el identificador único del elemento para el que se solicitan los formatos de metadatos disponibles. Si se omite este argumento, la respuesta incluye todos los formatos de metadatos admitidos por este repositorio. </param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListMetadataFormats/{repositoryIdentifier}")]
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
            byte[] array = _callOAIPMH.GetUri(uri);
            return File(array, "application/xml");
        }

        [ExcludeFromCodeCoverage]
        //Exluido del analis porque se necesita llamar a una url y no se debe llamar a otro servicio en un test unitario

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult ListRecords(Guid repositoryIdentifier, string metadataPrefix, string from = null, string until = null, string set = null, string resumptionToken = null)
        {
            RepositoryConfig repositoryConfig = _repositoriesConfigService.GetRepositoryConfigById(repositoryIdentifier);
            string uri = repositoryConfig.Url;
            uri += $"?verb=ListRecords";
            if (metadataPrefix != null)
            {
                uri += $"&metadataPrefix={metadataPrefix}";
            }
            if (!string.IsNullOrEmpty(from))
            {
                uri += $"&from={from}";
            }
            if (!string.IsNullOrEmpty(until))
            {
                uri += $"&until={until}";
            }
            if (set != null)
            {
                uri += $"&set={set}";
            }
            if (resumptionToken != null)
            {
                uri += $"&resumptionToken={resumptionToken}";
            }
            byte[] array = _callOAIPMH.GetUri(uri);
            return File(array, "application/xml");
        }

        [ExcludeFromCodeCoverage]
        //Exluido del analis porque se necesita llamar a una url y no se debe llamar a otro servicio en un test unitario


        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recuperar la estructura establecida de un repositorio en formato XML OAI-PMH, útil para la recolección selectiva.        
        /// </summary>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListSets anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH (los repositorios disponibles están en /etl-config/repository)</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListSets/{repositoryIdentifier}")]
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
            byte[] array = _callOAIPMH.GetUri(uri);
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
         [NonAction]
        public string GetHash(HashAlgorithm hashAlgorithm, string input)
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
    }
}