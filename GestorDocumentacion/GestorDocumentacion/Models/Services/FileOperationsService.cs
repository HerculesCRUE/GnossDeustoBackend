using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public class FileOperationsService
    {
        
        public string ReadFile(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            return result.ToString();
        }

        public void DeleteDocument(string route)
        {
            string routeFile = route;
            File.Delete(routeFile);
        }

        public void SaveDocument(string route, IFormFile document)
        {
            string routeFile = route;
            var stream = File.Create(routeFile);
            document.CopyTo(stream);
            stream.Close();
        }

        public byte[] ReadDocument(string route)
        {
            var data = File.ReadAllBytes(route);
            return data;
        }
    }
}
