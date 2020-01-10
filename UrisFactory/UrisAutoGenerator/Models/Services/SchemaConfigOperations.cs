using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UrisFactory.Extra.Exceptions;

namespace UrisFactory.Models.Services
{
    public static class SchemaConfigOperations
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
            bool savedCorrectly = false;
            File.Move(configPath, oldConfigPath);
            var stream = File.Create(configPath);
            formFile.CopyTo(stream);
            stream.Close();
            try
            {
                ConfigJsonHandler.LoadConfigJson();
                File.Delete(oldConfigPath);
                savedCorrectly = true;
            }
            catch(FailedLoadConfigJsonException)
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
                savedCorrectly = false;
            }
            return savedCorrectly;
        }
    }
}
