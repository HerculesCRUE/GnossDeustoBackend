using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Xunit;

namespace XUnitTestIntegracion
{
    public class UnitTest_Repository
    {
        [Fact]
        public void TestConexionListadoRepositoryConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallRepositoryConfigApiService callRepository = new CallRepositoryConfigApiService(callService, null, configUrl);
                var resultado = callRepository.GetRepositoryConfigs();
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestConexionGetRepositoryConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallRepositoryConfigApiService callRepository = new CallRepositoryConfigApiService(callService, null, configUrl);
                Guid id = Guid.NewGuid();
                var resultado = callRepository.GetRepositoryConfig(id);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestConexionDeleteRepositoryConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallRepositoryConfigApiService callRepository = new CallRepositoryConfigApiService(callService, null, configUrl);
                Guid id = Guid.NewGuid();
                var resultado = callRepository.DeleteRepositoryConfig(id);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestConexionCreateRepositoryConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallRepositoryConfigApiService callRepository = new CallRepositoryConfigApiService(callService, null, configUrl);
                RepositoryConfigViewModel item = new RepositoryConfigViewModel()
                {
                    Name = "Prueba",
                    OauthToken = "qyueu11",
                    Url = "url/prueba"
                };
                var resultado = callRepository.CreateRepositoryConfigView(item);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestConexionModifyRepositoryConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.Development.json");

                IConfiguration Configuration = builder.Build();

                ConfigUrlService configUrl = new ConfigUrlService(Configuration);
                CallApiService callService = new CallApiService();
                CallRepositoryConfigApiService callRepository = new CallRepositoryConfigApiService(callService, null, configUrl);
                RepositoryConfigViewModel item = new RepositoryConfigViewModel()
                {
                    Name = "Prueba_",
                    OauthToken = "qyueu11",
                    Url = "url/prueba"
                };
                var resultado = callRepository.CreateRepositoryConfigView(item);
                resultado.Name = "Modificado";
                callRepository.ModifyRepositoryConfig(resultado);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }
    }
}
