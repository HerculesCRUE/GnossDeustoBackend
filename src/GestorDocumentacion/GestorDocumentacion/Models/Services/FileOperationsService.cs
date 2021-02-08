// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la gestión de archivos
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Clase para la gestión de archivos
    /// </summary>
    public class FileOperationsService : IFileOperationService
    {
        /// <summary>
        /// Devuelve el contenido en texto de un fichero
        /// </summary>
        /// <param name="file">Fichero a leer</param>
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
        /// <summary>
        /// Elimina un fichero
        /// </summary>
        /// <param name="route">Ruta del fichero</param>
        public void DeleteDocument(string route)
        {
            string routeFile = route;
            File.Delete(routeFile);
        }
        /// <summary>
        /// Guarda un fichero
        /// </summary>
        /// <param name="route">Ruta donde guardar</param>
        /// <param name="document">documento a guardar</param>
        public void SaveDocument(string route, IFormFile document)
        {
            string routeFile = route;
            var stream = File.Create(routeFile);
            document.CopyTo(stream);
            stream.Close();
        }
        /// <summary>
        /// Lee los bytes de un fichero
        /// </summary>
        /// <param name="route">Ruta del fichero</param>
        public byte[] ReadDocument(string route)
        {
            var data = File.ReadAllBytes(route);
            return data;
        }
    }
}
