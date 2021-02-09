// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Tests del controlador de page
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
    public class TestPageController
    {
        [Fact]
        public void ListPageOk()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            PageController controllerMock = new PageController(mock,mockFile);
            List<PageViewModel> pages = (List<PageViewModel>)((OkObjectResult)controllerMock.GetPages()).Value;
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
        public void DeletePageOk()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            PageController controllerMock = new PageController(mock, mockFile);
            int countOld = mock.GetPages().Count;
            controllerMock.DeletePage(mock.GetPages().FirstOrDefault().PageID);
            Assert.True(mock.GetPages().Count < countOld);
        }


        [Fact]
        public void GetPageById()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            PageController controllerMock = new PageController(mock, mockFile);
            int countOld = mock.GetPages().Count;
            var result = ((OkObjectResult)controllerMock.GetPage(mock.GetPages().FirstOrDefault().Route)).Value;
            Assert.True(result != null);
        }
    }
}
