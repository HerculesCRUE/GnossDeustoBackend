using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using UrisFactory.ModelExamples;
using UrisFactory.Models.Services;

namespace UrisFactory.Controllers
{
    /// <summary>
    /// Controlador encargado de generar una uri válida para una resource class y un identificador ORCID
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FactoryController : ControllerBase
    {
        private ConfigJsonHandler _configJsonHandler;

        public FactoryController(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

        ///<summary>
        ///Genera una Uri con la estructura asociada a la resource class pasada con el identificador como parametro, ejemplo de uso: con la llamada: "http://herc-as-front-desa.atica.um.es/uris/Factory?resource_class=Article&amp;identifier=1231d", se obtiene http://graph.um.es/res/article/1231d
        ///</summary>
        ///<param name="resource_class">nombre de la resource class que especifica la estructura de uris a usar, el listado de resource class se pueden obtener a través de http://herc-as-front-desa.atica.um.es/uris/Schema, en el apartado ResourcesClasses-> ResourceClass; ejemplo: Article</param>
        ///<param name="identifier">identifier, es un cadena que represent un ORCID</param>
        [HttpGet(Name= "GenerateUri")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UrisFactoryResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UrisFactoryErrorReponse))]
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