using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OaiPmhNet;
using OaiPmhNet.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PMH.Controllers
{
    /// <summary>
    /// Configuración
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ontologyController : Controller
    {
        /// <summary>
        /// Recupera el archivo OWL de una ontología.
        /// </summary>
        /// <param name="id">Identificador de la ontología</param>
        /// <param name="version">Versión de de la ontología (si no se pasa como parámetro se obtiene la última versión)</param>
        /// <returns>Fichero OWL con la ontología</returns>       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(string id, int? version = null)
        {
            return Ok("");
        }


        /// <summary>
        /// Carga y versiona el archivo OWL de una ontología.
        /// </summary>
        /// <param name="id">Identificador de la ontología</param>
        /// <param name="name">Nombre de la ontología</param>
        /// <param name="owlfile">Ontología en formato OWL</param>
        /// <returns></returns>       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult POST(string id,string name, byte[] owlfile)
        {
            return Ok("");
        }

        /// <summary>
        /// Obtiene una lista con los identificadores de todas las ontologías configuradas
        /// </summary>
        /// <returns>Lista con los identificadores de las ontologías</returns>       
        [HttpGet("list", Name = "list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<string> List()
        {
            return new List<string>() { "onto1","onto2"};
        }


        /// <summary>
        /// Estable la validación de la ontología mediante un shape SHACL
        /// </summary>
        /// <param name="id">Identificador de la ontología</param>
        /// <param name="shape">Definición del shape SHACL</param>
        /// <returns></returns>       
        [HttpPut("setShape", Name = "setShape")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult setShape(string id, string shape)
        {
            return Ok("");
        }

        /// <summary>
        /// Obtiene la definición del shape SHACL de la ontología
        /// </summary>
        /// <param name="id">Identificador de la ontología</param>
        /// <returns>Definición del shape SHACL</returns>       
        [HttpGet("getShape", Name = "getShape")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string getShape(string id)
        {
            return "";
        }

        /// <summary>
        /// Elimina la definición del shape SHACL de la ontología
        /// </summary>
        /// <param name="id">Identificador de la ontología</param>
        /// <returns></returns>       
        [HttpDelete("deleteShape", Name = "deleteShape")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult deleteShape(string id)
        {
            return Ok("");
        }
    }
}