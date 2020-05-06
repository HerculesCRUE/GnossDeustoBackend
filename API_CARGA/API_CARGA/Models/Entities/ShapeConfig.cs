using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("RepositoryConfig")]
        [Required]
        public Guid RepositoryID { get; set; }
        /// <summary>
        /// Definición del shape SHACL
        /// </summary>
        public string Shape { get; set; }

        public RepositoryConfig RepositoryConfig { get; set; }
    }
}
