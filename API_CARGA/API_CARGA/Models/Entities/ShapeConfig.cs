using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Datos de configuración de una validación SHACL
    /// </summary>
    public class ShapeConfig
    {
        /// <summary>
        /// Identificador de la validación
        /// </summary>
        public Guid ShapeConfigID { get; set; }
        /// <summary>
        /// Nombre de la validación
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nombre de la clase que se validará
        /// </summary>
        public string EntityClass { get; set; }
        /// <summary>
        /// Definición del shape SHACL
        /// </summary>
        public string Shape { get; set; }
    }
}
