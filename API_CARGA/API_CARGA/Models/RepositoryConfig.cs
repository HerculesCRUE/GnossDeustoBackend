using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models
{
    /// <summary>
    /// Datos de configuración de un repositorio OAI-PMH
    /// </summary>
    public class RepositoryConfig
    {
        /// <summary>
        /// Identificador del repositorio
        /// </summary>
        public string identifier { get; set; }
        /// <summary>
        /// Nombre del repositorio
        /// </summary>
        public string name { get; set; }        
        /// <summary>
        /// Token OAUTH
        /// </summary>
        public string oauthToken { get; set; }
    }
}
