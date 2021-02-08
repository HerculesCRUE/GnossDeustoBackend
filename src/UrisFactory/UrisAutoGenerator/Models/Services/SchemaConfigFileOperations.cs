// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la gestión de las operaciones con el fichero de configuración
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    ///<summary>
    ///Clase para la gestión de las operaciones con el fichero de configuración
    ///</summary>
    public class SchemaConfigFileOperations : ISchemaConfigOperations
    {
        private static string configPath = "Config/UrisConfig.json";
        private static string oldConfigPath = "Config/oldUrisConfig.json";

        private ConfigJsonHandler _configJsonHandler;
        public SchemaConfigFileOperations(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }

        ///<summary>
        ///Obtiene el content type del fichero
        ///</summary>
        public string GetContentType()
        {
            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(configPath), out contentType);
            return contentType;
        }

        ///<summary>
        ///Obtiene los bytes con el fichero de configuracion
        ///</summary>
        public byte[] GetFileSchemaData()
        {
            return File.ReadAllBytes(configPath);
            //catch (Exception ex)
            //{
            //    string message = $"{ex.Message}-----------------------{ex.InnerException}";
            //    return Encoding.Unicode.GetBytes(message);
            //}
        }

        ///<summary>
        ///Guarda el fichero de configuracion
        ///</summary>
        ///<param name="formFile">cadena de texto emulando el json del fichero de configuracion</param>
        public bool SaveConfigFile(IFormFile formFile)
        {
            var stream = CreateStream();
            formFile.CopyTo(stream);
            stream.Close();
            bool savedCorrectly = replacePreviousSchemaConfig(stream);
            return savedCorrectly;
        }

        ///<summary>
        ///Guarda el fichero de configuracion
        ///</summary>
        public bool SaveConfigJson()
        {
            UriStructureGeneral uriSchema = _configJsonHandler.GetUrisConfig();
            string uriSchemaJson = JsonConvert.SerializeObject(uriSchema);
            var stream = CreateStream();
            byte[] data = new UTF8Encoding(true).GetBytes(uriSchemaJson);
            stream.Write(data, 0, data.Length);
            stream.Close();
            bool saved = replacePreviousSchemaConfig(stream);
            return saved;
        }

        ///<summary>
        ///Crea un file stream para su posterior uso
        ///</summary>

        private FileStream CreateStream()
        {
            File.Move(configPath, oldConfigPath);
            return File.Create(configPath);
        }

        ///<summary>
        ///Reemplaza la copia de seguridad que se ha hecho del anterior esquema por un esquema nuevo
        ///</summary>
        ///<param name="stream">file stream del fichero</param>
        private bool replacePreviousSchemaConfig(FileStream stream)
        {
            bool replaced = false;
            try
            {
                _configJsonHandler.LoadConfigJson();
                File.Delete(oldConfigPath);
                replaced = true;
            }
            catch (FailedLoadConfigJsonException)
            {
                try
                {
                    File.Delete(configPath);
                }
                catch (IOException)
                {
                    stream.Close();
                    File.Delete(configPath);
                }
                File.Move(oldConfigPath, configPath);
                replaced = false;
            }
            return replaced;
        }
    }
}
