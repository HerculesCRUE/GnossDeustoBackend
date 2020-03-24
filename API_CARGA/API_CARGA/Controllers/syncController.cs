using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UrisFactory.Controllers
{
    /// <summary>
    /// Dentro de este controlador se encuentran todos los métodos para configurar las sincronizaciones, obtener su estado, activarlas y desactivarlas.
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
        /// Obtiene un listado con todas las configuraciones de las sincronizaciones
        /// </summary>
        /// <returns>Listado con todas las configuraciones de las sincronizaciones</returns>
        [HttpGet("config")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<SyncConfig> GetSyncro()
        {
            return null;
        }

        /// <summary>
        /// Obtiene la configuración de una sincronización en particular
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
        /// <returns>Configuración de una sincronización en particular</returns>
        [HttpGet("config/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public SyncConfig GetSyncro(string identifier)
        {
            return null;
        }

        /// <summary>
        /// Añade una nueva configuración de sincronización
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización</param>
        /// <returns></returns>
        [HttpPost("execute/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostSyncro(Guid identifier)
        {
            _oaiPublishRDFService.PublishRepositories(identifier);
            return Ok("");
        }

        /// <summary>
        /// Elimina la configuración de una sincronización.
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización a eliminar</param>
        /// <returns></returns>
        [HttpDelete("config/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteSyncro(string identifier)
        {
            return Ok("");
        }

        /// <summary>
        /// Modifica la configuración de una sincronización 
        /// </summary>
        /// <param name="identifier">Identificador de la sincronización a modificar</param>
        /// <returns></returns>
        [HttpPut("config/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PutSyncro(string identifier)
        {
            return Ok("");
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
    }
}