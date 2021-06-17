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
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return "Crossref"; } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get { return "Crossref makes research outputs easy to find, cite, link, assess, and reuse. A not-for-profit membership organization that exists to make scholarly communications better."; } }
        /// <summary>
        /// HomePage
        /// </summary>
        public string HomePage { get { return "https://www.crossref.org/"; } }
        /// <summary>
        /// Id
        /// </summary>
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

    /// <summary>
    /// CROSSREF_Works
    /// </summary>
    public class CROSSREF_Works
    {
        /// <summary>
        /// Message
        /// </summary>
        public Message message { get; set; }
    }

    /// <summary>
    /// Message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Items
        /// </summary>
        public Item[] items { get; set; }
    }

    /// <summary>
    /// Item
    /// </summary>
    public class Item
    {
        /// <summary>
        /// DOI
        /// </summary>
        public string DOI { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        public string[] title { get; set; }
        /// <summary>
        /// Author
        /// </summary>
        public Author[] author { get; set; }
    }

    /// <summary>
    /// Author
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Given
        /// </summary>
        public string given { get; set; }
        /// <summary>
        /// Family
        /// </summary>
        public string family { get; set; }
        /// <summary>
        /// ORCID
        /// </summary>
        public string ORCID { get; set; }
    }
}
