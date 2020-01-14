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
    }
}