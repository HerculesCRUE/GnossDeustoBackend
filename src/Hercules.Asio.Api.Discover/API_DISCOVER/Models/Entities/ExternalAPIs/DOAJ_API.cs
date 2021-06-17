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
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return "DOAJ"; } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get { return "The DOAJ (Directory of Open Access Journals) was launched in 2003 with 300 open access journals. Today, this independent database contains over 15 000 peer-reviewed open access journals covering all areas of science, technology, medicine, social sciences, arts and humanities. Open access journals from all countries and in all languages are welcome to apply for inclusion."; } }
        /// <summary>
        /// HomePage
        /// </summary>
        public string HomePage { get { return "https://doaj.org/"; } }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get { return "doaj"; } }
        /// <summary>
        /// GetWorks
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
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

    /// <summary>
    /// DOAJWorks
    /// </summary>
    public class DOAJWorks
    {
        /// <summary>
        /// results
        /// </summary>
        public Result[] results { get; set; }
    }

    /// <summary>
    /// Result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// bibjson
        /// </summary>
        public Bibjson bibjson { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }

    /// <summary>
    /// Bibjson
    /// </summary>
    public class Bibjson
    {
        /// <summary>
        /// identifier
        /// </summary>
        public Identifier[] identifier { get; set; }
        /// <summary>
        /// author
        /// </summary>
        public DoajAuthor[] author { get; set; }
        /// <summary>
        /// title
        /// </summary>
        public string title { get; set; }
    }

    /// <summary>
    /// Identifier
    /// </summary>
    public class Identifier
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// type
        /// </summary>
        public string type { get; set; }
    }

    /// <summary>
    /// DoajAuthor
    /// </summary>
    public class DoajAuthor
    {
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }
    }
}
