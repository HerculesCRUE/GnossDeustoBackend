// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Datos de configuración de un repositorio OAI-PMH
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Datos de configuración de un repositorio OAI-PMH
    /// </summary>
    /// 
    [ExcludeFromCodeCoverage]
    public class RepositoryConfig
    {
        public RepositoryConfig()
        {
           
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
        /// Indica si se apica el descubrimiento al repositorio
        /// </summary>
        public bool ApplyDiscover { get; set; }
        /// <summary>
        /// url del repositorio
        /// </summary>
        public string Url { get; set; }
    }
}
