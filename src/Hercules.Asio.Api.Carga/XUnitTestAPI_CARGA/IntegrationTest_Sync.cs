// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto H�rcules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
//Test de integraci�n con llamadas etl
using API_CARGA.Models.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class IntegrationTest_Sync
    {
        //[Fact]
        //public void TestListIdentifier()
        //{

        //    ConfigUrlService urlService = new ConfigUrlService();
        //    //urlService.Url = "http://herc-as-front-desa.atica.um.es/carga/";
        //    ConfigTokenService configTokenService = new ConfigTokenService();
        //    CallTokenService callTokenService = new CallTokenService(configTokenService);
        //    CallMockNeedPublishData api = new CallMockNeedPublishData();
        //    OaiPublishRDFService oaiPublish = new OaiPublishRDFService(null, api, callTokenService);
        //    var lista = oaiPublish.CallListIdentifier(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"));
        //    Assert.True(lista.Count > 0);
        //}

        //[Fact]
        //public void TestGetRecord()
        //{
        //    ConfigUrlService urlService = new ConfigUrlService();
        //    //urlService.Url = "http://herc-as-front-desa.atica.um.es/carga/";
        //    CallMockNeedPublishData api = new CallMockNeedPublishData();
        //    ConfigTokenService configTokenService = new ConfigTokenService();
        //    CallTokenService callTokenService = new CallTokenService(configTokenService);
        //    OaiPublishRDFService oaiPublish = new OaiPublishRDFService(null, api, callTokenService);
        //    string rdf = oaiPublish.CallGetRecord(new Guid("5efac0ad-ec4e-467d-bbf5-ce3f64edb46a"), "1");
        //    Assert.True(!string.IsNullOrEmpty(rdf));
        //}
    }
}
