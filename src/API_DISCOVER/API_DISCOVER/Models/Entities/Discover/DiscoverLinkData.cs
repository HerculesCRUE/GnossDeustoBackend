using System;
using System.Collections.Generic;
using System.Text;

namespace API_DISCOVER.Models.Entities.Discover
{
    /// <summary>
    /// Datos para trabajar con el descubrimiento de enlaces
    /// </summary>
    public class DiscoverLinkData
    {
        /// <summary>
        /// Datos de una propiedad
        /// </summary>
        public class PropertyData
        {
            /// <summary>
            /// Propiedad
            /// </summary>
            public string property { get; set; }

            /// <summary>
            /// Valores de la propiedad junto con su/sus provenance
            /// </summary>
            public Dictionary<string, HashSet<string>> valueProvenance{ get; set; }
        }

        /// <summary>
        /// Listado de entidades con las propiedades obtenidas por el descubrimiento
        /// </summary>
        public Dictionary<string, List<PropertyData>> entitiesProperties { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DiscoverLinkData()
        {
            entitiesProperties = new Dictionary<string, List<PropertyData>>();
        }
    }
}
