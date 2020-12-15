// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de DOAJ
    /// </summary>
    public class DOAJ_API : I_ExternalAPI
    {
        public string Name { get { return "DOAJ"; } }

        public string Description { get { return "The DOAJ (Directory of Open Access Journals) was launched in 2003 with 300 open access journals. Today, this independent database contains over 15 000 peer-reviewed open access journals covering all areas of science, technology, medicine, social sciences, arts and humanities. Open access journals from all countries and in all languages are welcome to apply for inclusion."; } }

        public string HomePage { get { return "https://doaj.org/"; } }

        public string Id { get { return "doaj"; } }

        public static DOAJWorks GetWorks(string title)
        {
            DOAJWorks works = new DOAJWorks();
            works.results = new Result[] { };            
            DOAJWorks articles = GetArticles(title);
            if(articles!=null && articles.results!=null)
            {
                works.results = works.results.Union(articles.results).ToArray();
            }
            DOAJWorks journals = GetJournals(title);
            if (journals != null && journals.results != null)
            {
                works.results = works.results.Union(journals.results).ToArray();
            }
            return works;
        }

        private static DOAJWorks GetArticles(string title)
        {
            string cadena = "https://doaj.org/api/v2/search/articles/title:\"" + title + "\"";
            var doc = new WebClient().DownloadString(cadena);
            DOAJWorks doajWorks = JsonSerializer.Deserialize<DOAJWorks>(doc);

            return doajWorks;
        }
        private static DOAJWorks GetJournals(string title)
        {
            string cadena = "https://doaj.org/api/v2/search/journals/title:\"" + title + "\"";
            var doc = new WebClient().DownloadString(cadena);
            DOAJWorks doajWorks = JsonSerializer.Deserialize<DOAJWorks>(doc);

            return doajWorks;
        }
    }


    public class DOAJWorks
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public DateTime timestamp { get; set; }
        public string query { get; set; }
        public Result[] results { get; set; }
        public string next { get; set; }
        public string last { get; set; }
    }

    public class Result
    {
        public DateTime last_updated { get; set; }
        public Bibjson bibjson { get; set; }
        public DateTime created_date { get; set; }
        public string id { get; set; }
        public Admin admin { get; set; }
    }

    public class Bibjson
    {
        public Identifier[] identifier { get; set; }
        public Journal journal { get; set; }
        public string end_page { get; set; }
        public string[] keywords { get; set; }
        public string year { get; set; }
        public string start_page { get; set; }
        public Subject[] subject { get; set; }
        public DoajAuthor[] author { get; set; }
        public DoajLink[] link { get; set; }
        public string _abstract { get; set; }
        public string title { get; set; }
        public string month { get; set; }

        public bool active { get; set; }
        public string country { get; set; }
        public string publisher { get; set; }
        public string apc_url { get; set; }
        public string submission_charges_url { get; set; }
        public bool allows_fulltext_indexing { get; set; }
        public int publication_time { get; set; }
        public Oa_Start oa_start { get; set; }
        public Apc apc { get; set; }
        public Editorial_Review editorial_review { get; set; }
        public Plagiarism_Detection plagiarism_detection { get; set; }
        public Article_Statistics article_statistics { get; set; }
        public Author_Copyright author_copyright { get; set; }
        public Author_Publishing_Rights author_publishing_rights { get; set; }
        public string[] language { get; set; }
        public string[] format { get; set; }
        public string provider { get; set; }
        public Submission_Charges submission_charges { get; set; }
        public Archiving_Policy archiving_policy { get; set; }
        public string[] persistent_identifier_scheme { get; set; }
        public string[] deposit_policy { get; set; }
        public string institution { get; set; }
        public string alternative_title { get; set; }
    }

    public class Journal
    {
        public string volume { get; set; }
        public string number { get; set; }
        public string country { get; set; }
        public string[] issns { get; set; }
        public string publisher { get; set; }
        public string[] language { get; set; }
        public string title { get; set; }
    }

    public class Identifier
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Subject
    {
        public string code { get; set; }
        public string scheme { get; set; }
        public string term { get; set; }
    }

    public class DoajAuthor
    {
        public string name { get; set; }
        public string affiliation { get; set; }
    }

    public class DoajLink
    {
        public string content_type { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Oa_Start
    {
        public int year { get; set; }
    }

    public class Apc
    {
        public string currency { get; set; }
        public int average_price { get; set; }
    }

    public class Editorial_Review
    {
        public string process { get; set; }
        public string url { get; set; }
    }

    public class Plagiarism_Detection
    {
        public bool detection { get; set; }
        public string url { get; set; }
    }

    public class Article_Statistics
    {
        public bool statistics { get; set; }
        public string url { get; set; }
    }

    public class Author_Copyright
    {
        public string copyright { get; set; }
        public string url { get; set; }
    }

    public class Author_Publishing_Rights
    {
        public string publishing_rights { get; set; }
        public string url { get; set; }
    }

    public class Submission_Charges
    {
        public string currency { get; set; }
        public int average_price { get; set; }
    }

    public class Archiving_Policy
    {
        public string url { get; set; }
        public string nat_lib { get; set; }
        public string[] known { get; set; }
    }

    public class Admin
    {
        public bool ticked { get; set; }
        public bool seal { get; set; }
    }
}
