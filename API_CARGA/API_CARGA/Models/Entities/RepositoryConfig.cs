// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Datos de configuración de un repositorio OAI-PMH
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Datos de configuración de un repositorio OAI-PMH
    /// </summary>
    public class RepositoryConfig
    {
        public RepositoryConfig()
        {
            ///ShapeConfig = new HashSet<ShapeConfig>();
        }

        /// <summary>
        /// Identificador del repositorio
        /// </summary>
        [Key]
        public Guid RepositoryConfigID { get; set; }
        /// <summary>
        /// Nombre del repositorio
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Token OAUTH
        /// </summary>
        public string OauthToken { get; set; }
        /// <summary>
        /// url del repositorio
        /// </summary>
        public string Url { get; set; }
        [ForeignKey("RepositoryID")]
        public virtual ICollection<ShapeConfig> ShapeConfig { get; set; }


    }
}
