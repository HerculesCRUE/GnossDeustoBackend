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
                CallApiService callService = new CallApiService(configUrl);
                CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService);
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
                CallApiService callService = new CallApiService(configUrl);
                CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService);
                Guid id = Guid.NewGuid();
                var resultado = callRepository.GetShapeConfig(id);
                Assert.True(true);
            }
            catch (Exception)
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
                CallApiService callService = new CallApiService(configUrl);
                CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService);
                Guid id = Guid.NewGuid();
                var resultado = callRepository.DeleteShapeConfig(id);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

      //  [Fact]
      //  public void TestConexionCreateShapeConfig()
       // {
       //     try
         //   {
        //        ConfigUrlService configUrl = new ConfigUrlService();
        //        CallApiService callService = new CallApiService(configUrl);
        //        CallShapeConfigApiService callRepository = new CallShapeConfigApiService(callService);
        //        ShapeConfigCreateModel item = new ShapeConfigCreateModel()
         //       {
       //             Name = "Prueba",
        //            

        //        };
       //         var resultado = callRepository.CreateShapeConfig(item);
       //         Assert.True(true);
       //     }
       //     catch (Exception)
      //      {
       //         Assert.True(false);
       //     }
       // }

       
    }
}
