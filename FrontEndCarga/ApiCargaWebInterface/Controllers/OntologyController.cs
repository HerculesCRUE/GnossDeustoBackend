// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para reemplazar y obtener las ontologías
using System;
using System.Text;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para reemplazar y obtener las ontologías
    /// </summary>
    public class OntologyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        ICallEtlService _callEDtlPublishService;
        public OntologyController(ICallEtlService callEDtlPublishService)
        {
            _callEDtlPublishService = callEDtlPublishService;
        }

        /// <summary>
        /// Carga una ontología
        /// </summary>
        /// <param name="Ontology_uri">Nueva Ontología</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]/load-ontology")]
        public IActionResult LoadOntology(IFormFile Ontology_uri)
        {
            try
            {
                _callEDtlPublishService.PostOntology(Ontology_uri);
                return View("Index", new OntologyModel
                {
                    Messagge = $"Ontologia subida"
                });
            }
            catch (Exception ex)
            {
                return View("Index", new OntologyModel
                {
                    Messagge = $"Ha ocurrido un error al publicar la ontologia"
                });
            }
        }

        /// <summary>
        /// Obtiene la ontología
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/get-ontology")]
        public IActionResult GetOntology()
        {
            var fileName = "roh.owl";
            var result = _callEDtlPublishService.GetOntology();
            if (result != null)
            {
                var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(result));
                var contentType = "APPLICATION/octet-stream";
                return File(content, contentType, fileName);
            }
            else
            {
                return NotFound();
            }
        }
    }
}