// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test unitario
using API_CARGA.Models;
using API_CARGA.Models.Services;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOAIPublishRDF
    {
        [Fact]
        public void GetRepository()
        {
            try
            {
                var options = new DbContextOptionsBuilder<EntityContext>().UseInMemoryDatabase(databaseName: "MockDataBase").Options;
                EntityContext context = new EntityContext(options, true);
                OaiPublishRDFService rdfService = new OaiPublishRDFService(context, new CallMockNeedPublishData(), null, new ConfigUrlService());
                rdfService.PublishRepositories(Guid.NewGuid());
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
           
        }

        

    }
}
