using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Interfaz para gestionar las operaciones de los repositorios 
    ///</summary>
    public interface IShapesConfigService
    {
        ///<summary>
        ///Devuelve una lista shapes
        ///</summary>
        public List<ShapeConfig> GetShapesConfigs();
        ///<summary>
        ///Devuelve un shape
        ///</summary>
        ///<param name="id">Identificador del shape a devolver</param>
        public ShapeConfig GetShapeConfigById(Guid id);
        ///<summary>
        ///Elimina un shape existente
        ///</summary>
        ///<param name="identifier">Identificador del shape a eliminar</param>
        public bool RemoveShapeConfig(Guid identifier);
        ///<summary>
        ///Añade un shape
        ///</summary>
        ///<param name="shapeConfig">Shape a añadir</param>
        public Guid AddShapeConfig(ShapeConfig shapeConfig);
        ///<summary>
        ///Modifica un shape existente
        ///</summary>
        ///<param name="shapeConfig">Shape a modificar con los nuevos valores</param>
        public bool ModifyShapeConfig(ShapeConfig shapeConfig);
    }
}
