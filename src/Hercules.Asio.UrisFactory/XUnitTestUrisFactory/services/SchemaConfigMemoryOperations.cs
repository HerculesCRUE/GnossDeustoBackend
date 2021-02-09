// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para hacer las operaciones del esquema de configuración en memoria y hacer los test unitarios
using Microsoft.AspNetCore.Http;
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
