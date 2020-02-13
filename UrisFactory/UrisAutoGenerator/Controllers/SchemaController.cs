using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Entities;
using UrisFactory.Models.Services;

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
        public IActionResult ReplaceSchemaConfig(IFormFile newSchemaConfig)
        {
            bool result = _schemaConfigOperations.SaveConfigFile(newSchemaConfig);
            if (result)
            {
                return Ok("new config file loaded");
            }
            else
            {
                return BadRequest("Error: new config file is not correctly formed.");
            }
        }

        ///<summary>
        ///Return the uri structure and the resource class asociated with the uri structure
        ///</summary>
        ///<param name="name">name of the uri structure</param>
        [HttpGet("{name}")]
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
                return BadRequest($"No data of uriStructure {name}");
            }
        }

        ///<summary>
        ///Delete the uri structure and the resource class asociated with the uri structure
        ///</summary>
        ///<param name="name">name of the uri structure</param>
        [HttpDelete]
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
                return BadRequest($"No data of uriStructure {name}");
            }
        }

        ///<summary>
        ///Add an uri structure and a resource class asociated with the uri structure
        ///</summary>
        ///<param name="infoUriStructure">uri structure and the resource class to add</param>
        [HttpPut]
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
                        return Ok($"uriStructure: {infoUriStructure.UriStructure.Name} has been deleted and the new config schema is loaded");
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
                    return BadRequest(badInfoEx.Message);
                }
            }
            else
            {
                return BadRequest("info structure is missing");
            }
        }
    }
}