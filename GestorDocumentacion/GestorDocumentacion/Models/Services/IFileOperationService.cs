// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para la gestión de archivos
using Microsoft.AspNetCore.Http;

namespace GestorDocumentacion.Models.Services
{
    public interface IFileOperationService
    {
        /// <summary>
        /// Devuelve el contenido en texto de un fichero
        /// </summary>
        /// <param name="file">Fichero a leer</param>
        public string ReadFile(IFormFile file);
        /// <summary>
        /// Elimina un fichero
        /// </summary>
        /// <param name="route">Ruta del fichero</param>
        public void DeleteDocument(string route);
        /// <summary>
        /// Guarda un fichero
        /// </summary>
        /// <param name="route">Ruta donde guardar</param>
        /// <param name="document">documento a guardar</param>
        public void SaveDocument(string route, IFormFile document);
        /// <summary>
        /// Lee los bytes de un fichero
        /// </summary>
        /// <param name="route">Ruta del fichero</param>
        public byte[] ReadDocument(string route);
    }
}