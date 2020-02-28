using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Guid AddShapeConfig(ShapeConfig shapeConfig)
        {
            Guid addedID = Guid.Empty;
            ShapeConfig shapeConfigOriginal = GetShapeConfigByName(shapeConfig.Name);
            if(shapeConfigOriginal == null)
            {
                addedID = Guid.NewGuid();
                shapeConfig.ShapeConfigID = addedID;
                _listShapesConfig.Add(shapeConfig);

            }
            return addedID;
        }

        public ShapeConfig GetShapeConfigById(Guid id)
        {
            return _listShapesConfig.FirstOrDefault(shape => shape.ShapeConfigID.Equals(id));
        }

        public ShapeConfig GetShapeConfigByName(string name)
        {
            return _listShapesConfig.FirstOrDefault(shape => shape.Name.Equals(name));
        }

        public List<ShapeConfig> GetShapesConfigs()
        {
            return _listShapesConfig;
        }

        public bool ModifyShapeConfig(ShapeConfig shapeConfig)
        {
            bool modified = false;
            ShapeConfig shapeConfigOriginal = GetShapeConfigById(shapeConfig.ShapeConfigID);
            if(shapeConfigOriginal != null)
            {
                shapeConfigOriginal.Name = shapeConfig.Name;
                shapeConfigOriginal.Shape = shapeConfig.Shape;
                shapeConfigOriginal.EntityClass = shapeConfig.EntityClass;
                modified = true;
            }
            return modified;
        }

        public bool RemoveShapeConfig(Guid identifier)
        {
            try
            {
                ShapeConfig shapeConfig = GetShapeConfigById(identifier);
                if(shapeConfig != null)
                {
                    _listShapesConfig.Remove(shapeConfig);
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            } 
        }
    }
}
