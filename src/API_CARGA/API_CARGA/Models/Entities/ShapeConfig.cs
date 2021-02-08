// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Datos de configuración de una validación SHACL
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Datos de configuración de una validación SHACL
    /// </summary>
    /// 
    [ExcludeFromCodeCoverage]
    public class ShapeConfig
    {
        /// <summary>
        /// Identificador de la validación
        /// </summary>
        [Key]
        public Guid ShapeConfigID { get; set; }
        /// <summary>
        /// Nombre de la validación
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Identificador de la validación
        /// </summary>
        //[ForeignKey("RepositoryConfig")]
        [Required]
        public Guid RepositoryID { get; set; }
        /// <summary>
        /// Definición del shape SHACL
        /// </summary>
        public string Shape { get; set; }

        //public virtual RepositoryConfig RepositoryConfig { get; set; }
    }
}
