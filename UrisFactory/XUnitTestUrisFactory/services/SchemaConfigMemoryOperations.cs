using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using XUnitTestUrisFactory.Properties;

namespace XUnitTestUrisFactory.services
{
    public class SchemaConfigMemoryOperations: ISchemaConfigOperations
    {
        private ConfigJsonHandler _configJsonHandler;
        public SchemaConfigMemoryOperations(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

        public string GetContentType()
        {
            return "application/json";
        }

        public byte[] GetFileSchemaData()
        {
            return Resources.UrisConfig; 
        }

        public bool SaveConfigFile(IFormFile formFile)
        {
            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            string texto = Encoding.UTF8.GetString(bytes);
            UriStructureGeneral uriStructureGeneral  = ReaderConfigJson.Read(texto);
            return ConfigJsonHandler.IsCorrectFormedUriStructure(uriStructureGeneral);
        }

        public bool SaveConfigJson()
        {
            return ConfigJsonHandler.IsCorrectFormedUriStructure(_configJsonHandler.GetUrisConfig());
        }
        
        public MemoryStream CreateStream(IFormFile formFile)
        {
            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            return memoryStream;
        }


        
    }
}
