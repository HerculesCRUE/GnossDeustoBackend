// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Tests del controlador de templates
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
    public class TestTemplateController
    {
        [Fact]
        public void ListTemplatesOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            TemplateController controllerMock = new TemplateController(mock, mockFile);
            List<TemplateViewModel> pages = (List<TemplateViewModel>)((OkObjectResult)controllerMock.GetTemplates()).Value;
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
        public void AddTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            TemplateController controllerMock = new TemplateController(mock, mockFile);
            int countOld = mock.GetTemplates().Count;
            var added = controllerMock.LoadTemplate("TemplateNew", Guid.Empty, null);
            Assert.True(mock.GetTemplates().Count > countOld);
        }


        [Fact]
        public void DeleteTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            TemplateController controllerMock = new TemplateController(mock, mockFile);
            int countOld = mock.GetTemplates().Count;
            controllerMock.DeleteTemplate(mock.GetTemplates().FirstOrDefault().TemplateID);
            Assert.True(mock.GetTemplates().Count < countOld);
        }


        [Fact]
        public void GetTemplateById()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            TemplateController controllerMock = new TemplateController(mock, mockFile);
            int countOld = mock.GetTemplates().Count;
            var result = ((OkObjectResult)controllerMock.GetTemplate(mock.GetTemplates().FirstOrDefault().TemplateID)).Value;
            Assert.True(result != null);
        }

        [Fact]
        public void ModifyTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            FileOperationMockService mockFile = new FileOperationMockService();
            TemplateController controllerMock = new TemplateController(mock, mockFile);
            int countOld = mock.GetTemplates().Count;
            var added = controllerMock.LoadTemplate("TemplateModified", mock.GetTemplates().FirstOrDefault().TemplateID, null);
            Assert.True(mock.GetTemplate("TemplateModified") != null);
        }
    }
}
