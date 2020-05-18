using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOperationsValidation
    {
        [Fact]
        public void GetShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            List<ShapeConfig> listaRepositorios = shapesConfigMockService.GetShapesConfigs();
            Assert.True(listaRepositorios.Count > 0);
        }

        [Fact]
        public void GetShapeByID()
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
        public void AddConfigShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ShapeConfig shapeConfigToAdd = new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "Circular Shape",
                RepositoryID = Guid.NewGuid(),
                Shape = "Circle"
            };
            Guid identifierAdded = shapesConfigMockService.AddShapeConfig(shapeConfigToAdd);
            ShapeConfig shapeConfig = shapesConfigMockService.GetShapeConfigById(identifierAdded);
            Assert.True(shapeConfigToAdd.Name.Equals(shapeConfig.Name));
        }

        [Fact]
        public void DeleteConfigShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ShapeConfig shapeConfig = shapesConfigMockService.GetShapesConfigs()[0];
            shapesConfigMockService.RemoveShapeConfig(shapeConfig.ShapeConfigID);
            shapeConfig = shapesConfigMockService.GetShapeConfigById(shapeConfig.ShapeConfigID);
            Assert.Null(shapeConfig);
        }

        [Fact]
        public void ModifyConfigShape()
        {
            ShapesConfigMockService shapesConfigMockService = new ShapesConfigMockService();
            ShapeConfig shapeConfig = shapesConfigMockService.GetShapesConfigs()[0];
            Random random = new Random();
            string newName = "updatedShape_" + random.NextDouble();
            shapeConfig.Name = newName;
            shapesConfigMockService.ModifyShapeConfig(shapeConfig);
            ShapeConfig updatedshapeConfig = shapesConfigMockService.GetShapeConfigById(shapeConfig.ShapeConfigID);
            Assert.True(updatedshapeConfig.Name.Equals(newName));
        }

    }
}
