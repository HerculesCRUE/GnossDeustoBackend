using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models
{
    /// <summary>
    /// Datos de configuración de una validación SHACL
    /// </summary>
    public class ShapeConfig
    {
        /// <summary>
        /// Identificador de la validación
        /// </summary>
        public string identifier { get; set; }
        /// <summary>
        /// Nombre de la validación
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Nombre de la clase que se validará
        /// </summary>
        public string entityClass { get; set; }
        /// <summary>
        /// Definición del shape SHACL
        /// </summary>
        public string shape { get; set; }
    }
}
