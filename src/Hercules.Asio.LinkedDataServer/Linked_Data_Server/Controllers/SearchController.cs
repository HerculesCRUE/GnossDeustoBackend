using Hercules.Asio.LinkedDataServer.Utility;
using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using Linked_Data_Server.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Controllers
{
    public class SearchController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        private readonly static Config_Linked_Data_Server mLinked_Data_Server_Config = LoadLinked_Data_Server_Config();
        private readonly ISparqlUtility _sparqlUtility;

        public SearchController(ISparqlUtility sparqlUtility)
        {
            _sparqlUtility = sparqlUtility;
        }
        [HttpGet]
        public IActionResult Index(string q, int pagina)
        {
            SearchModelTemplate searchModelTemplate = GenerateSearchTemplate(q, pagina);

            ViewData["Title"] = "Resultados para '" + q+"'";
            return View(searchModelTemplate);
        }
        [NonAction]
        public SearchModelTemplate GenerateSearchTemplate(string q, int pagina)
        {
            ViewBag.UrlHome = mConfigService.GetUrlHome();
            SearchModelTemplate searchModelTemplate = new SearchModelTemplate();
            searchModelTemplate.entidades = new Dictionary<string, SearchModelTemplate.Entidad>();

            if (pagina == 0)
            {
                pagina = 1;
            }

            string consulta = @$"   select distinct ?s ?o ?rdfType where 
                                    {{
                                        ?s ?p ?o.
                                        ?s a ?rdfType.
                                        FILTER(?p in (<{string.Join(">,<", mLinked_Data_Server_Config.PropsTitle)}>))
                                        FILTER(regex(?o, '^{SparqlUtility.GetRegexSearch(q)}','i') || regex(?o, ' {SparqlUtility.GetRegexSearch(q)}','i'))
                                    }}order by asc(?o) asc (?s) OFFSET {(pagina - 1) * 10} limit 11";
            string pXAppServer = "";
            SparqlObject sparqlObject = _sparqlUtility.SelectData(mConfigService, mConfigService.GetSparqlGraph(), consulta, ref pXAppServer);
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {
                if (!searchModelTemplate.entidades.ContainsKey(row["s"].value))
                {
                    searchModelTemplate.entidades.Add(row["s"].value, new SearchModelTemplate.Entidad(row["o"].value, row["rdfType"].value));
                }
            }

            if (pagina > 1)
            {
                searchModelTemplate.paginaAnterior = pagina - 1;
            }
            if (sparqlObject.results.bindings.Count() == 11)
            {
                searchModelTemplate.paginaSiguiente = pagina + 1;
                searchModelTemplate.entidades.Remove(searchModelTemplate.entidades.Last().Key);
            }
            return searchModelTemplate;

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
