// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz  para la gestión de los documentos
using GestorDocumentacion.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Interfaz para el servicio de los documentos
    /// </summary>
    public interface IDocumentsOperationsService
    {
        /// <summary>
        /// Elimina un objeto documento y el documento enlazado a él
        /// </summary>
        /// <param name="documentId">Identificador del documento</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeleteDocument(Guid documentId);
        /// <summary>
        /// Obtiene un documento por su nombre
        /// </summary>
        /// <param name="name">Nombre del documento a obtener</param>
        /// <returns>Un objeto documento</returns>
        public Document GetDocument(string name);
        /// <summary>
        /// Obtiene los bytes de un documento
        /// </summary>
        /// <param name="documentId">Identificador del documento a obtener</param>
        /// <returns>contenido del fichero</returns>
        public byte[] GetDocumentBytes(Guid documentId);
        /// <summary>
        /// Obtiene un documento por su Identificador
        /// </summary>
        /// <param name="documentId">Identificador del documento a obtener</param>
        /// <returns>Un objeto documento</returns>
        public Document GetDocument(Guid documentId);
        /// <summary>
        /// Obtiene una lista de documentos
        /// </summary>
        /// <returns>Lista de objetos documento</returns>
        public List<Document> GetDocuments();
        /// <summary>
        /// Obtiene el nombre del documento y su contenido
        /// </summary>
        /// <param name="documentId">Identificador del documento</param>
        /// <returns>Un diccionario en el que en la clave es el nombre del documento y el valor el contenido del fichero</returns>
        public Dictionary<string, byte[]> GetDocumentInfo(Guid documentId);
        /// <summary>
        /// Carga o modifica un documento
        /// </summary>
        /// <param name="document">Documento nuevo o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <param name="documentFile">Contenido del fichero</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadDocument(Document document, bool isNew, IFormFile documentFile);
    }
}
