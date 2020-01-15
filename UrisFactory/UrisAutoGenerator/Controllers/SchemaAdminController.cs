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
    public class SchemaAdminController : Controller
    {        
        [HttpGet(Name="getSchema")]
        public FileResult GetSchema()
        {
            string contentType = SchemaConfigFileOperations.GetContentType();
            return File(SchemaConfigFileOperations.GetFileSchemaData(), contentType);
        }

        [HttpPost]
        public IActionResult ReplaceSchemaConfig(IFormFile newSchemaConfig)
        {
            bool result = SchemaConfigFileOperations.SaveConfigFile(newSchemaConfig);
            if (result)
            {
                return Ok("new config file loaded");
            }
            else
            {
                return BadRequest("Error: new config file is not correctly formed.");
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetUriStructureInfo(string name)
        {
            UriStructure uri = ConfigJsonHandler.GetUriStructure(name);
            if (uri != null)
            {
                ResourcesClass resourceClass = ConfigJsonHandler.GetResourceClass(name);
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

        [HttpDelete]
        public IActionResult DeleteUriStructure(string name)
        {
            if (ConfigJsonHandler.ExistUriStructure(name))
            {
                ConfigJsonHandler.DeleteUriStructureInfo(name);
                bool deleted = SchemaConfigFileOperations.SaveConfigJsonInConfigFile();
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

        [HttpPut]
        public IActionResult AddUriStructure(InfoUriStructure infoUriStructure)
        {
            if(infoUriStructure != null && infoUriStructure.ResourcesClass != null && infoUriStructure.UriStructure != null)
            {
                try
                {
                    ConfigJsonHandler.AddUriStructureInfo(infoUriStructure.UriStructure, infoUriStructure.ResourcesClass);

                    bool saved = SchemaConfigFileOperations.SaveConfigJsonInConfigFile();
                    if (saved)
                    {
                        return Ok($"uriStructure: {infoUriStructure.UriStructure.Name} has been deleted and the new config schema is loaded");
                    }
                    else
                    {
                        return Problem(detail: "Server error has ocurred", null, 500);
                    }
                }
                catch(UriStructureConfiguredException)
                {
                    return BadRequest($"UriStructure {infoUriStructure.UriStructure.Name} already exist");
                }
                catch (UriStructureBadInfoException)
                {
                    return BadRequest($"UriStructure name {infoUriStructure.UriStructure.Name} and ResourcesClass ResourceURI{infoUriStructure.ResourcesClass.ResourceURI} no match, or a data component is empty");
                }
            }
            else
            {
                return BadRequest("info structure is missing");
            }
        }
    }
}