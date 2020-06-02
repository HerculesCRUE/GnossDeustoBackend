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
                OaiPublishRDFService rdfService = new OaiPublishRDFService(context, new CallMockNeedPublishData(), null);
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
