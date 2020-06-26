using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_Unidata.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Unidata.Controllers
{
    public class UnidataController : Controller
    {
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