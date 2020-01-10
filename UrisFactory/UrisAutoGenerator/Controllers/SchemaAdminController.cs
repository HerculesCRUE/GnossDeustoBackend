using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

       //public IActionResult ReplaceSchemaConfig(File newSchemaConfig)
       // {

       // }
    }
}