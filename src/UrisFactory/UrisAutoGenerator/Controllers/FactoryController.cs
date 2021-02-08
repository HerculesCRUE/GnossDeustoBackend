// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador encargado de generar una uri válida para una resource class y un identificador ORCID
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using UrisFactory.Extra.Enum;
using UrisFactory.ModelExamples;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;

namespace UrisFactory.Controllers
{
    /// <summary>
    /// Controlador encargado de generar una uri válida para una resource class y un identificador ORCID
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
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
        ///<param name="resource_class">nombre de la resource class o rdfType que especifica la estructura de uris a usar, el listado de resource class se pueden obtener a través de http://herc-as-front-desa.atica.um.es/uris/Schema, en el apartado ResourcesClasses-> ResourceClass; ejemplo: Article</param>
        ///<param name="identifier">identifier, es un cadena que representa un ORCID</param>
        ///<param name="eleccion_uri">los valores posibles son 0 y 1, este valor indican si el parametro resource_class pasado es un resource class o si por lo contrario es el rdfType, el 0 indica que es un resource class mientras que el 1 es para indicar que es rdfType</param>
        [HttpGet(Name= "GenerateUri")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UrisFactoryResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UrisFactoryErrorReponse))]
        public IActionResult GenerateUri(string resource_class, string identifier, EleccionUri eleccion_uri)
        {
            Dictionary<string, string> queryDictionary = new Dictionary<string, string>();
            if (HttpContext != null)
            {
                var queryString = HttpContext.Request.Query.ToList();
                foreach (var value in queryString)
                {
                    queryDictionary.Add(value.Key, value.Value.FirstOrDefault());
                }
            }
            else
            {
                queryDictionary.Add(UriComponentsList.Identifier, identifier);
            }

            UriFormer uriFormer = new UriFormer(_configJsonHandler.GetUrisConfig());
            string uri = "";
            if (eleccion_uri.Equals(EleccionUri.RDFtype))
            {
                uri = uriFormer.GetURI(resource_class, queryDictionary, true);
            }
            else
            {
                uri = uriFormer.GetURI(resource_class, queryDictionary);
            }
            
            return Ok(uri);
        }
    }
}