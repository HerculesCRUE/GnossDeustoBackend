using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using UrisFactory.Models.Services;

namespace UrisFactory.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UrisController : ControllerBase
    {
        private ConfigJsonHandler _configJsonHandler;

        public UrisController(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

        [HttpGet(Name= "GenerateUri")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateUri(string resource, string resource_class, string identifier, [FromQuery] Dictionary<string,string> parametros_opcionales)
        {
            var queryString = HttpContext.Request.Query.ToList();
            Dictionary<string, string> queryDictionary = new Dictionary<string, string>();
            foreach(var value in queryString)
            {
                queryDictionary.Add(value.Key, value.Value.FirstOrDefault());
            }

            UriFormer uriFormer = new UriFormer(_configJsonHandler.GetUrisConfig());
            string uri = uriFormer.GetURI(resource, resource_class, queryDictionary);
            return Ok(uri);
        }
    }
}