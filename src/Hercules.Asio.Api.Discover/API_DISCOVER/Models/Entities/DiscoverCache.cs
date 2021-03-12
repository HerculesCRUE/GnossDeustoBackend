﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using API_DISCOVER.Models.Entities.ExternalAPIs;
using System.Collections.Generic;

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
            CROSSREF_Works = new Dictionary<string, CROSSREF_Works>();
            PUBMED_WorkSearchByTitle = new Dictionary<string, uint[]>();
            PUBMED_WorkByID = new Dictionary<uint, PubmedArticleSet>();
            WOSWorks = new Dictionary<string, WOSWorks>();
            RECOLECTAWorks = new Dictionary<string, List<RecolectaDocument>>();
            DOAJWorks = new Dictionary<string, DOAJWorks>();
            DBPEDIAData = new Dictionary<string, DBPEDIAData>();
            NormalizedNames = new Dictionary<string, string>();
            NGrams = new Dictionary<string, HashSet<string>>();
            CoefJackard = new Dictionary<string, float>();
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

        /// <summary>
        /// Caché para peticiones a CROSSREF
        /// </summary>
        public Dictionary<string, CROSSREF_Works> CROSSREF_Works { get; set; }

        /// <summary>
        /// Caché para peticiones a PUBMED
        /// </summary>
        public Dictionary<string, uint[]> PUBMED_WorkSearchByTitle { get; set; }

        /// <summary>
        /// Caché para peticiones a PUBMED
        /// </summary>
        public Dictionary<uint, PubmedArticleSet> PUBMED_WorkByID { get; set; }

        /// <summary>
        /// Caché para peticiones a WOS
        /// </summary>
        public Dictionary<string, WOSWorks> WOSWorks { get; set; }

        /// <summary>
        /// Caché para peticiones a RECOLECTA
        /// </summary>
        public Dictionary<string, List<RecolectaDocument>> RECOLECTAWorks { get; set; }

        /// <summary>
        /// Caché para peticiones a DOAJ
        /// </summary>
        public Dictionary<string, DOAJWorks> DOAJWorks { get; set; }

        /// <summary>
        /// Caché para peticiones a DBPEDIA
        /// </summary>
        public Dictionary<string, DBPEDIAData> DBPEDIAData { get; set; }

        /// <summary>
        /// Caché para los nombres normalizados
        /// </summary>
        public Dictionary<string, string> NormalizedNames { get; set; }

        /// <summary>
        /// Caché para los n-gramas
        /// </summary>
        public Dictionary<string, HashSet<string>> NGrams { get; set; }

        /// <summary>
        /// Caché para los coeficientes Jackard
        /// </summary>
        public Dictionary<string, float> CoefJackard { get; set; }
    }
}