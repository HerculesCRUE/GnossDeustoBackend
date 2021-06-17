using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using ApiCargaWebInterface.Models.Services;
using System.Net;
using System.IO;

namespace ApiCargaWebInterface.Controllers
{
    public class AutocompleteController : Controller
    {
        readonly ConfigUrlService _ConfigUrlService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configUrlService"></param>
        public AutocompleteController(ConfigUrlService configUrlService)
        {
            _ConfigUrlService = configUrlService;
        }

        public IActionResult Index(string q)
        {
            // Obtiene la URL del servicio al cual hay que hacer la petición.
            Uri url = new Uri(_ConfigUrlService.GetGraph());
            string urlPeticion = $@"{url.Scheme}://{url.Host}/autocomplete?q={q}";

            // Compruebo que la URL esté bien formada.
            if (!Uri.IsWellFormedUriString(urlPeticion, UriKind.Absolute))
            {
                return BadRequest();
            }

            // Hace la petición.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPeticion);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Transforma el response en string.
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string responseString = reader.ReadToEnd();

            // Devuelve el resultado en Json.
            List<KeyValuePair<string, string>> resultado = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(responseString);
            return Json(resultado);
        }

        public IActionResult GetUrlSearch()
        {
            // Obtiene la URL del servicio al cual hay que hacer la petición.
            Uri url = new Uri(_ConfigUrlService.GetGraph());
            return Json($@"{url.Scheme}://{url.Host}/Search");
        }
    }
}
