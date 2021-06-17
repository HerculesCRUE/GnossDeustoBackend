// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace VDS.RDF
{
    /// <summary>
    /// RohGraph
    /// </summary>
    [Serializable]
    public class RohGraph : Graph
    {

        /// <summary>
        /// Creates a new instance of a Graph.
        /// </summary>
        public RohGraph()
            : base() { }

        /// <summary>
        /// Creates a new instance of a Graph with an optionally empty Namespace Map.
        /// </summary>
        /// <param name="emptyNamespaceMap">Whether the Namespace Map should be empty.</param>
        public RohGraph(bool emptyNamespaceMap)
            : base(emptyNamespaceMap){}

        /// <summary>
        /// Creates a new instance of a Graph using the given Triple Collection.
        /// </summary>
        /// <param name="tripleCollection">Triple Collection.</param>
        public RohGraph(BaseTripleCollection tripleCollection)
            : base(tripleCollection) { }

        /// <summary>
        /// Creates a new instance of a Graph using the given Triple Collection and an optionally empty Namespace Map.
        /// </summary>
        /// <param name="tripleCollection">Triple Collection.</param>
        /// <param name="emptyNamespaceMap">Whether the Namespace Map should be empty.</param>
        public RohGraph(BaseTripleCollection tripleCollection, bool emptyNamespaceMap)
            : base(tripleCollection, emptyNamespaceMap) { }

        /// <summary>
        /// Creates a new unused Blank Node ID and returns it.
        /// </summary>
        /// <returns></returns>
        public override String GetNextBlankNodeID()
        {
            String id = Guid.NewGuid().ToString();
            id="N" + id.Replace("-", "");
            return id;
        }
        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public RohGraph Clone()
        {
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            string rdf = VDS.RDF.Writing.StringWriter.Write(this, rdfxmlwriter);
            RohGraph rohGraphClone = new RohGraph();
            rohGraphClone.LoadFromString(rdf, new RdfXmlParser());
            return rohGraphClone;
        }
    }
}
