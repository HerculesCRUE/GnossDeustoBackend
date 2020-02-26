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

namespace UrisFactory.Controllers
{
    [ApiController]
    [Route("[Controller]")]
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
        ///Return the Config file schema
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
        ///Replace the Config file schema
        ///</summary>
        ///<param name="newSchemaConfig">new config file schema</param>
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
        ///Return the uri structure and the resource class asociated with the uri structure
        ///</summary>
        ///<param name="name">name of the uri structure</param>
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
                ResourcesClass resourceClass = _configJsonHandler.GetResourceClass(name);
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
        ///Delete the uri structure and the resource class asociated with the uri structure
        ///</summary>
        ///<param name="name">name of the uri structure</param>
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
        ///Add an uri structure and a resource class asociated with the uri structure
        ///</summary>
        ///<param name="infoUriStructure">uri structure and the resource class to add</param>
        [HttpPut]
        [SwaggerRequestExample(typeof(InfoUriStructure), typeof(UriStructureInfoRequest))]
        [SwaggerResponse(StatusCodes.Status200OK, "Example", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddUriStructureResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Example", typeof(UriErrorExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AddUriStructureErrorResponse))]
        public IActionResult AddUriStructure(InfoUriStructure infoUriStructure)
        {
            if(infoUriStructure != null && infoUriStructure.ResourcesClass != null && infoUriStructure.UriStructure != null)
            {
                try
                {
                    _configJsonHandler.AddUriStructureInfo(infoUriStructure.UriStructure, infoUriStructure.ResourcesClass);
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