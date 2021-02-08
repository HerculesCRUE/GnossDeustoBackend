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
        public Result[] results { get; set; }
    }

    public class Result
    {
        public Bibjson bibjson { get; set; }
        public string id { get; set; }
    }

    public class Bibjson
    {
        public Identifier[] identifier { get; set; }
        public DoajAuthor[] author { get; set; }
        public string title { get; set; }
    }
    
    public class Identifier
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class DoajAuthor
    {
        public string name { get; set; }
    }
}
