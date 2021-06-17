// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para cargar triples en el grafo unidata
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_Unidata.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Unidata.Controllers
{
    /// <summary>
    /// Controlador para cargar triples en el grafo unidata
    /// </summary>
    [Authorize]
    public class UnidataController : Controller
    {
        /// <summary>
        /// Carga los triples en el grafo unidata
        /// </summary>
        /// <param name="triples">Lista de triples a insertar</param> 
        [HttpPost("loadtriples")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult LoadTriples(List<string> triples)
        {
            
            SparqlUtility.LoadTriples(triples);
            return Ok();
        }
    }
}