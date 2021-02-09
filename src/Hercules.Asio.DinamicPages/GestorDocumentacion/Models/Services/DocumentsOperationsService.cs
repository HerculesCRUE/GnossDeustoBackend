// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la gestión de los documentos
using GestorDocumentacion.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Clase para la gestión de los documentos
    /// </summary>
    public class DocumentsOperationsService : IDocumentsOperationsService
    {
        private static string path = "CMS/Documents";
        private readonly EntityContext _context;
        private IFileOperationService _fileOperationsService;

        public DocumentsOperationsService(EntityContext context, IFileOperationService fileOperationsService)
        {
            _context = context;
             _fileOperationsService = fileOperationsService;
        }

        /// <summary>
        /// Elimina un objeto documento y el documento enlazado a él
        /// </summary>
        /// <param name="documentId">Identificador del documento</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeleteDocument(Guid documentId)
        {
            Document page = _context.Document.FirstOrDefault(document => document.DocumentId.Equals(documentId));
            if (page != null)
            {
                _context.Entry(page).State = EntityState.Deleted;
                _fileOperationsService.DeleteDocument(page.SavedRoute);
                _context.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Obtiene un documento por su nombre
        /// </summary>
        /// <param name="name">Nombre del documento a obtener</param>
        /// <returns>Un objeto documento</returns>
        public Document GetDocument(string name)
        {
            return _context.Document.FirstOrDefault(documento => documento.Name.Equals(name));
        }

        /// <summary>
        /// Obtiene los bytes de un documento
        /// </summary>
        /// <param name="documentId">Identificador del documento a obtener</param>
        /// <returns>contenido del fichero</returns>
        public byte[] GetDocumentBytes(Guid documentId)
        {
            Document document = _context.Document.FirstOrDefault(documento => documento.DocumentId.Equals(documentId));
            byte[] data = null;
            if (document != null)
            {
                data = _fileOperationsService.ReadDocument(document.SavedRoute);
            }
            return data;
        }

        /// <summary>
        /// Obtiene un documento por su Identificador
        /// </summary>
        /// <param name="documentId">Identificador del documento a obtener</param>
        /// <returns>Un objeto documento</returns>
        public Document GetDocument(Guid documentId)
        {
            Document document = _context.Document.FirstOrDefault(documento => documento.DocumentId.Equals(documentId));
            return document;
        }

        /// <summary>
        /// Obtiene una lista de documentos
        /// </summary>
        /// <returns>Lista de objetos documento</returns>
        public List<Document> GetDocuments()
        {
            return _context.Document.ToList();
        }

        /// <summary>
        /// Carga o modifica un documento
        /// </summary>
        /// <param name="document">Documento nuevo o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <param name="documentFile">Contenido del fichero</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadDocument(Document document, bool isNew, IFormFile documentFile)
        {
            document.SavedRoute = $"{path}/{document.DocumentId}.pdf";
            if (isNew)
            {
                if (document != null && !string.IsNullOrEmpty(document.Name) && GetDocument(document.Name) == null)
                {
                    _context.Document.Add(document);
                    _context.SaveChanges();
                    
                    _fileOperationsService.SaveDocument(document.SavedRoute, documentFile);
                    return true;
                }
            }
            else
            {
                var documentModify = GetDocument(document.DocumentId);
                if (documentFile != null)
                {
                    _fileOperationsService.DeleteDocument(document.SavedRoute);
                    _fileOperationsService.SaveDocument(document.SavedRoute, documentFile);
                }
                if (!string.IsNullOrEmpty(document.Name) && document.Name != documentModify.Name)
                {
                    if (GetDocument(document.Name) == null)
                    {
                        documentModify.Name = document.Name;
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                
            }
            return false;
            
        }

        /// <summary>
        /// Obtiene el nombre del documento y su contenido
        /// </summary>
        /// <param name="documentId">Identificador del documento</param>
        /// <returns>Un diccionario en el que en la clave es el nombre del documento y el valor el contenido del fichero</returns>
        public Dictionary<string, byte[]> GetDocumentInfo(Guid documentId)
        {
            Dictionary<string, byte[]> docInfo = new Dictionary<string, byte[]>();
            docInfo.Add(GetDocument(documentId).Name, GetDocumentBytes(documentId));
            return docInfo;
        }
    }
}
