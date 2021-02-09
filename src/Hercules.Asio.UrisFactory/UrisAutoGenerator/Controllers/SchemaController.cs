// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador encargado de gerstionar el esquema de Uris, que ofrece métodos para la consulta de este esquema y su modificación
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.ViewModels;
using UrisFactory.Models.Services;
using Swashbuckle.AspNetCore.Filters;
using UrisFactory.ModelExamples;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using UrisFactory.Extra.Enum;

namespace UrisFactory.Controllers
{
    /// <summary>
    /// Controlador encargado de gerstionar el esquema de Uris, que ofrece métodos para la consulta de este esquema y su modificación
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    //[Authorize]
    public class SchemaController : Controller
    {
        private ConfigJsonHandler _configJsonHandler;
        private ISchemaConfigOperations _schemaConfigOperations;

        public SchemaController(ConfigJsonHandler configJsonHandler, ISchemaConfigOperations schemaConfigOperations)
        {
            _configJsonHandler = configJsonHandler;
            _schemaConfigOperations = schemaConfigOperations;
        }

        ///<summary>
        ///Obtiene el fichero de configuración de los esquemas configurados
        ///</summary>
        [HttpGet(Name="getSchema")]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(UriStructureGeneral))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UriStructureGeneralExample))]
        public FileResult GetSchema()
        {
            string contentType = _schemaConfigOperations.GetContentType();
            return File(_schemaConfigOperations.GetFileSchemaData(), contentType);
        }

        ///<summary>
        ///Reemplaza el fichero de configuración por otro fichero dado, para ver la estrucutura del fichero, se recomienda ver el fichero dado por: http://herc-as-front-desa.atica.um.es/uris/Schema
        ///</summary>
        ///<remarks>
        ///Se puede encontrar un ejemplo del fichero como plantilla en: https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/20191211%20Esquema%20de%20URIs.json
        /// </remarks>
        ///<param name="newSchemaConfig">nuevo fichero de configuración</param>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ReplaceSchemaResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ReplaceShemaErrorResponse))]
        public IActionResult ReplaceSchemaConfig(IFormFile newSchemaConfig)
        {
            bool result = _schemaConfigOperations.SaveConfigFile(newSchemaConfig);
            if (result)
            {
                return Ok("new config file loaded");
            }
            else
            {
                return BadRequest("{{\"error\": \" new config file is not correctly formed.\"}}");
            }
        }

        ///<summary>
        ///Obtiene la estrucutra uri y las resources class asociadas a esa estructura R
        ///</summary>
        ///<param name="name">nombre de la estructura uri, se pueden obtener a través del método http://herc-as-front-desa.atica.um.es/uris/Schema, en los objetos UriStructures, Name</param>
        [HttpGet("{name}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(InfoUriStructure))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UriStructureInfoRequest))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(InfoStructureErrorResponse))]
        public IActionResult GetUriStructureInfo(string name)
        {
            UriStructure uri = _configJsonHandler.GetUriStructure(name);
            if (uri != null)
            {
                List<ResourcesClass> resourceClass = _configJsonHandler.GetResourceClass(name);
                InfoUriStructure infoUriStructure= new InfoUriStructure();
                infoUriStructure.UriStructure = uri;
                infoUriStructure.ResourcesClass = resourceClass;
                return Ok(infoUriStructure);
            }
            else
            {
                return BadRequest($"{{\"error\": \"No data of uriStructure {name}\"}}");
            }
        }

        ///<summary>
        ///Borra la estrcutura uri y las resource class asociadas a esa estructura
        ///</summary>
        ///<param name="name">nombre de la estructura uri a eliminar, se pueden obtener a través del método http://herc-as-front-desa.atica.um.es/uris/Schema, en los objetos UriStructures, Name</param>
        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DeleteUriStructureResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(DeleteUriStructureErrorResponse))]
        public IActionResult DeleteUriStructure(string name)
        {
            if (_configJsonHandler.ExistUriStructure(name))
            {
                _configJsonHandler.DeleteUriStructureInfo(name);
                bool deleted = _schemaConfigOperations.SaveConfigJson();
                if (deleted)
                {
                    return Ok($"uriStructure: {name} has been deleted and the new config schema is loaded");
                }
                else
                {
                    return Problem(detail: "Server error has ocurred",null,500);
                }
            }
            else
            {
                return BadRequest($"{{\"error\": \"No data of uriStructure {name}\"}}");
            }
        }

        ///<summary>
        ///Añade una nueva estructura de uris y una reource class asociada a esta nueva estrucutra 
        ///</summary>
        ///<param name="infoUriStructure">objeto que contiene una estrucutura nueva y una resource class asociada a esa estructura</param>
        [HttpPut]
        [SwaggerRequestExample(typeof(InfoUriStructure), typeof(UriStructureInfoRequest))]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddUriStructureResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AddUriStructureErrorResponse))]
        public IActionResult AddUriStructure(InfoUriStructure infoUriStructure)
        {
            if(infoUriStructure != null && infoUriStructure.ResourcesClass != null && infoUriStructure.UriStructure != null && infoUriStructure.ResourcesClass.Count == 1)
            {
                try
                {
                    _configJsonHandler.AddUriStructureInfo(infoUriStructure.UriStructure, infoUriStructure.ResourcesClass.First());
                    bool saved = _schemaConfigOperations.SaveConfigJson();
                    if (saved)
                    {
                        return Ok($"uriStructure: {infoUriStructure.UriStructure.Name} has been added and the new config schema is loaded");
                    }
                    else
                    {
                        return Problem(detail: "Server error has ocurred", null, 500);
                    }
                }
                catch(UriStructureConfiguredException confEx)
                {
                    return BadRequest(confEx.Message);
                }
                catch (UriStructureBadInfoException badInfoEx)
                {
                    return BadRequest($"{{\"error\": \"{badInfoEx.Message}\"}}");
                }
            }
            else
            {
                return BadRequest("{{\"error\": \"info structure is missing\"}}");
            }
        }
    }
}