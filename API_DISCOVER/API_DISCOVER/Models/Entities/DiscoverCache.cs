// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Objeto para cachear las peticiones a diversos APIs de Discover
    /// </summary>
    public class DiscoverCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DiscoverCache()
        {
            Sparql = new Dictionary<int, SparqlObject>();
            ORCIDExpandedSearch = new Dictionary<string, ORCIDExpandedSearch>();
            ORCIDPerson = new Dictionary<string, ORCIDPerson>();
            ORCIDWorks = new Dictionary<string, ORCIDWorks>();
            SCOPUSWorks = new Dictionary<string, SCOPUSWorks>();
            SCOPUSPerson = new Dictionary<ulong, SCOPUSPerson>();
            DBLPAuthors = new Dictionary<string, DBLPAuthors>();
            DBLPPerson = new Dictionary<string, DBLPPerson>();
            NormalizedNames = new Dictionary<string, string>();
            Similarity = new Dictionary<string, float>();
        }

        /// <summary>
        /// Caché para las consultas SPARQL
        /// </summary>
        public Dictionary<int, SparqlObject> Sparql { get; set; }

        /// <summary>
        /// Caché para peticiones a ORCID
        /// </summary>
        public Dictionary<string, ORCIDExpandedSearch> ORCIDExpandedSearch { get; set; }

        /// <summary>
        /// Caché para peticiones a ORCID
        /// </summary>
        public Dictionary<string, ORCIDPerson> ORCIDPerson { get; set; }

        /// <summary>
        /// Caché para peticiones a ORCID
        /// </summary>
        public Dictionary<string, ORCIDWorks> ORCIDWorks { get; set; }

        /// <summary>
        /// Caché para peticiones a SCOPUS
        /// </summary>
        public Dictionary<string, SCOPUSWorks> SCOPUSWorks { get; set; }

        /// <summary>
        /// Caché para peticiones a SCOPUS
        /// </summary>
        public Dictionary<ulong, SCOPUSPerson> SCOPUSPerson { get; set; }

        /// <summary>
        /// Caché para peticiones a DBLP
        /// </summary>
        public Dictionary<string, DBLPAuthors> DBLPAuthors { get; set; }

        /// <summary>
        /// Caché para peticiones a DBLP
        /// </summary>
        public Dictionary<string, DBLPPerson> DBLPPerson { get; set; }

        public Dictionary<string, string> NormalizedNames { get; set; }

        public Dictionary<string, float> Similarity { get; set; }
    }
}