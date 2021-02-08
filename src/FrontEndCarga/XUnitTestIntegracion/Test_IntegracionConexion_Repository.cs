using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using System;
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
