using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_CARGA.Controllers
{
    [Route("etl-config/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {

        /// <summary>
        /// Obtiene la configuración de los shape SHACL de validación
        /// </summary>
        /// <returns>Listado con las definiciones de las validaciones</returns>       
        [HttpGet]
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
        [HttpGet("{identifier}")]
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
        [HttpPost]
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
        [HttpDelete("{identifier}")]
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
        [HttpPut("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PutShape(string identifier, ShapeConfig shapeconfig)
        {
            return Ok("");
        }
    }
}