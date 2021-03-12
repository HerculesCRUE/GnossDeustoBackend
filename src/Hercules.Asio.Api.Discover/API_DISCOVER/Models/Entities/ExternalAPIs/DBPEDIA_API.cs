// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using API_DISCOVER.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de DBPEDIA
    /// </summary>
    public class DBPEDIA_API : I_ExternalAPI
    {
        public string Name { get { return "DBPEDIA"; } }
        public string Description { get { return "DBPEDIA es un proyecto para la extracción de datos de Wikipedia para proponer una versión Web semántica."; } }

        public string HomePage { get { return "https://dbpedia.org/"; } }

        public string Id { get { return "dbpedia"; } }

        /// <summary>
        /// Busca personas en el API de DBPEDIA
        /// </summary>
        /// <param name="q">Texto a buscar (con urlEncode)</param>
        /// <returns>Objeto con las personas encontradas</returns>
        public static DBPEDIAData Search(string q)
        {
            string SPARQLEndpoint = "https://dbpedia.org/sparql";
            string Graph = "http://dbpedia.org";
            string consulta = "select * where {?s <http://www.w3.org/2000/01/rdf-schema#label> '" + q + "'@es. ?s a ?rdftype.FILTER(?rdftype in (<http://dbpedia.org/ontology/Organisation>,<http://dbpedia.org/ontology/Place>)) MINUS{?s <http://dbpedia.org/ontology/wikiPageDisambiguates> ?dis} OPTIONAL {?s <http://www.w3.org/2002/07/owl#sameAs> ?geonames.FILTER(?geonames like'http://sws.geonames.org*')}}";
            string QueryParam = "query";
            SparqlUtility utility = new SparqlUtility();
            SparqlObject sparqlObject = utility.SelectData(SPARQLEndpoint, Graph, consulta, QueryParam, "", "");
            DBPEDIAData dBPEDIAData = new DBPEDIAData();

            foreach (var result in sparqlObject.results.bindings)
            {
                if (result.ContainsKey("s") && result["s"] != null)
                {
                    dBPEDIAData.uri_dbpedia = result["s"].value;
                }
                if (result.ContainsKey("geonames") && result["geonames"] != null)
                {
                    dBPEDIAData.uri_geonames = result["geonames"].value;
                }
            }
            return dBPEDIAData;
        }
    }
    public class DBPEDIAData
    {
        public DBPEDIAData()
        {

        }
        public string uri_dbpedia { get; set; }
        public string uri_geonames { get; set; }
    }
}
