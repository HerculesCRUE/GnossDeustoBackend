using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class ShapesConfigMockService : IShapesConfigService
    {
        private List<ShapeConfig> _listShapesConfig;

        public ShapesConfigMockService()
        {
            _listShapesConfig = new List<ShapeConfig>();

            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_1",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_2",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_3",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_4",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
            _listShapesConfig.Add(new ShapeConfig()
            {
                ShapeConfigID = Guid.NewGuid(),
                Name = "ShapeConfig_5",
                EntityClass = "ShapeClass",
                Shape = "Definition_1"
            });
        }
        public Guid AddRepositoryConfig(ShapeConfig shapeConfig)
        {
            Guid addedID = Guid.Empty;
            ShapeConfig shapeConfigOriginal = GetRepositoryConfigByName(shapeConfig.Name);
            if(shapeConfigOriginal == null)
            {
                addedID = Guid.NewGuid();
                shapeConfig.ShapeConfigID = addedID;
                _listShapesConfig.Add(shapeConfig);

            }
            return addedID;
        }

        public ShapeConfig GetRepositoryConfigById(Guid id)
        {
            return _listShapesConfig.FirstOrDefault(shape => shape.ShapeConfigID.Equals(id));
        }

        public ShapeConfig GetRepositoryConfigByName(string name)
        {
            return _listShapesConfig.FirstOrDefault(shape => shape.Name.Equals(name));
        }

        public List<ShapeConfig> GetRepositoryConfigs()
        {
            return _listShapesConfig;
        }

        public bool ModifyRepositoryConfig(ShapeConfig shapeConfig)
        {
            bool modified = false;
            ShapeConfig shapeConfigOriginal = GetRepositoryConfigById(shapeConfig.ShapeConfigID);
            if(shapeConfigOriginal != null)
            {
                shapeConfigOriginal.Name = shapeConfig.Name;
                shapeConfigOriginal.Shape = shapeConfig.Shape;
                shapeConfigOriginal.EntityClass = shapeConfig.EntityClass;
                modified = true;
            }
            return modified;
        }

        public bool RemoveRepositoryConfig(Guid identifier)
        {
            try
            {
                ShapeConfig shapeConfig = GetRepositoryConfigById(identifier);
                if(shapeConfig != null)
                {
                    _listShapesConfig.Remove(shapeConfig);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            } 
        }
    }
}
