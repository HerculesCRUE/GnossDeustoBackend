using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para gestionar los shapes en base de datos 
    ///</summary>
    public class ShapesConfigBDService : IShapesConfigService
    {
        private readonly EntityContext _context;
        public ShapesConfigBDService(EntityContext context)
        {
            _context = context;
        }

        ///<summary>
        ///Añade un shape
        ///</summary>
        ///<param name="shapeConfig">Shape a añadir</param>
        public Guid AddShapeConfig(ShapeConfig shapeConfig)
        {
            Guid addedID = Guid.Empty;
            addedID = Guid.NewGuid();
            shapeConfig.ShapeConfigID = addedID;
            _context.ShapeConfig.Add(shapeConfig);
            _context.SaveChanges();
            return addedID;
        }

        ///<summary>
        ///Devuelve un shape
        ///</summary>
        ///<param name="id">Identificador del shape a devolver</param>
        public ShapeConfig GetShapeConfigById(Guid id)
        {
            return _context.ShapeConfig.FirstOrDefault(shape => shape.ShapeConfigID.Equals(id));
        }

        ///<summary>
        ///Devuelve una lista shapes
        ///</summary>
        public List<ShapeConfig> GetShapesConfigs()
        {
            return _context.ShapeConfig.OrderBy(shape => shape.Name).ToList();
        }

        ///<summary>
        ///Modifica un shape existente
        ///</summary>
        ///<param name="shapeConfig">Shape a modificar con los nuevos valores</param>
        public bool ModifyShapeConfig(ShapeConfig shapeConfig)
        {
            bool modified = false;
            ShapeConfig shapeConfigOriginal = GetShapeConfigById(shapeConfig.ShapeConfigID);
            if (shapeConfigOriginal != null)
            {
                shapeConfigOriginal.Name = shapeConfig.Name;
                if (shapeConfig.Shape != null)
                {
                    shapeConfigOriginal.Shape = shapeConfig.Shape;
                }
                shapeConfigOriginal.RepositoryID = shapeConfig.RepositoryID;
                _context.SaveChanges();
                modified = true;
            }
            return modified;
        }

        ///<summary>
        ///Elimina un shape existente
        ///</summary>
        ///<param name="identifier">Identificador del shape a eliminar</param>
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
