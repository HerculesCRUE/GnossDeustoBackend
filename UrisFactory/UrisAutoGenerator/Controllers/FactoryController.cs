using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using UrisFactory.Models.Services;

namespace UrisFactory.Controllers
{

    [ApiController]
    //[Route("uris/[controller]")]
    [Route("[controller]")]
    public class FactoryController : ControllerBase
    {
        private ConfigJsonHandler _configJsonHandler;

        public FactoryController(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

        ///<summary>
        ///Generate a Uri associated with the resource class and the identifier
        ///</summary>
        ///<param name="resource_class">name of the resource class that specified the resource uri structure to use</param>
        ///<param name="identifier">identifier</param>
        ///<returns>test</returns>
        [HttpGet(Name= "GenerateUri")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateUri(string resource_class, string identifier)
        {
            var queryString = HttpContext.Request.Query.ToList();
            Dictionary<string, string> queryDictionary = new Dictionary<string, string>();
            foreach(var value in queryString)
            {
                queryDictionary.Add(value.Key, value.Value.FirstOrDefault());
            }

            UriFormer uriFormer = new UriFormer(_configJsonHandler.GetUrisConfig());
            string uri = uriFormer.GetURI(resource_class, queryDictionary);
            return Ok(uri);
        }
    }
}