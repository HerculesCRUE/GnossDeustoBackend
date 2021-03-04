using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Xunit;

namespace XUnitTestIntegracion
{
    public class UnitTest_Shape
    {
        [Fact]
        public void TestConexionListadoShapeConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService, null, configUrl);
                var resultado = callRepository.GetShapeConfigs();
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestConexionGetShapeConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService, null, configUrl);
                Guid id = Guid.NewGuid();
                var resultado = callRepository.GetShapeConfig(id);
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestConexionDeleteShapeConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService, null, configUrl);
                Guid id = Guid.NewGuid();
                var resultado = callRepository.DeleteShapeConfig(id);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }
       
    }
}
