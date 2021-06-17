// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de ORCID
    /// </summary>
    public class ORCID_API : I_ExternalAPI
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return "ORCID"; } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get { return "ORCID is a nonprofit organization helping create a world in which all who participate in research, scholarship and innovation are uniquely identified and connected to their contributions and affiliations, across disciplines, borders, and time."; } }
        /// <summary>
        /// HomePage
        /// </summary>
        public string HomePage { get { return "https://orcid.org/"; } }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get { return "orcid"; } }

        /// <summary>
        /// Busca personas en el API de ORCID
        /// </summary>
        /// <param name="q">Texto a buscar (con urlEncode)</param>
        /// <returns>Objeto con las personas encontradas</returns>
        public static ORCIDExpandedSearch ExpandedSearch(string q)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
            string jsonRespuesta = webClient.DownloadString("https://pub.orcid.org/v3.0/expanded-search?q=" + q + "&rows=5");
            webClient.Dispose();
            return JsonConvert.DeserializeObject<ORCIDExpandedSearch>(jsonRespuesta);
        }
        
        /// <summary>
        /// Obtiene los datos de una persona en el API de ORCID
        /// </summary>
        /// <param name="id">Identificador de ORCID</param>
        /// <returns>Objeto con los datos de la persona</returns>
        public static ORCIDPerson Person(string id)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
            string jsonRespuestaOrcidPerson = webClient.DownloadString("https://pub.orcid.org/v3.0/" + id + "/person");
            webClient.Dispose();
            return JsonConvert.DeserializeObject<ORCIDPerson>(jsonRespuestaOrcidPerson);
        }

        /// <summary>
        /// Obtiene los trabajos de una persona en el API de ORCID
        /// </summary>
        /// <param name="id">Identificador de ORCID</param>
        /// <returns>Objeto con los datos de las publicaciones</returns>
        public static ORCIDWorks Works(string id)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
            string jsonRespuestaOrcidWorks = webClient.DownloadString("https://pub.orcid.org/v3.0/" +id + "/works");
            webClient.Dispose();
            return JsonConvert.DeserializeObject<ORCIDWorks>(jsonRespuestaOrcidWorks);
        }
    }

    /// <summary>
    /// ORCIDExpandedSearch
    /// </summary>
    [DataContract]
    public class ORCIDExpandedSearch
    {
        /// <summary>
        /// ORCIDExpandedSearch
        /// </summary>
        public ORCIDExpandedSearch() { 

        }

        /// <summary>
        /// expanded_result
        /// </summary>
        [DataMember(Name = "expanded-result")]
        public List<Result> expanded_result { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        [DataContract]
        public class Result
        {
            /// <summary>
            /// Result
            /// </summary>
            public Result() { 
            }
            /// <summary>
            /// orcid_id
            /// </summary>
            [DataMember(Name = "orcid-id")]
            public string orcid_id { get; set; }
            /// <summary>
            /// given_names
            /// </summary>
            [DataMember(Name = "given-names")]
            public string given_names { get; set; }
            /// <summary>
            /// family_names
            /// </summary>
            [DataMember(Name = "family-names")]
            public string family_names { get; set; }
        }
    }

    /// <summary>
    /// ORCIDPerson
    /// </summary>
    [DataContract]
    public class ORCIDPerson
    {
        /// <summary>
        /// ORCIDPerson
        /// </summary>
        public ORCIDPerson()
        {

        }

        /// <summary>
        /// external_identifiers
        /// </summary>
        [DataMember(Name = "external-identifiers")]
        public ExternalIdentifiers external_identifiers { get; set; }

        /// <summary>
        /// ExternalIdentifiers
        /// </summary>
        [DataContract]
        public class ExternalIdentifiers
        {
            /// <summary>
            /// ExternalIdentifiers
            /// </summary>
            public ExternalIdentifiers()
            {
            }
            /// <summary>
            /// external_identifier
            /// </summary>
            [DataMember(Name = "external-identifier")]
            public List<ExternalIdentifier> external_identifier { get; set; }
            /// <summary>
            /// ExternalIdentifier
            /// </summary>
            [DataContract]
            public class ExternalIdentifier
            {
                /// <summary>
                /// ExternalIdentifier
                /// </summary>
                public ExternalIdentifier()
                {
                }
                /// <summary>
                /// external_id_type
                /// </summary>
                [DataMember(Name = "external-id-type")]
                public string external_id_type { get; set; }
                /// <summary>
                /// external_id_value
                /// </summary>
                [DataMember(Name = "external-id-value")]
                public string external_id_value { get; set; }
            }
        }
    }

    /// <summary>
    /// ORCIDWorks
    /// </summary>
    [DataContract]
    public class ORCIDWorks
    {
        /// <summary>
        /// ORCIDWorks
        /// </summary>
        public ORCIDWorks()
        {

        }
        /// <summary>
        /// group
        /// </summary>
        [DataMember(Name = "group")]
        public List<Group> group { get; set; }
        /// <summary>
        /// Group
        /// </summary>
        [DataContract]
        public class Group
        {
            /// <summary>
            /// Group
            /// </summary>
            public Group()
            {
            }
            /// <summary>
            /// work_summary
            /// </summary>
            [DataMember(Name = "work-summary")]
            public List<WorkSummary> work_summary { get; set; }
            /// <summary>
            /// WorkSummary
            /// </summary>
            [DataContract]
            public class WorkSummary
            {
                /// <summary>
                /// WorkSummary
                /// </summary>
                public WorkSummary()
                {
                }
                /// <summary>
                /// title
                /// </summary>
                [DataMember(Name = "title")]
                public Title title { get; set; }
                /// <summary>
                /// putcode
                /// </summary>
                [DataMember(Name = "put-code")]
                public string putcode { get; set; }
                /// <summary>
                /// externalids
                /// </summary>
                [DataMember(Name = "external-ids")]
                public Externalids externalids { get; set; }
                /// <summary>
                /// Title
                /// </summary>
                [DataContract]
                public class Title
                {
                    /// <summary>
                    /// Title
                    /// </summary>
                    public Title()
                    {
                    }
                    /// <summary>
                    /// title2
                    /// </summary>
                    [DataMember(Name = "title")]
                    public Title2 title2 { get; set; }
                    /// <summary>
                    /// Title2
                    /// </summary>
                    [DataContract]
                    public class Title2
                    {
                        /// <summary>
                        /// Title2
                        /// </summary>
                        public Title2()
                        {
                        }
                        /// <summary>
                        /// value
                        /// </summary>
                        [DataMember(Name = "value")]
                        public string value { get; set; }
                    }
                }
                /// <summary>
                /// Externalids
                /// </summary>
                [DataContract]
                public class Externalids
                {
                    /// <summary>
                    /// Externalids
                    /// </summary>
                    public Externalids()
                    {
                    }
                    /// <summary>
                    /// externalid
                    /// </summary>
                    [DataMember(Name = "external-id")]
                    public List<Externalid> externalid { get; set; }
                    /// <summary>
                    /// Externalid
                    /// </summary>
                    [DataContract]
                    public class Externalid
                    {
                        /// <summary>
                        /// Externalid
                        /// </summary>
                        public Externalid()
                        {
                        }
                        /// <summary>
                        /// externalidtype
                        /// </summary>
                        [DataMember(Name = "external-id-type")]
                        public string externalidtype { get; set; }
                        /// <summary>
                        /// externalidvalue
                        /// </summary>
                        [DataMember(Name = "external-id-value")]
                        public string externalidvalue { get; set; }
                    }
                }
            }
        }
    }
}
