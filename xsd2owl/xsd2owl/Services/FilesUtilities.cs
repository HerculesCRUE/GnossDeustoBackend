using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace xsd2owl.Services
{
    public static class FilesUtilities
    {
        public static string xsdUploadFolder = "uploadedXSD";
        public async static Task<string> SaveFormFile(string host, IFormFile formFile)
        {
            string urlFile = "";
            var filePathTemp = Path.GetTempFileName();
            string[] fileProper = filePathTemp.Split('\\').Last().Split('.');
            string fileName = $"{fileProper[0]}.xsd";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\{xsdUploadFolder}", fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await formFile.CopyToAsync(stream);
            }

            urlFile = $"{host}/{xsdUploadFolder}/{fileName}";
            return urlFile;
        }
    }
}
