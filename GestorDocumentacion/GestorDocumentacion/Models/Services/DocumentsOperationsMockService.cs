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
    public class DocumentsOperationsMockService : IDocumentsOperationsService
    {
        private static string path = "CMS/Documents";
        private IFileOperationService _fileOperationsService;
        private List<Document> _listDocuments;

        public DocumentsOperationsMockService(IFileOperationService fileOperationsService)
        {
            _listDocuments = new List<Document>();
            Document document1 = new Document()
            {
                DocumentId = Guid.NewGuid(),
                Name = "Document1",
                SavedRoute = path
            };
            Document document2 = new Document()
            {
                DocumentId = Guid.NewGuid(),
                Name = "Document2",
                SavedRoute = path
            };
            _listDocuments.Add(document1);
            _listDocuments.Add(document2);
            _fileOperationsService = fileOperationsService;
        }

        /// <summary>
        /// Elimina un objeto documento y el documento enlazado a él
        /// </summary>
        /// <param name="documentId">Identificador del documento</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeleteDocument(Guid documentId)
        {
            Document document = _listDocuments.FirstOrDefault(document => document.DocumentId.Equals(documentId));
            if (document != null)
            {
                _listDocuments.Remove(document);
                _fileOperationsService.DeleteDocument(document.SavedRoute);
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
            return _listDocuments.FirstOrDefault(documento => documento.Name.Equals(name));
        }

        /// <summary>
        /// Obtiene los bytes de un documento
        /// </summary>
        /// <param name="documentId">Identificador del documento a obtener</param>
        /// <returns>contenido del fichero</returns>
        public byte[] GetDocumentBytes(Guid documentId)
        {
            Document document = _listDocuments.FirstOrDefault(documento => documento.DocumentId.Equals(documentId));
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
            Document document = _listDocuments.FirstOrDefault(documento => documento.DocumentId.Equals(documentId));
            return document;
        }

        /// <summary>
        /// Obtiene una lista de documentos
        /// </summary>
        /// <returns>Lista de objetos documento</returns>
        public List<Document> GetDocuments()
        {
            return _listDocuments;
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
                    _listDocuments.Add(document);

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
