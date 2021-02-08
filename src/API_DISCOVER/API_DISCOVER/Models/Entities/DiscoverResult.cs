// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using API_DISCOVER.Models.Entities.Discover;
using API_DISCOVER.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VDS.RDF;
using VDS.RDF.Writing;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Resultado despues de aplicar a un RDF el descubrimiento
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DiscoverResult
    {
        /// <summary>
        /// Constructor del resultado del descubirmiento
        /// </summary>
        /// <param name="pDataGraph">Grafo con los datos</param>
        /// <param name="pDataInferenceGraph">Grafo con los datos (con inferencia)</param>
        /// <param name="pOntologyGraph">Grafo con la ontología</param>
        /// <param name="pReconciliationData">Datos obtenidos de la reconciliación</param>
        /// <param name="pDiscoveredEntitiesProbability">Probabilidades de descubriiento</param>
        /// <param name="pDateStart">Fecha inicio descubirmiento</param>
        /// <param name="pDateEnd">Fecha fin descubirmiento</param>
        /// <param name="pDiscoverLinkData">Datos para trabajar con el descubrimiento de enlaces</param>
        public DiscoverResult(RohGraph pDataGraph,RohGraph pDataInferenceGraph, RohGraph pOntologyGraph, ReconciliationData pReconciliationData, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability,DateTime pDateStart,DateTime pDateEnd, DiscoverLinkData pDiscoverLinkData)
        {
            dataGraph = pDataGraph;
            dataInferenceGraph = pDataInferenceGraph;
            ontologyGraph = pOntologyGraph;
            reconciliationData = pReconciliationData;
            discoveredEntitiesProbability = pDiscoveredEntitiesProbability;
            secondsProcessed = (pDateEnd-pDateStart).TotalSeconds;
            start = pDateStart;
            end = pDateEnd;
            discoverLinkData = pDiscoverLinkData;
        }

        /// <summary>
        /// Grafo con los datos
        /// </summary>
        public RohGraph dataGraph { get; set; }

        /// <summary>
        /// Grafo con los datos (con inferencia)
        /// </summary>
        public RohGraph dataInferenceGraph { get; set; }

        /// <summary>
        /// Grafo con la ontología
        /// </summary>
        public RohGraph ontologyGraph { get; set; }

        /// <summary>
        /// Datos obtenidos de la reconciliacion
        /// </summary>
        public ReconciliationData reconciliationData { get; set; }

        /// <summary>
        /// Probabilidades de descubriiento
        /// </summary>
        public Dictionary<string, Dictionary<string, float>> discoveredEntitiesProbability { get; }

        /// <summary>
        /// Inicio del descubrimiento
        /// </summary>
        public DateTime start { get; }


        /// <summary>
        /// Fin del descubrimiento
        /// </summary>
        public DateTime end { get; }


        /// <summary>
        /// Tiempo (en segundos) en procesar el descubrimiento
        /// </summary>
        public double secondsProcessed { get; }

        /// <summary>
        /// Datos obtendidos con la integración con fuentes externas junto con su provenencia
        /// </summary>
        public DiscoverLinkData discoverLinkData { get; }
        
        /// <summary>
        /// Obtiene el RDF del dataGraph
        /// </summary>
        /// <returns></returns>
        public string GetDataGraphRDF()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();
            rdfXmlWriter.Save(dataGraph, sw);
            return sw.ToString();
        }

    }
}
