// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Tests de la gestión de documents
using GestorDocumentacion.Models;
using GestorDocumentacion.Models.Entities;
using GestorDocumentacion.Models.Services;
using System.Linq;
using Xunit;

namespace XUnitTestGestorDocumentacion
{
    public class UnitTestDocumentsOperation
    {
        [Fact]
        public void ListPageOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            Assert.True(mock.GetDocuments().Count > 0);
        }

        [Fact]
        public void AddPageOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            int countOld = mock.GetDocuments().Count;
            Document documentNew = new Document()
            {
                Name = "DocumentNew",
                SavedRoute = "/Documents"
            };
            bool added = mock.LoadDocument(documentNew, true, null);
            Assert.True(mock.GetDocuments().Count > countOld && added);
        }

        [Fact]
        public void AddPageFalse()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            int countOld = mock.GetDocuments().Count;
            Document documentNew = new Document()
            {
                Name = "Document1",
                SavedRoute = "/Documnet"
            };
            bool added = mock.LoadDocument(documentNew, true, null);
            Assert.True(mock.GetDocuments().Count == countOld && !added);
        }

        [Fact]
        public void DeletePageOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            int countOld = mock.GetDocuments().Count;
            mock.DeleteDocument(mock.GetDocuments().FirstOrDefault().DocumentId);
            Assert.True(countOld > mock.GetDocuments().Count);
        }

        [Fact]
        public void GetPageByName()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            Assert.True(mock.GetDocument("Document1") != null);
        }

        [Fact]
        public void GetPageById()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            Assert.True(mock.GetDocument(mock.GetDocuments().FirstOrDefault().DocumentId) != null);
        }

        [Fact]
        public void ModifyPageOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            Document document = new Document()
            {
                DocumentId = mock.GetDocuments().FirstOrDefault().DocumentId,
                Name = "Modify Document"
            };
            bool added = mock.LoadDocument(document, false, null);
            Assert.True(mock.GetDocument(document.Name) != null);
        }

        [Fact]
        public void GetDocumentInfo()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            Document document = new Document()
            {
                DocumentId = mock.GetDocuments().FirstOrDefault().DocumentId,
                Name = "Modify Document"
            };
            var result = mock.GetDocumentInfo(mock.GetDocuments().FirstOrDefault().DocumentId);
            Assert.True(result.Count>0);
        }
    }
}
