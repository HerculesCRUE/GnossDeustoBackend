using GestorDocumentacion.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public class DocumentsOperationsService : IDocumentsOperationsService
    {
        private static string path = "CMS/Documents";
        private readonly EntityContext _context;
        private FileOperationsService _fileOperationsService;
        public DocumentsOperationsService(EntityContext context, FileOperationsService fileOperationsService)
        {
            _context = context;
             _fileOperationsService = fileOperationsService;
        }
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

        public Document GetDocument(string name)
        {
            return _context.Document.FirstOrDefault(documento => documento.Name.Equals(name));
        }

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
        public Document GetDocument(Guid documentId)
        {
            Document document = _context.Document.FirstOrDefault(documento => documento.DocumentId.Equals(documentId));
            return document;
        }

        public List<Document> GetDocuments()
        {
            return _context.Document.ToList();
        }

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
                    if (GetDocument(document.Name) != null)
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

        public Dictionary<string, byte[]> GetDocumentInfo(Guid documentId)
        {
            Dictionary<string, byte[]> docInfo = new Dictionary<string, byte[]>();
            docInfo.Add(GetDocument(documentId).Name, GetDocumentBytes(documentId));
            return docInfo;
        }
    }
}
