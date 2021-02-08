// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Tests de la gestión de pages
using GestorDocumentacion.Models;
using GestorDocumentacion.Models.Entities;
using GestorDocumentacion.Models.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace XUnitTestGestorDocumentacion
{
    public class UnitTestTemplatesOperation
    {
        [Fact]
        public void ListPageOk()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            Assert.True(mock.GetPages().Count > 0);
        }

        [Fact]
        public void AddPageOk()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            int countOld = mock.GetPages().Count;
            Page pageNew = new Page()
            {
                Content = "<p>hola</p>",
                Route = "pages/newPage.html",
                PageID = Guid.NewGuid()
            };
            bool added =  mock.LoadPage(pageNew, true);
            Assert.True(mock.GetPages().Count > countOld && added);
        }

        [Fact]
        public void AddPageFalse()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            int countOld = mock.GetPages().Count;
            Page pageNew = new Page()
            {
                Content = "<p>hola</p>",
                Route = "/pages/page1.html",
                PageID = Guid.NewGuid()
            };
            bool added = mock.LoadPage(pageNew, true);
            Assert.True(mock.GetPages().Count == countOld && !added);
        }

        [Fact]
        public void DeletePageOk()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            int countOld = mock.GetPages().Count;
            mock.DeletePage(mock.GetPages().FirstOrDefault().PageID);
            Assert.True(countOld > mock.GetPages().Count);
        }

        [Fact]
        public void GetPageByName()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            Assert.True(mock.GetPage("/pages/page1.html") != null);
        }

        [Fact]
        public void GetPageById()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            Assert.True(mock.GetPage(mock.GetPages().FirstOrDefault().PageID) != null);
        }

        [Fact]
        public void ModifyPageOk()
        {
            PagesOperationServiceMockService mock = new PagesOperationServiceMockService();
            Page page = new Page()
            {
                PageID = mock.GetPages().FirstOrDefault().PageID,
                Route = "/modify/route"
            };
            bool added = mock.LoadPage(page, false);
            Assert.True(mock.GetPage(page.Route)!=null);
        }
    }
}
