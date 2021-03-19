// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using API_DISCOVER.Models.Entities.ExternalAPIs;
using System;
using System.Collections.Generic;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Objeto para cachear las peticiones a diversos APIs de Discover
    /// </summary>
    [Serializable]
    public class DiscoverCacheGlobal
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DiscoverCacheGlobal()
        {
            NormalizedNames = new Dictionary<string, string>();
            NormalizedTitles = new Dictionary<string, string>();
            NGrams = new Dictionary<string, HashSet<string>>();
        }
        /// <summary>
        /// Caché para los nombres normalizados
        /// </summary>
        public Dictionary<string, string> NormalizedNames { get; set; }

        /// <summary>
        /// Caché para los titulos normalizados
        /// </summary>
        public Dictionary<string, string> NormalizedTitles { get; set; }

        /// <summary>
        /// Caché para los n-gramas
        /// </summary>
        public Dictionary<string, HashSet<string>> NGrams { get; set; }
    }
}