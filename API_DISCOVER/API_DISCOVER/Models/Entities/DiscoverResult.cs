// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using API_DISCOVER.Utility;
using System;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Writing;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Resultado despues de aplicar a un RDF el descubrimiento
    /// </summary>
    public class DiscoverResult
    {
        /// <summary>
        /// Constructor del resultado del descubirmiento
        /// </summary>
        /// <param name="pDataGraph">Grafo con los datos</param>
        /// <param name="pDataInferenceGraph">Grafo con los datos (con inferencia)</param>
        /// <param name="pOntologyGraph">Grafo con la ontología</param>
        /// <param name="pDiscoveredEntitiesWithSubject">Entidades descubiertas con los sujetos</param>
        /// <param name="pDiscoveredEntitiesWithId">Entidades descubiertas con los identificadores</param>
        /// <param name="pDiscoveredEntitiesWithDataBase">Entidades descubiertas con la BBDD</param>
        /// <param name="pDiscoveredEntitiesWithExternalIntegration">Entidades descubiertas con al integración externa</param>
        /// <param name="pDiscoveredEntitiesProbability">Probabilidades de descubriiento</param>
        /// <param name="pDateStart">Fecha inicio descubirmiento</param>
        /// <param name="pDateEnd">Fecha fin descubirmiento</param>
        /// <param name="pExternalIntegration">Datos obtendidos con las integraciones con fuentes externas junto con su provenecia</param>
        public DiscoverResult(RohGraph pDataGraph,RohGraph pDataInferenceGraph, RohGraph pOntologyGraph, HashSet<string> pDiscoveredEntitiesWithSubject, Dictionary<string, string> pDiscoveredEntitiesWithId, Dictionary<string, KeyValuePair<string, float>> pDiscoveredEntitiesWithDataBase, Dictionary<string, KeyValuePair<string, float>> pDiscoveredEntitiesWithExternalIntegration, Dictionary<string, Dictionary<string, float>> pDiscoveredEntitiesProbability,DateTime pDateStart,DateTime pDateEnd, Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> pExternalIntegration)
        {
            dataGraph = pDataGraph;
            dataInferenceGraph = pDataInferenceGraph;
            ontologyGraph = pOntologyGraph;
            discoveredEntitiesWithSubject = pDiscoveredEntitiesWithSubject;
            discoveredEntitiesWithId = pDiscoveredEntitiesWithId;
            discoveredEntitiesWithDataBase = pDiscoveredEntitiesWithDataBase;
            discoveredEntitiesWithExternalIntegration = pDiscoveredEntitiesWithExternalIntegration;
            discoveredEntitiesProbability = pDiscoveredEntitiesProbability;
            secondsProcessed = (pDateEnd-pDateStart).TotalSeconds;
            start = pDateStart;
            end = pDateEnd;
            externalIntegration = pExternalIntegration;
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
        /// Entidades descubiertas con los sujetos
        /// </summary>
        public HashSet<string> discoveredEntitiesWithSubject { get; }

        /// <summary>
        /// Entidades descubiertas con los identificadores
        /// </summary>
        public Dictionary<string, string> discoveredEntitiesWithId { get; }

        /// <summary>
        /// Entidades descubiertas con la BBDD
        /// </summary>
        public Dictionary<string, KeyValuePair<string, float>> discoveredEntitiesWithDataBase { get; }

        /// <summary>
        /// Entidades descubiertas con la Integración externa
        /// </summary>
        public Dictionary<string, KeyValuePair<string, float>> discoveredEntitiesWithExternalIntegration { get; }

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
        public Dictionary<string, Dictionary<string, KeyValuePair<string, HashSet<string>>>> externalIntegration { get; }
        
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
