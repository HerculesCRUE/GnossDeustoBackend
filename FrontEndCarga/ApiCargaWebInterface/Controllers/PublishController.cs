using System;
using System.Collections.Generic;
using System.Linq;
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
        ICallEtlService _callEDtlPublishService;
        public PublishController(ICallEtlService callEDtlPublishService)
        {
            _callEDtlPublishService = callEDtlPublishService;
        }
        [Route("[Controller]/{repository}")]
        public IActionResult Index(Guid repository)
        {
            PublishRepositoryModel publishRepositoryModel = new PublishRepositoryModel()
            {
                RepositoryId = repository,
                Id = "",
                Type = "rdf"
            };
            return View(publishRepositoryModel);
        }

        [HttpGet]
        [Route("[Controller]/getrdf")]
        public IActionResult GetRDF(Guid repositoryId, string id, string type)
        {
            string result = _callEDtlPublishService.CallGetRecord(repositoryId, id, type);
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
        [Route("[Controller]")]
        public IActionResult PublishRdf(Guid repositoryId, IFormFile rdfPublish)
        {
            try
            {
                _callEDtlPublishService.CallDataValidate(rdfPublish, repositoryId);
                _callEDtlPublishService.CallDataPublish(rdfPublish);
                return View("Index", new PublishRepositoryModel
                {
                    RepositoryId = repositoryId,
                    Id = "",
                    Type = "rdf",
                    Result = $"Publicado con éxito el rdf: {rdfPublish.FileName}"
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
                    Result = $"Ha ocurrido un error al publicar el rdf"
                });
            }
        }

       
    }
}