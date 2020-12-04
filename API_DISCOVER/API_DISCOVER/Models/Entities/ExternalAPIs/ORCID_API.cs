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
    {//TODO cambiar
        public string Name { get { return "ORCID"; } }

        public string HomePage { get { return "https://orcid.org/"; } }

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
            return JsonConvert.DeserializeObject<ORCIDWorks>(jsonRespuestaOrcidWorks);
        }
    }

    [DataContract]
    public class ORCIDExpandedSearch
    {
        public ORCIDExpandedSearch() { 

        }

        [DataMember(Name = "expanded-result")]
        public List<Result> expanded_result { get; set; }

        [DataContract]
        public class Result
        {
            public Result() { 
            }
            [DataMember(Name = "orcid-id")]
            public string orcid_id { get; set; }
            [DataMember(Name = "given-names")]
            public string given_names { get; set; }
            [DataMember(Name = "family-names")]
            public string family_names { get; set; }
        }
    }

    [DataContract]
    public class ORCIDPerson
    {
        public ORCIDPerson()
        {

        }

        [DataMember(Name = "external-identifiers")]
        public ExternalIdentifiers external_identifiers { get; set; }

        [DataContract]
        public class ExternalIdentifiers
        {
            public ExternalIdentifiers()
            {
            }
            [DataMember(Name = "external-identifier")]
            public List<ExternalIdentifier> external_identifier { get; set; }

            [DataContract]
            public class ExternalIdentifier
            {
                public ExternalIdentifier()
                {
                }
                [DataMember(Name = "external-id-type")]
                public string external_id_type { get; set; }
                [DataMember(Name = "external-id-value")]
                public string external_id_value { get; set; }
            }
        }
    }

    [DataContract]
    public class ORCIDWorks
    {
        public ORCIDWorks()
        {

        }
        [DataMember(Name = "group")]
        public List<Group> group { get; set; }

        [DataContract]
        public class Group
        {
            public Group()
            {
            }
            [DataMember(Name = "work-summary")]
            public List<WorkSummary> work_summary { get; set; }

            [DataContract]
            public class WorkSummary
            {
                public WorkSummary()
                {
                }
                [DataMember(Name = "title")]
                public Title title { get; set; }
                [DataMember(Name = "put-code")]
                public string putcode { get; set; }

                [DataMember(Name = "external-ids")]
                public Externalids externalids { get; set; }

                [DataContract]
                public class Title
                {
                    public Title()
                    {
                    }
                    [DataMember(Name = "title")]
                    public Title2 title2 { get; set; }

                    [DataContract]
                    public class Title2
                    {
                        public Title2()
                        {
                        }
                        [DataMember(Name = "value")]
                        public string value { get; set; }
                    }
                }

                [DataContract]
                public class Externalids
                {
                    public Externalids()
                    {
                    }
                    [DataMember(Name = "external-id")]
                    public List<Externalid> externalid { get; set; }

                    [DataContract]
                    public class Externalid
                    {
                        public Externalid()
                        {
                        }
                        [DataMember(Name = "external-id-type")]
                        public string externalidtype { get; set; }

                        [DataMember(Name = "external-id-value")]
                        public string externalidvalue { get; set; }
                    }
                }
            }
        }
    }
}
