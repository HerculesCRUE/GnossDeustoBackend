using System;
using System.Collections.Generic;
using System.Text;
using VDS.RDF;

namespace API_DISCOVER.Models.Entities.Discover
{
    /// <summary>
    /// Datos para trabajar con el descubrimiento de enlaces
    /// </summary>
    public class ExternalIntegrationData
    {
        /// <summary>
        /// Grafo con los datos obtenidos de la fuente externa
        /// </summary>
        public RohGraph externalGraph { get; set; }

        /// <summary>
        /// Grafo con lo datos del provenance
        /// </summary>
        public RohGraph provenanceGraph { get; set; }


    }
}
