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
        /// <param name="ontology_type">Tipo de ontología; siendo el 0 la ontología roh, el 1 la ontología rohes y el 2 la ontología rohum </param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]/load-ontology")]
        public IActionResult LoadOntology(IFormFile Ontology_uri, int ontology_type)
        {
            try
            {
                _callEDtlPublishService.PostOntology(Ontology_uri, ontology_type);
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
        /// <param name="ontology_type">Tipo de ontología a obtener; siendo el 0 la ontología roh, el 1 la ontología rohes y el 2 la ontología rohum </param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/get-ontology")]
        public IActionResult GetOntology(int ontology_type)
        {
            var fileName = "";
            if (ontology_type == 0)
            {
                fileName = "roh.owl";
            }
            else if (ontology_type == 1)
            {
                fileName = "rohes.owl";
            }
            else
            {
                fileName = "rohum.owl";
            }
            var result = _callEDtlPublishService.GetOntology(ontology_type);
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