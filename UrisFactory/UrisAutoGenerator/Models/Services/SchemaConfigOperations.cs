using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Models.Services
{
    public static class SchemaConfigOperations
    {
        private static string configPath = "config/UrisConfig.json";

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
    }
}
