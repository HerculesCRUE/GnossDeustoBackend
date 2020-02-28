using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;

namespace API_CARGA.Models.Services
{
    public interface IShapesConfigService
    {
        public List<ShapeConfig> GetShapesConfigs();
        public ShapeConfig GetShapeConfigByName(string name);
        public ShapeConfig GetShapeConfigById(Guid id);
        public bool RemoveShapeConfig(Guid identifier);
        public Guid AddShapeConfig(ShapeConfig shapeConfig);
        public bool ModifyShapeConfig(ShapeConfig shapeConfig);
    }
}
