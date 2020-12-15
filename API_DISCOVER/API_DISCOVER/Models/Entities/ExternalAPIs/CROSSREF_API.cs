// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using Newtonsoft.Json;
using System;
using System.Net;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de CROSSREF
    /// </summary>
    public class CROSSREF_API:I_ExternalAPI
    {
        public string Name { get { return "Crossref"; } }
        public string Description { get { return "Crossref makes research outputs easy to find, cite, link, assess, and reuse. A not-for-profit membership organization that exists to make scholarly communications better."; } }

        public string HomePage { get { return "https://www.crossref.org/"; } }

        public string Id { get { return "crossref"; } }

        /// <summary>
        /// Busca documentos en función de sus autores en el API de CROSSREF
        /// </summary>
        /// <param name="q">Texto a buscar</param>
        /// <param name="CrossrefUserAgent">user agent para usar en las peticiones al API de CROSSREF</param>
        /// <returns>Objeto con los documentos y las personas</returns>
        public static CROSSREF_Works WorkSearchByContributor(string q, string CrossrefUserAgent)
        {
            WebClient webClient = new WebClient();
            if (!string.IsNullOrEmpty(CrossrefUserAgent))
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, CrossrefUserAgent);
            }
            string jsonRespuesta = webClient.DownloadString($"https://api.crossref.org/works?query.author={q}&rows=200");
            return JsonConvert.DeserializeObject<CROSSREF_Works>(jsonRespuesta);
        }
    }


    public class CROSSREF_Works
    {
        public string status { get; set; }
        public string messagetype { get; set; }
        public string messageversion { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public Facets facets { get; set; }
        public int totalresults { get; set; }
        public Item[] items { get; set; }
        public int itemsperpage { get; set; }
        public Query query { get; set; }
    }

    public class Facets
    {
    }

    public class Query
    {
        public int startindex { get; set; }
        public object searchterms { get; set; }
    }

    public class Item
    {
        public Indexed indexed { get; set; }
        public int referencecount { get; set; }
        public string publisher { get; set; }
        public IsbnType[] isbntype { get; set; }
        public ContentDomain contentdomain { get; set; }
        public PublishedPrint publishedprint { get; set; }
        public string DOI { get; set; }
        public string type { get; set; }
        public Created created { get; set; }
        public string source { get; set; }
        public int isreferencedbycount { get; set; }
        public string[] title { get; set; }
        public string prefix { get; set; }
        public Author[] author { get; set; }
        public string member { get; set; }
        public PublishedOnline publishedonline { get; set; }
        public Event _event { get; set; }
        public string[] containertitle { get; set; }
        public Deposited deposited { get; set; }
        public float score { get; set; }
        public Issued issued { get; set; }
        public string[] ISBN { get; set; }
        public int referencescount { get; set; }
        public string[] alternativeid { get; set; }
        public string URL { get; set; }
        public string publisherlocation { get; set; }
        public Link[] link { get; set; }
        public string[] subtitle { get; set; }
        public string issue { get; set; }
        public License[] license { get; set; }
        public string[] shortcontainertitle { get; set; }
        public string page { get; set; }
        public string volume { get; set; }
        public Reference[] reference { get; set; }
        public string language { get; set; }
        public JournalIssue journalissue { get; set; }
        public Relation relation { get; set; }
        public string[] ISSN { get; set; }
        public IssnType[] issntype { get; set; }
        public string[] subject { get; set; }
        public string updatepolicy { get; set; }
        public Assertion[] assertion { get; set; }
        public string _abstract { get; set; }
        public Funder[] funder { get; set; }
    }

    public class Indexed
    {
        public int[][] dateparts { get; set; }
        public DateTime datetime { get; set; }
        public long timestamp { get; set; }
    }

    public class ContentDomain
    {
        public string[] domain { get; set; }
        public bool crossmarkrestriction { get; set; }
    }

    public class PublishedPrint
    {
        public int[][] dateparts { get; set; }
    }

    public class Created
    {
        public int[][] dateparts { get; set; }
        public DateTime datetime { get; set; }
        public long timestamp { get; set; }
    }

    public class PublishedOnline
    {
        public int[][] dateparts { get; set; }
    }

    public class Event
    {
        public string name { get; set; }
        public string location { get; set; }
        public string[] sponsor { get; set; }
        public string acronym { get; set; }
        public string number { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
    }

    public class Start
    {
        public int[][] dateparts { get; set; }
    }

    public class End
    {
        public int[][] dateparts { get; set; }
    }

    public class Deposited
    {
        public int[][] dateparts { get; set; }
        public DateTime datetime { get; set; }
        public long timestamp { get; set; }
    }

    public class Issued
    {
        public int[][] dateparts { get; set; }
    }

    public class JournalIssue
    {
        public PublishedOnline1 publishedonline { get; set; }
        public string issue { get; set; }
        public PublishedPrint1 publishedprint { get; set; }
    }

    public class PublishedOnline1
    {
        public int[][] dateparts { get; set; }
    }

    public class PublishedPrint1
    {
        public int[][] dateparts { get; set; }
    }

    public class Relation
    {
        public object[] cites { get; set; }
    }

    public class IsbnType
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Author
    {
        public string given { get; set; }
        public string family { get; set; }
        public string sequence { get; set; }
        public Affiliation[] affiliation { get; set; }
        public string ORCID { get; set; }
        public bool authenticatedorcid { get; set; }
    }

    public class Affiliation
    {
        public string name { get; set; }
    }

    public class Link
    {
        public string URL { get; set; }
        public string contenttype { get; set; }
        public string contentversion { get; set; }
        public string intendedapplication { get; set; }
    }

    public class License
    {
        public string URL { get; set; }
        public Start1 start { get; set; }
        public int delayindays { get; set; }
        public string contentversion { get; set; }
    }

    public class Start1
    {
        public int[][] dateparts { get; set; }
        public DateTime datetime { get; set; }
        public long timestamp { get; set; }
    }

    public class Reference
    {
        public string key { get; set; }
        public string doiassertedby { get; set; }
        public string firstpage { get; set; }
        public string DOI { get; set; }
        public string articletitle { get; set; }
        public string volume { get; set; }
        public string author { get; set; }
        public string year { get; set; }
        public string journaltitle { get; set; }
        public string seriestitle { get; set; }
        public string unstructured { get; set; }
        public string volumetitle { get; set; }
        public string issue { get; set; }
    }

    public class IssnType
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Assertion
    {
        public string value { get; set; }
        public string name { get; set; }
        public string label { get; set; }
    }

    public class Funder
    {
        public string DOI { get; set; }
        public string name { get; set; }
        public string doiassertedby { get; set; }
        public string[] award { get; set; }
    }

}
