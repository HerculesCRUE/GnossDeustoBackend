using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class ShapesConfigBDService : IShapesConfigService
    {
        private readonly EntityContext _context;
        public ShapesConfigBDService(EntityContext context)
        {
            _context = context;
        }

        public Guid AddShapeConfig(ShapeConfig shapeConfig)
        {
            Guid addedID = Guid.Empty;
            addedID = Guid.NewGuid();
            shapeConfig.ShapeConfigID = addedID;
            _context.ShapeConfig.Add(shapeConfig);
            _context.SaveChanges();
            return addedID;
        }

        public ShapeConfig GetShapeConfigById(Guid id)
        {
            return _context.ShapeConfig.FirstOrDefault(shape => shape.ShapeConfigID.Equals(id));
        }

        public List<ShapeConfig> GetShapesConfigs()
        {
            return _context.ShapeConfig.OrderBy(shape => shape.Name).ToList();
        }

        public bool ModifyShapeConfig(ShapeConfig shapeConfig)
        {
            bool modified = false;
            ShapeConfig shapeConfigOriginal = GetShapeConfigById(shapeConfig.ShapeConfigID);
            if (shapeConfigOriginal != null)
            {
                shapeConfigOriginal.Name = shapeConfig.Name;
                shapeConfigOriginal.Shape = shapeConfig.Shape;
                shapeConfigOriginal.RepositoryID = shapeConfig.RepositoryID;
                _context.SaveChanges();
                modified = true;
            }
            return modified;
        }

        public bool RemoveShapeConfig(Guid identifier)
        {
            try
            {
                ShapeConfig shapeConfig = GetShapeConfigById(identifier);
                if (shapeConfig != null)
                {
                    _context.Entry(shapeConfig).State = EntityState.Deleted;
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
