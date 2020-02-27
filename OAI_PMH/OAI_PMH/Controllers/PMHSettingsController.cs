using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMH.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PRH.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PMHSettingsController : Controller
    {
        /// <summary>
        /// Obtiene un listado con todas las fuentes configuradas
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListSources", Name = "ListSources")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<Source> ListSources()
        {
            List<Source> sources = new List<Source>();
            sources.Add(new Source() { id = "fuente1", sourceType =Source.SourceType.XML});
            sources.Add(new Source() { id = "fuente2", sourceType = Source.SourceType.DataBase });
            sources.Add(new Source() { id = "fuente3", sourceType = Source.SourceType.API });
            return sources;
        }

        /// <summary>
        /// Agrega una nueva fuente
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddSource", Name = "AddSource")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddSource(SourceSettings source)
        {
            return Ok("");
        }

        /// <summary>
        /// Actualiza la configuración de una fuente
        /// </summary>
        /// <returns></returns>
        [HttpPut("SetSourceSettings", Name = "SetSourceSettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetSource(SourceSettings source)
        {
            return Ok("");
        }

        /// <summary>
        /// Obtiene una fuente
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSourceSettings", Name = "GetSourceSettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public SourceSettings GetSource(string sourceID)
        {
            return new SourceSettings();
        }

        /// <summary>
        /// Elimina una fuente
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteSource", Name = "DeleteSource")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteSource(string sourceID)
        {
            return Ok("");
        }

        /// <summary>
        /// Obtiene la estructura de la fuente para realizar los mapeos
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSourceModel", Name = "GetSourceModel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSourceModel(string sourceID)
        {
            return Ok("");
        }


        /// <summary>
        /// Obtiene un listado con los IDs de las clases configuradas junto con un booleano que indica si tiene mapeo realizado
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListClassMappings", Name = "ListClassMappings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ListClassMappings()
        {
            return Ok("");
        }

        /// <summary>
        /// Añade una clase
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddClass", Name = "AddClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddClass(string pEntityClass, IFormFile pFile)
        {
            return Ok("");
        }

        /// <summary>
        /// Elimina una clase
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteClass", Name = "RemoveClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RemoveClass(string pEntityClass)
        {
            return Ok("");
        }

        /// <summary>
        /// Obtiene la clase solicitada junto con su mapeo configurado
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetClassMapping", Name = "GetClassMapping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetClassMapping(string pEntityClass)
        {
            return Ok("");
        }

        /// <summary>
        /// Añade/modifica el mapeo de una clase
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddClassMapping", Name = "AddClassMapping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddClassMapping(string pEntityClass, object pMapping)
        {
            return Ok("");
        }
    }
}