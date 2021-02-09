// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Tests del controlador de document
using GestorDocumentacion.Controllers;
using GestorDocumentacion.Models;
using GestorDocumentacion.Models.Entities;
using GestorDocumentacion.Models.Services;
using GestorDocumentacion.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTestGestorDocumentacion
{
    public class TestDocumentController
    {
        [Fact]
        public void ListDocumentsOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            DocumentController controllerMock = new DocumentController(mock);
            List<Document> pages = (List<Document>)((OkObjectResult)controllerMock.GetDocuments()).Value;
            if (pages.Count > 0)
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void AddDocumenteOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            DocumentController controllerMock = new DocumentController(mock);
            int countOld = mock.GetDocuments().Count;
            var added = controllerMock.LoadDocument("DocumentNew", Guid.Empty, null);
            Assert.True(mock.GetDocuments().Count > countOld);
        }


        [Fact]
        public void DeleteDocumentOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            DocumentController controllerMock = new DocumentController(mock);
            int countOld = mock.GetDocuments().Count;
            controllerMock.DeleteDocument(mock.GetDocuments().FirstOrDefault().DocumentId);
            Assert.True(mock.GetDocuments().Count < countOld);
        }


        [Fact]
        public void GetDocumentById()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            DocumentController controllerMock = new DocumentController(mock);
            int countOld = mock.GetDocuments().Count;
            var result = ((FileContentResult)controllerMock.GetDocument(mock.GetDocuments().FirstOrDefault().DocumentId));
            Assert.True(result != null);
        }

        [Fact]
        public void ModifyDocumentOk()
        {
            FileOperationMockService mockFile = new FileOperationMockService();
            DocumentsOperationsMockService mock = new DocumentsOperationsMockService(mockFile);
            DocumentController controllerMock = new DocumentController(mock);
            controllerMock.LoadDocument("DocumentModified", mock.GetDocuments().FirstOrDefault().DocumentId, null);
            Assert.True(mock.GetDocument("DocumentModified") != null);
        }
    }
}
