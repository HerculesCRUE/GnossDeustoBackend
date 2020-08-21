using GestorDocumentacion.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public interface IDocumentsOperationsService
    {
        public bool DeleteDocument(Guid documentId);

        public Document GetDocument(string name);

        public byte[] GetDocumentBytes(Guid documentId);
        public Document GetDocument(Guid documentId);

        public List<Document> GetDocuments();
        public Dictionary<string, byte[]> GetDocumentInfo(Guid documentId);

        public bool LoadDocument(Document document, bool isNew, IFormFile documentFile);
    }
}
