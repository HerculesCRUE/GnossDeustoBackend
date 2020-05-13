using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_CARGA.ModelExamples;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using API_CARGA.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace UrisFactory.Controllers
{
    /// <summary>
    /// Dentro de este controlador se encuentran todos los métodos para configurar las sincronizaciones, obtener su estado, activarlas y desactivarlas.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class SyncController : Controller
    {
        private OaiPublishRDFService _oaiPublishRDFService;
        public SyncController(OaiPublishRDFService oaiPublishRDFService)
        {
            _oaiPublishRDFService = oaiPublishRDFService;
        }

        /// <summary>
        /// Ejecuta una sincronización
        /// </summary>
        /// <param name="publishModel">Modelo para la ejecución de una soncronización</param>
        /// <returns></returns>
        [HttpPost("execute")]///{identifier}
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