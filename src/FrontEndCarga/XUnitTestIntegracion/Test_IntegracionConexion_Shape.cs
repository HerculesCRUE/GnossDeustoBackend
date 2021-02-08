using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using System;
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
                ConfigUrlService configUrl = new ConfigUrlService();
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
