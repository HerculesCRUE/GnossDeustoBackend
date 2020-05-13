using API_CARGA.Models;
using API_CARGA.Models.Services;
using API_CARGA.Models.Transport;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestIntegracion
{
    public class UnitTest_OAIPMH
    {
        [Fact]
        public void TestListIdentifier()
        {
            ConfigUrlService urlService = new ConfigUrlService();
            urlService.Url = "http://herc-as-front-desa.atica.um.es/carga/";
            OaiPublishRDFService oaiPublish = new OaiPublishRDFService(urlService, null);
            var lista = oaiPublish.CallListIdentifier(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"));
            Assert.True(lista.Count > 0);
        }

        [Fact]
        public void TestGetRecord()
        {
            List<IdentifierOAIPMH> listIdentifier = new List<IdentifierOAIPMH>();
            listIdentifier.Add(new IdentifierOAIPMH { Identifier = "0000-0001-8055-6823", Fecha = DateTime.Now });
            ConfigUrlService urlService = new ConfigUrlService();
            urlService.Url = "http://herc-as-front-desa.atica.um.es/carga/";
            OaiPublishRDFService oaiPublish = new OaiPublishRDFService(urlService, null);
            var lista = oaiPublish.CallGetRecord(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"), "0000-0001-8055-6823");
            Assert.True(lista.Count > 0);
        }
    }
}
