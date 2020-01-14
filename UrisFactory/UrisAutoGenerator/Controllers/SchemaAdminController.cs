using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            string contentType = "";
            contentType = SchemaConfigOperations.GetContentType();
            return File(SchemaConfigOperations.GetFileData(), contentType);
        }

        [HttpPost]
        public IActionResult ReplaceSchemaConfig(IFormFile newSchemaConfig)
        {
            bool result = SchemaConfigOperations.SaveConfigFile(newSchemaConfig);
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
            UriStructureGeneral uriSchema = ConfigJsonHandler.GetUriStructure();
            UriStructure uri = uriSchema.UriStructures.FirstOrDefault(uriStructure => uriStructure.Name.Equals(name));
            if(uri != null)
            {
                ResourcesClass resourceClass = uriSchema.ResourcesClasses.FirstOrDefault(resourcesClass => resourcesClass.ResourceURI.Equals(name));
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
            UriStructureGeneral uriSchema = ConfigJsonHandler.GetUriStructure();
            UriStructure uri = uriSchema.UriStructures.FirstOrDefault(uriStructure => uriStructure.Name.Equals(name));
            if (uri != null)
            {
                ResourcesClass resourceClass = uriSchema.ResourcesClasses.FirstOrDefault(resourcesClass => resourcesClass.ResourceURI.Equals(name));
                uriSchema.UriStructures.Remove(uri);
                uriSchema.ResourcesClasses.Remove(resourceClass);
                bool deleted = SchemaConfigOperations.SaveConfigJsonInConfigFile();
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
                UriStructureGeneral uriSchema = ConfigJsonHandler.GetUriStructure();
                uriSchema.ResourcesClasses.Add(infoUriStructure.ResourcesClass);
                uriSchema.UriStructures.Add(infoUriStructure.UriStructure);
                bool saved = SchemaConfigOperations.SaveConfigJsonInConfigFile();
                if (saved)
                {
                    return Ok($"uriStructure: {infoUriStructure.UriStructure.Name} has been deleted and the new config schema is loaded");
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest("info structure is missing");
            }
        }
    }
}