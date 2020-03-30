using API_CARGA.Models.Services;
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
            OaiPublishRDFService oaiPublish = new OaiPublishRDFService(urlService);
            var lista = oaiPublish.CallListIdentifier(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"));
            Assert.True(lista.Count > 0);
        }

        [Fact]
        public void TestGetRecord()
        {
            List<String> listIdentifier = new List<string>();
            listIdentifier.Add("0000-0001-8055-6823");
            ConfigUrlService urlService = new ConfigUrlService();
            urlService.Url = "http://herc-as-front-desa.atica.um.es/carga/";
            OaiPublishRDFService oaiPublish = new OaiPublishRDFService(urlService);
            var lista = oaiPublish.CallGetRecord(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"), listIdentifier);
            Assert.True(lista.Count > 0);
        }
    }
}
