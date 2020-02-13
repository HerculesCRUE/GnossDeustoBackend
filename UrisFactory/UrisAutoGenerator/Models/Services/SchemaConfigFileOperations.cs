using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class SchemaConfigFileOperations : ISchemaConfigOperations
    {
        private static string configPath = "config/UrisConfig.json";
        private static string oldConfigPath = "config/oldUrisConfig.json";

        private ConfigJsonHandler _configJsonHandler;
        public SchemaConfigFileOperations(ConfigJsonHandler configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
        }
        public string GetContentType()
        {
            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(configPath), out contentType);
            return contentType;
        }

        public byte[] GetFileSchemaData()
        {
            try
            {
                return File.ReadAllBytes(configPath);
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}-----------------------{ex.InnerException}";
                return Encoding.Unicode.GetBytes(message);
            }
        }

        public bool SaveConfigFile(IFormFile formFile)
        {
            var stream = CreateStream();
            formFile.CopyTo(stream);
            stream.Close();
            bool savedCorrectly = replacePreviousSchemaConfig(stream);
            return savedCorrectly;
        }

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

        private FileStream CreateStream()
        {
            File.Move(configPath, oldConfigPath);
            return File.Create(configPath);
        }

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
