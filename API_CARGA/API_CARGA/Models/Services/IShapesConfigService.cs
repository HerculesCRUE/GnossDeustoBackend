using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;

namespace API_CARGA.Models.Services
{
    public interface IShapesConfigService
    {
        public List<ShapeConfig> GetRepositoryConfigs();
        public ShapeConfig GetRepositoryConfigByName(string name);
        public ShapeConfig GetRepositoryConfigById(Guid id);
        public bool RemoveRepositoryConfig(Guid identifier);
        public Guid AddRepositoryConfig(ShapeConfig shapeConfig);
        public bool ModifyRepositoryConfig(ShapeConfig shapeConfig);
    }
}
