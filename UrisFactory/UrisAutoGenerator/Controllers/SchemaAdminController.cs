using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}