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
    public class PublishController : Controller
    {
        ICallEtlService _callEtlPublishService;
        readonly ICallRepositoryConfigService _serviceApi;
        public PublishController(ICallEtlService callDtlPublishService, ICallRepositoryConfigService serviceApi)
        {
            _callEtlPublishService = callDtlPublishService;
            _serviceApi = serviceApi;
        }
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
            return View(publishRepositoryModel);
        }

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
        [HttpPost]
        [Route("[Controller]/validate")]
        public IActionResult ValidateRdf(Guid repositoryId, IFormFile rdfToValidate, IFormFile validationRDF, List<Guid> shapesList)
        {
            try
            {
                RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repositoryId);
                _callEtlPublishService.ValidateRDFPersonalized(repositoryId, rdfToValidate, validationRDF, shapesList, result.ShapeConfig);
                
                return View("Index", new PublishRepositoryModel
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

        [HttpPost]
        [Route("[Controller]")]
        public IActionResult PublishRdf(Guid repositoryId, IFormFile rdfPublish)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(repositoryId);
            try
            {
                _callEtlPublishService.CallDataValidate(rdfPublish, repositoryId);
                _callEtlPublishService.CallDataPublish(rdfPublish);
                
                return View("Index", new PublishRepositoryModel
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
                return View("Index", new PublishRepositoryModel
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