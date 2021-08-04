using Linked_Data_Server.Models;
using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using Linked_Data_Server.Utility;
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
using Hercules.Asio.LinkedDataServer.Utility;

namespace Linked_Data_Server.Controllers
{

    public class AutocompleteController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly static Config_Linked_Data_Server mLinked_Data_Server_Config = LoadLinked_Data_Server_Config();
        private readonly ILogger<HomeController> _logger;
        private readonly ISparqlUtility _sparqlUtility;

        public AutocompleteController(ISparqlUtility sparqlUtility, ILogger<HomeController> logger)
        {
            _logger = logger;
            _sparqlUtility = sparqlUtility;
        }


        public IActionResult Index(string q)
        {
            List<KeyValuePair<string, string>> response = new List<KeyValuePair<string, string>>();
            string searchAutocompletar = SparqlUtility.GetSearchAutocompletar(q);
            if (!string.IsNullOrEmpty(searchAutocompletar))
            {
                string consulta = @$"     select distinct ?s ?o ?rdfType where 
                                    {{                                        
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle)}>))
                                        ?s a ?rdfType.
                                        ?s ?p ?o.
                                        {SparqlUtility.GetSearchAutocompletar(q)}
                                    }}order by desc(?sc) asc(?o) asc (?s) limit 10 ";
                string pXAppServer = "";
                SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
                foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
                {
                    //Las categorías tienen otro comportamiento (revisar categorías no unesko)
                    if (row["rdfType"].value == "http://www.w3.org/2004/02/skos/core#Concept")
                    {
                        string url = $"{Request.Scheme}://{Request.Host}/Search?concept={row["s"].value}";
                        response.Add(new KeyValuePair<string, string>(row["o"].value, url));
                    }
                    else
                    {
                        response.Add(new KeyValuePair<string, string>(row["o"].value, row["s"].value));
                    }
                }
            }
            return Json(response);
        }

        /// <summary>
        /// Cargamos las configuraciones 
        /// </summary>
        /// <returns></returns>
        private static Config_Linked_Data_Server LoadLinked_Data_Server_Config()
        {
            return JsonConvert.DeserializeObject<Config_Linked_Data_Server>(System.IO.File.ReadAllText("Config/Linked_Data_Server_Config.json"));
        }
    }
}
