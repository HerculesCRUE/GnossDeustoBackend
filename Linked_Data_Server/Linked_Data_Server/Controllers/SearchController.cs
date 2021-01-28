using Linked_Data_Server.Models.Entities;
using Linked_Data_Server.Models.Services;
using Linked_Data_Server.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Controllers
{
    public class SearchController : Controller
    {
        private readonly static ConfigService mConfigService = new ConfigService();
        [HttpPost]
        public IActionResult Index(string q)
        {
            List<KeyValuePair<string, string>> response = new List<KeyValuePair<string, string>>();
            string consulta = @$"   select distinct ?s ?o where 
                                    {{
                                        ?s ?p ?o.
                                        FILTER(?p in (<{string.Join(">,<", mConfigService.GetPropsTitle())}>) AND (lcase(?o) like'{q.ToLower()}*' OR lcase(?o) like'* {q.ToLower()}*'))
                                    }}limit 10";
            SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {
                response.Add(new KeyValuePair<string, string>(row["o"].value, row["s"].value));
            }
            ViewData["Title"] = "Buscador de " + q;
            return View(response);
        }
    }
}
