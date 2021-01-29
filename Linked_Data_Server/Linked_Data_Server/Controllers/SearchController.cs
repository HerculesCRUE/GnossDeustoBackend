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
        [HttpGet]
        public IActionResult Index(string q, int pagina)
        {
            SearchModelTemplate searchModelTemplate = new SearchModelTemplate();
            searchModelTemplate.entidades = new Dictionary<string, SearchModelTemplate.Entidad>();

            if(pagina == 0)
            {
                pagina = 1;
            }

            string consulta = @$"   select distinct ?s ?o ?rdfType where 
                                    {{
                                        ?s ?p ?o.
                                        ?s a ?rdfType.
                                        FILTER(?p in (<{string.Join(">,<", mConfigService.GetPropsTitle())}>) AND (lcase(?o) like'{q.ToLower()}*' OR lcase(?o) like'* {q.ToLower()}*'))
                                    }}OFFSET {(pagina-1)*10} limit 11";

            SparqlObject sparqlObject = SparqlUtility.SelectData(mConfigService.GetSparqlEndpoint(), mConfigService.GetSparqlGraph(), consulta, mConfigService.GetSparqlQueryParam());
            foreach (Dictionary<string, SparqlObject.Data> row in sparqlObject.results.bindings)
            {

                searchModelTemplate.entidades.Add(row["s"].value, new SearchModelTemplate.Entidad(row["o"].value, row["rdfType"].value));
            }

            if (pagina > 1)
            {
                searchModelTemplate.paginaAnterior = pagina - 1;
            }
            if (sparqlObject.results.bindings.Count()==11)
            {
                searchModelTemplate.paginaSiguiente = pagina + 1;
                searchModelTemplate.entidades.Remove(searchModelTemplate.entidades.Last().Key);
            }
            
            ViewData["Title"] = "Resultados para '" + q+"'";
            return View(searchModelTemplate);
        }
    }
}
