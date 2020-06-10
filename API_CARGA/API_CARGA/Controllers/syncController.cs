// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contiene los procesos necesarios para la ejecución de las sincronizaciones.
using System;
using API_CARGA.Models.Services;
using API_CARGA.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_CARGA.Controllers
{
    /// <summary>
    /// Contiene los procesos necesarios para la ejecución de las sincronizaciones.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class syncController : Controller
    {
        private OaiPublishRDFService _oaiPublishRDFService;
        public syncController(OaiPublishRDFService oaiPublishRDFService)
        {
            _oaiPublishRDFService = oaiPublishRDFService;
        }

        /// <summary>
        /// Ejecuta una sincronización
        /// </summary>
        /// <param name="publishModel">Modelo para la ejecución de una sincronización</param>
        /// <returns></returns>
        [HttpPost("execute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostSyncro(PublishRepositoryModel publishModel)
        {
            try
            {
                _oaiPublishRDFService.PublishRepositories(publishModel.repository_identifier, publishModel.fecha_from, publishModel.set);
                return Ok("");
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
            
        }
    }
}