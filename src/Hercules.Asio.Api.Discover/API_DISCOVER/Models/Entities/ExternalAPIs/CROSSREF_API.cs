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
            webClient.Dispose();
            return JsonConvert.DeserializeObject<CROSSREF_Works>(jsonRespuesta);
        }
    }


    public class CROSSREF_Works
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public Item[] items { get; set; }
    }

    public class Item
    {
        public string DOI { get; set; }
        public string[] title { get; set; }
        public Author[] author { get; set; }
    }

    public class Author
    {
        public string given { get; set; }
        public string family { get; set; }
        public string ORCID { get; set; }
    }
}
