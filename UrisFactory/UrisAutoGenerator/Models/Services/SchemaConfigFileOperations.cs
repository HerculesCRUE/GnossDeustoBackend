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
    public static class SchemaConfigFileOperations
    {
        private static string configPath = "config/UrisConfig.json";
        private static string oldConfigPath = "config/oldUrisConfig.json";

        public static string GetContentType()
        {
            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(configPath), out contentType);
            return contentType;
        }

        public static byte[] GetFileData()
        {
            return File.ReadAllBytes(configPath);
        }

        public static bool SaveConfigFile(IFormFile formFile)
        {
            var stream = CreateStream();
            formFile.CopyTo(stream);
            stream.Close();
            bool savedCorrectly = replacePreviousSchemaConfig(stream);
            return savedCorrectly;
        }

        public static bool SaveConfigJsonInConfigFile()
        {
            UriStructureGeneral uriSchema = ConfigJsonHandler.GetUrisConfig();
            string uriSchemaJson = JsonConvert.SerializeObject(uriSchema);
            var stream = CreateStream();
            byte[] data = new UTF8Encoding(true).GetBytes(uriSchemaJson);
            stream.Write(data, 0, data.Length);
            stream.Close();
            bool saved = replacePreviousSchemaConfig(stream);
            return saved;
        }

        private static FileStream CreateStream()
        {
            File.Move(configPath, oldConfigPath);
            return File.Create(configPath);
        }

        private static bool replacePreviousSchemaConfig(FileStream stream)
        {
            bool replaced = false;
            try
            {
                ConfigJsonHandler.LoadConfigJson();
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
