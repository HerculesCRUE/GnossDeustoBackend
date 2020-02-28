using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOperationsShapeConfig
    {
        [Fact]
        public void GetConfigRepository()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            List<ShapeConfig> listaRepositorios = shapesConfigMockService.GetShapesConfigs();
            if (listaRepositorios.Count > 0)
            {
                ShapeConfig shapeConfig = listaRepositorios[0];
                ShapeConfig shapeConfigGetByID = shapesConfigMockService.GetShapeConfigById(shapeConfig.ShapeConfigID);
                Assert.True(shapeConfig.Name.Equals(shapeConfigGetByID.Name));
            }

        }

        [Fact]
        public void DeleteConfigRepository()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ShapeConfig shapeConfig = shapesConfigMockService.GetShapeConfigByName("ShapeConfig_1");
            shapesConfigMockService.RemoveShapeConfig(shapeConfig.ShapeConfigID);
            shapeConfig = shapesConfigMockService.GetShapeConfigById(shapeConfig.ShapeConfigID);
            Assert.Null(shapeConfig);
        }

        [Fact]
        public void AddConfigRepository()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ShapeConfig shapeConfigToAdd = new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "Circular Shape",
                EntityClass = "EntityMaster",
                Shape = "Circle"
            };
            Guid identifierAdded = shapesConfigMockService.AddShapeConfig(shapeConfigToAdd);
            ShapeConfig shapeConfig = shapesConfigMockService.GetShapeConfigById(identifierAdded);
            Assert.True(shapeConfigToAdd.Name.Equals(shapeConfig.Name));
        }
    }
}
