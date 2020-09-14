// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test de integración con OAIPMH
using API_CARGA.Controllers;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class IntegrationTest_OAIPMH
    {
        [Fact]
        public void TestGetRecord()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();            
            ConfigTokenService configTokenService = new ConfigTokenService();
            CallTokenService callTokenService = new CallTokenService(configTokenService);
            CallUri callUri = new CallUri(callTokenService);
            //TODO implementar mock de DiscoverItem
            etlController etlController = new etlController(null,repositoriesConfigMockService, shapesConfigMockService, null, callUri, null, null);
            FileContentResult resultesponse = (FileContentResult)etlController.GetRecord(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"), "1", "rdf");
            string respuesta = Encoding.Default.GetString(resultesponse.FileContents);
            XDocument respuestaXML = XDocument.Parse(respuesta.Substring(respuesta.IndexOf("<OAI-PMH ")));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string GetRecord = respuestaXML.Root.Element(nameSpace + "GetRecord").ToString();
            Assert.True(!string.IsNullOrEmpty(GetRecord));
        }

        [Fact]
        public void TestIdentify()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ConfigTokenService configTokenService = new ConfigTokenService();
            CallTokenService callTokenService = new CallTokenService(configTokenService);
            CallUri callUri = new CallUri(callTokenService);
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            //TODO implementar mock de DiscoverItem
            etlController etlController = new etlController(null,repositoriesConfigMockService, shapesConfigMockService, null, callUri, null, null);
            FileContentResult resultesponse = (FileContentResult)etlController.Identify(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"));
            string respuesta = Encoding.Default.GetString(resultesponse.FileContents);
            XDocument respuestaXML = XDocument.Parse(respuesta.Substring(respuesta.IndexOf("<OAI-PMH ")));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string Identify = respuestaXML.Root.Element(nameSpace + "Identify").ToString();
            Assert.True(!string.IsNullOrEmpty(Identify));
        }

        [Fact]
        public void TestListIdentifiers()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ConfigTokenService configTokenService = new ConfigTokenService();
            CallTokenService callTokenService = new CallTokenService(configTokenService);
            CallUri callUri = new CallUri(callTokenService);
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            //TODO implementar mock de DiscoverItem
            etlController etlController = new etlController(null,repositoriesConfigMockService, shapesConfigMockService, null, callUri , null, null);
            FileContentResult resultesponse = (FileContentResult)etlController.ListIdentifiers(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"),"rdf", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            string respuesta = Encoding.Default.GetString(resultesponse.FileContents);
            XDocument respuestaXML = XDocument.Parse(respuesta.Substring(respuesta.IndexOf("<OAI-PMH ")));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListIdentifiers = respuestaXML.Root.Element(nameSpace + "ListIdentifiers").ToString();
            Assert.True(!string.IsNullOrEmpty(ListIdentifiers));
        }

        [Fact]
        public void TestListMetadataFormats()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            ConfigTokenService configTokenService = new ConfigTokenService();
            CallTokenService callTokenService = new CallTokenService(configTokenService);
            CallUri callUri = new CallUri(callTokenService);
            //TODO implementar mock de DiscoverItem
            etlController etlController = new etlController(null,repositoriesConfigMockService, shapesConfigMockService, null, callUri, null, null);
            FileContentResult resultesponse = (FileContentResult)etlController.ListMetadataFormats(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"));
            string respuesta = Encoding.Default.GetString(resultesponse.FileContents);
            XDocument respuestaXML = XDocument.Parse(respuesta.Substring(respuesta.IndexOf("<OAI-PMH ")));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListMetadataFormats = respuestaXML.Root.Element(nameSpace + "ListMetadataFormats").ToString();
            Assert.True(!string.IsNullOrEmpty(ListMetadataFormats));
        }

        [Fact]
        public void TestListRecords()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            ConfigTokenService configTokenService = new ConfigTokenService();
            CallTokenService callTokenService = new CallTokenService(configTokenService);
            CallUri callUri = new CallUri(callTokenService);
            //TODO implementar mock de DiscoverItem
            etlController etlController = new etlController(null,repositoriesConfigMockService, shapesConfigMockService, null, callUri, null, null);
            FileContentResult resultesponse = (FileContentResult)etlController.ListRecords(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"), "rdf", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            string respuesta = Encoding.Default.GetString(resultesponse.FileContents);
            XDocument respuestaXML = XDocument.Parse(respuesta.Substring(respuesta.IndexOf("<OAI-PMH ")));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListRecords = respuestaXML.Root.Element(nameSpace + "ListRecords").ToString();
            Assert.True(!string.IsNullOrEmpty(ListRecords));
        }

        [Fact]
        public void TestListSets()
        {           
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            ConfigTokenService configTokenService = new ConfigTokenService();
            CallTokenService callTokenService = new CallTokenService(configTokenService);
            CallUri callUri = new CallUri(callTokenService);
            //TODO implementar mock de DiscoverItem
            etlController etlController = new etlController(null,repositoriesConfigMockService, shapesConfigMockService, null, callUri, null, null);
            FileContentResult resultesponse = (FileContentResult)etlController.ListSets(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"));
            string respuesta = Encoding.Default.GetString(resultesponse.FileContents);
            XDocument respuestaXML = XDocument.Parse(respuesta.Substring(respuesta.IndexOf("<OAI-PMH ")));
            XNamespace nameSpace = respuestaXML.Root.GetDefaultNamespace();
            string ListSets = respuestaXML.Root.Element(nameSpace + "ListSets").ToString();
            Assert.True(!string.IsNullOrEmpty(ListSets));
        }
    }
}
