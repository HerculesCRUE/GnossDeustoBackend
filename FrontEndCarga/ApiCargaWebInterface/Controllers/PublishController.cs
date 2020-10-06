// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para la publicación manual y la validacion de los rdf
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para la publicación manual y la validacion de los rdf
    /// </summary>
    public class PublishController : Controller
    {
        ICallEtlService _callEtlPublishService;
        readonly ICallRepositoryConfigService _serviceApi;
        public PublishController(ICallEtlService callDtlPublishService, ICallRepositoryConfigService serviceApi)
        {
            _callEtlPublishService = callDtlPublishService;
            _serviceApi = serviceApi;
        }
        /// <summary>
        /// Obtiene la información de un repositorio OAIPMH
        /// </summary>
        /// <param name="repository">Identificador del repositorio</param>
        /// <returns></returns>
        [Route("[Controller]/{repository}")]
        public IActionResult Index(Guid repository)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repository);
            PublishRepositoryModel publishRepositoryModel = new PublishRepositoryModel()
            {
                RepositoryId = repository,
                Id = "",
                Type = "rdf",
                RepositoryShapes = result.ShapeConfig
            };
            return View("ObtenerRdf",publishRepositoryModel);
        }
        /// <summary>
        /// Obtiene la página de obtención de un RDF
        /// </summary>
        /// <param name="repository">Identificador del repositorio</param>
        /// <returns></returns>
        [Route("[Controller]/rdf/{repository}")]
        public IActionResult ObtenerRdf(Guid repository)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repository);
            PublishRepositoryModel publishRepositoryModel = new PublishRepositoryModel()
            {
                RepositoryId = repository,
                Id = "",
                Type = "rdf",
            };
            return View(publishRepositoryModel);
        }
        /// <summary>
        /// Obtiene la página para validar un RDF
        /// </summary>
        /// <param name="repository">Identificador del repositorio</param>
        /// <returns></returns>
        [Route("[Controller]/validate/{repository}")]
        public IActionResult ValidarRdf(Guid repository)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repository);
            PublishRepositoryModel publishRepositoryModel = new PublishRepositoryModel()
            {
                RepositoryId = repository,
                Id = "",
                Type = "rdf",
                RepositoryShapes = result.ShapeConfig
            };
            return View(publishRepositoryModel);
        }
        /// <summary>
        /// Obtiene la página para publicar un RDF
        /// </summary>
        /// <param name="repository">Identificador del repositorio</param>
        /// <returns></returns>
        [Route("[Controller]/send/{repository}")]
        public IActionResult PublicarRdf(Guid repository)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repository);
            PublishRepositoryModel publishRepositoryModel = new PublishRepositoryModel()
            {
                RepositoryId = repository,
                Id = "",
                Type = "rdf",
            };
            return View(publishRepositoryModel);
        }
        /// <summary>
        /// Obtiene el rdf de un elemento asociado al repositorio
        /// </summary>
        /// <param name="repoIdentifier">Identificador del repositorio OAI-PMH </param>
        /// <param name="identifier">Identificador de la entidad a recolectar (Los identificadores se obtienen con el metodo /etl/ListIdentifiers/{repositoryIdentifier}).</param>
        /// <param name="type">metadata que se desea recuperar (rdf). Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud /etl/ListMetadataFormats/{repositoryIdentifier}.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/getrdf")]
        public IActionResult GetRDF(Guid repositoryId, string id, string type)
        {
            string result = _callEtlPublishService.CallGetRecord(repositoryId, id, type);
            if (result != null)
            {
                var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(result));
                var contentType = "APPLICATION/octet-stream";
                var fileName = $"{repositoryId}_{id}.rdf";
                return File(content, contentType, fileName);
            }
            else
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Valida un rdf tanto con un rdf de validación personalizado como por una lista de shapes configuradas en el repositorio
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio OAIPMH</param>
        /// <param name="rdfToValidate">RDF a validar</param>
        /// <param name="validationRdf">RDF de validación</param>
        /// <param name="shapesList">Lista de shapes de validación</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]/validate")]
        public IActionResult ValidateRdf(Guid repositoryId, IFormFile rdfToValidate, IFormFile validationRDF, List<Guid> shapesList)
        {
            try
            {
                RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repositoryId);
                _callEtlPublishService.ValidateRDFPersonalized(repositoryId, rdfToValidate, validationRDF, shapesList, result.ShapeConfig);
                
                return View("ValidarRdf", new PublishRepositoryModel
                {
                    RepositoryId = repositoryId,
                    Id = "",
                    Type = "rdf",
                    Result = $"RDF válido",
                    RepositoryShapes = result.ShapeConfig
                });
            }
            catch (ValidationException vEx)
            {
                return View("ValidationError", vEx.Report);
            }
        }

        /// <summary>
        /// Publica un rdf
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio</param>
        /// <param name="rdfPublish">RDF a publicar</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]")]
        public IActionResult PublishRdf(Guid repositoryId, IFormFile rdfPublish)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repositoryId);
            try
            {
                _callEtlPublishService.CallDataValidate(rdfPublish, repositoryId);
                _callEtlPublishService.CallDataPublish(rdfPublish,"",true);
                
                return View("PublicarRdf", new PublishRepositoryModel
                {
                    RepositoryId = repositoryId,
                    Id = "",
                    Type = "rdf",
                    Result = $"Publicado con éxito el rdf: {rdfPublish.FileName}",
                    RepositoryShapes = result.ShapeConfig
                });
            }
            catch (ValidationException vEx)
            {
                return View("ValidationError", vEx.Report);
            }
            catch (Exception ex)
            {
                return View("PublicarRdf", new PublishRepositoryModel
                {
                    RepositoryId = repositoryId,
                    Id = "",
                    Type = "rdf",
                    Result = $"Ha ocurrido un error al publicar el rdf",
                    RepositoryShapes = result.ShapeConfig
                });
            }
        }

       
    }
}