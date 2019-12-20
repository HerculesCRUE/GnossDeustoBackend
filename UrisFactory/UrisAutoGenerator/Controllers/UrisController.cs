using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;

namespace UrisFactory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrisController : ControllerBase
    {
        [HttpGet(Name= "GenerateUri")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("GenerateUri")]
        public IActionResult GenerateUri(string resource, string resource_class, string identifier, [FromQuery] Dictionary<string,string> parametros_opcionales)
        {
            var queryString = HttpContext.Request.Query.ToList();

            Dictionary<string, string> queryDictionary = new Dictionary<string, string>();
            foreach(var value in queryString)
            {
                queryDictionary.Add(value.Key, value.Value.FirstOrDefault());
            }

            string uri = UriFormer.GetURI(resource, resource_class, queryDictionary);
            return Ok(uri);
        }
    }
}