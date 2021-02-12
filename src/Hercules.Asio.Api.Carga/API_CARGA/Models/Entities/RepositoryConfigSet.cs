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
    /// Datos de la última sincronización de un set
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RepositoryConfigSet
    {
        /// <summary>
        /// Identificador del set de la sincronización
        /// </summary>
        [Key]
        public Guid RepositoryConfigSetID { get; set; }
        /// <summary>
        /// Nombre del set de la sincronización
        /// </summary>
        [Required]
        public string Set { get; set; }
        /// <summary>
        /// Fecha del último elemento del set sincronizado
        /// </summary>
        [Required]
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Identificador del repositorio
        /// </summary>
        [Required]
        public Guid RepositoryID { get; set; }
    }
}
