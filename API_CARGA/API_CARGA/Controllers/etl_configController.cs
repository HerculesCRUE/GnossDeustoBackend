using System;
using System.Collections.Generic;
using System.Linq;
using API_CARGA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OaiPmhNet;
using OaiPmhNet.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PMH.Controllers
{
    /// <summary>
    /// Configuración del ETL
    /// </summary>
    [ApiController]
    [Route("etl-config")]
    public class etl_configController : Controller
    {
        /// <summary>
        /// Obtiene un listado con todas las configuraciones de los repositorios OAI-PMH
        /// </summary>
        /// <returns>Listado con todas las configuraciones de los repositorios OAI-PMH</returns>
        [HttpGet("repository")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<RepositoryConfig> GetRepository()
        {
            return null;
        }

        /// <summary>
        /// Obtiene la configuración de un repositorio OAI-PMH
        /// </summary>
        /// <param name="identifier">Identificador del repositorio</param>
        /// <returns>Configuración del repositorio</returns>
        [HttpGet("repository/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public RepositoryConfig GetRepository(string identifier)
        {
            return null;
        }

        /// <summary>
        /// Añade una nueva configuración de un repositorio OAI-PMH
        /// </summary>
        /// <param name="repositoryconfig">Datos de configuración del repositorio</param>
        /// <returns></returns>
        [HttpPost("repository")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostRepository(RepositoryConfig repositoryconfig)
        {
            return Ok("");
        }

        /// <summary>
        /// Elimina la configuración de un repositorio OAI-PMH.
        /// </summary>
        /// <param name="identifier">Identificador del repositorio</param>
        /// <returns></returns>
        [HttpDelete("repository/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteRepository(string identifier)
        {
            return Ok("");
        }

        /// <summary>
        /// Modifica la configuración de un repositorio OAI-PMH.
        /// </summary>
        /// <param name="identifier">Identificador del repositorio</param>
        /// <param name="repositoryconfig">Datos de configuración del repositorio</param>
        /// <returns></returns>
        [HttpPut("repository/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PutRepository(string identifier, RepositoryConfig repositoryconfig)
        {
            return Ok("");
        }




        /// <summary>
        /// Obtiene la configuración de los shape SHACL de validación
        /// </summary>
        /// <returns>Listado con las definiciones de las validaciones</returns>       
        [HttpGet("validation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<ShapeConfig> GetShape()
        {
            return null;
        }

        /// <summary>
        /// Obtiene la configuración del shape SHACL pasado por parámetro
        /// </summary>
        /// <param name="identifier">Identificador de la validación</param>
        /// <returns>Definición de la validación</returns>       
        [HttpGet("validation/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ShapeConfig GetShape(string identifier)
        {
            return null;
        }


        /// <summary>
        /// Añade una configuración de validación mediante un shape SHACL
        /// </summary>
        /// <param name="shapeconfig">Configuración de la validación</param>
        /// <returns></returns>
        [HttpPost("validation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostShape(ShapeConfig shapeconfig)
        {
            return Ok("");
        }

        /// <summary>
        /// Elimina la configuración una configuración de validación 
        /// </summary>
        /// <param name="identifier">Identificador de la validación</param>
        /// <returns></returns>
        [HttpDelete("validation/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteShape(string identifier)
        {
            return Ok("");
        }

        /// <summary>
        /// Modifica la configuración de validación mediante un shape SHACL
        /// </summary>
        /// <param name="identifier">Identificador de la validación</param>
        /// <param name="shapeconfig">Datos de configuración de la validación</param>
        /// <returns></returns>
        [HttpPut("validation/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PutShape(string identifier, ShapeConfig shapeconfig)
        {
            return Ok("");
        }

    }
}