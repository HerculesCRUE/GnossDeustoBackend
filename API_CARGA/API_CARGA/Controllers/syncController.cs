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
        private ISyncsConfigService _syncConfigService;
        public SyncController(OaiPublishRDFService oaiPublishRDFService, ISyncsConfigService iSyncConfigService)
        {
            _oaiPublishRDFService = oaiPublishRDFService;
            _syncConfigService = iSyncConfigService;
        }

        /// <summary>
        /// Obtiene un listado con todas las configuraciones de las sincronizaciones
        /// </summary>
        /// <returns>Listado con todas las configuraciones de las sincronizaciones</returns>
        [HttpGet("config")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(List<SyncConfig>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConfigSyncsResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSyncro()
        {
            return Ok(_syncConfigService.GetSyncConfigs());
        }

        /// <summary>
        /// Obtiene la configuración de una sincronización en particular
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
        /// <returns>Configuración de una sincronización en particular</returns>
        [HttpGet("config/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(SyncConfig))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConfigSyncResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSyncro(Guid identifier)
        {
            return Ok(_syncConfigService.GetSyncConfigById(identifier));
        }

        /// <summary>
        /// Añade una nueva configuración de sincronización
        /// </summary>
        /// <returns></returns>
        [HttpPost("config")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddConfigSync(SyncConfig syncConfig)
        {
            Guid addedID = _syncConfigService.AddSyncConfig(syncConfig);
            return Ok(addedID);
        }

        /// <summary>
        /// Elimina la configuración de una sincronización.
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización a eliminar</param>
        /// <returns></returns>
        [HttpDelete("config/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteSyncro(Guid identifier)
        {
            bool deleted = _syncConfigService.RemoveSyncConfig(identifier);
            if (deleted)
            {
                return Ok($"Config syncro {identifier} has been deleted");
            }
            else
            {
                return Problem("Error has ocurred");
            }
        }

        /// <summary>
        /// Modifica la configuración de una sincronización 
        /// </summary>
        /// <returns></returns>
        [HttpPut("config/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(ErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ModifySyncErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ModifySyncConfig(SyncConfig syncConfig)
        {
            bool modified = _syncConfigService.ModifySyncConfig(syncConfig);
            if (modified)
            {
                return Ok($"Config repository {syncConfig.SyncConfigID} has been modified");
            }
            else
            {
                return BadRequest(new ErrorExample { Error = $"Check that sync config with id {syncConfig.SyncConfigID} exist" });
            }
        }

        /// <summary>
        /// Obtiene el estado de una sincronización
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
        /// <returns>Datos con el estado de la soncronización</returns>
        [HttpGet("status/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public SyncStatus GetStatus(string identifier)
        {
            return null;
        }

        /// <summary>
        /// Activa una sincronización
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
        /// <param name="start">Fecha de inicio para la sincronización (opcional)</param>
        /// <returns></returns>
        [HttpPost("enable/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Enable(string identifier,DateTime? start=null)
        {
            return Ok("");
        }

        /// <summary>
        /// Desactiva una sincronización
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
        /// <returns></returns>
        [HttpPost("disable/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Disable(string identifier)
        {
            return Ok("");
        }

        /// <summary>
        /// Añade una nueva configuración de sincronización
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
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