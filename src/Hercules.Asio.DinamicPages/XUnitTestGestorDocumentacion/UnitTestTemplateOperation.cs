// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Tests de la gestión de templates
using GestorDocumentacion.Models;
using GestorDocumentacion.Models.Entities;
using GestorDocumentacion.Models.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace XUnitTestGestorDocumentacion
{
    public class UnitTestTemplateOperation
    {
        [Fact]
        public void ListTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            Assert.True(mock.GetTemplates().Count > 0);
        }

        [Fact]
        public void AddTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            int countOld = mock.GetTemplates().Count;
            Template templateNew = new Template()
            {
                Name = "TemplateNew",
                Content = "<p>hola</p>",
                TemplateID = Guid.NewGuid()
            };
            bool added = mock.LoadTemplate(templateNew, true);
            Assert.True(mock.GetTemplates().Count > countOld && added);
        }

        [Fact]
        public void AddTemplateFalse()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            int countOld = mock.GetTemplates().Count;
            Template templateNew = new Template()
            {
                Name = "Template1",
                Content = "<p>hola</p>",
                TemplateID = Guid.NewGuid()
            };
            bool added = mock.LoadTemplate(templateNew, true);
            Assert.True(mock.GetTemplates().Count == countOld && !added);
        }

        [Fact]
        public void DeleteTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            int countOld = mock.GetTemplates().Count;
            mock.DeleteTemplate(mock.GetTemplates().FirstOrDefault().TemplateID);
            Assert.True(countOld > mock.GetTemplates().Count);
        }

        [Fact]
        public void GetTemplateByName()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            Assert.True(mock.GetTemplate("Template1") != null);
        }

        [Fact]
        public void GetTemplateById()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            Assert.True(mock.GetTemplate(mock.GetTemplates().FirstOrDefault().TemplateID) != null);
        }

        [Fact]
        public void ModifyTemplateOk()
        {
            TemplatesOperationMockService mock = new TemplatesOperationMockService();
            Template template = new Template()
            {
                TemplateID = mock.GetTemplates().FirstOrDefault().TemplateID,
                Name = "Modify Template"
            };
            bool added = mock.LoadTemplate(template, false);
            Assert.True(mock.GetTemplate(template.Name) != null);
        }
    }
}
