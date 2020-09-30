// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para gestionar el descubrimiento
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para gestionar el descubrimiento
    /// </summary>
    public class DiscoverController : Controller
    {
        readonly DiscoverItemBDService _discoverItemService;
        public DiscoverController(DiscoverItemBDService iIDiscoverItemService)
        {
            _discoverItemService = iIDiscoverItemService;
        }
        /// <summary>
        /// Obtiene los detalles de un error de descubrimiento
        /// </summary>
        /// <returns></returns>
        public IActionResult Details(Guid itemId)
        {
            var discovery = _discoverItemService.GetDiscoverItemById(itemId);
            DiscoverItemViewModel model = new DiscoverItemViewModel();
            model.DissambiguationProblems = new Dictionary<string, List<string>>();
            model.DissambiguationProblemsTitles = new Dictionary<string, string>();
            if (discovery.Status.Equals("Error"))
            {
                model.Error = discovery.Error;
                model.JobId = discovery.JobID;
                model.IdDiscoverItem = discovery.ID;
            }
            else
            {
                RohGraph dataGraph = new RohGraph();
                dataGraph.LoadFromString(discovery.DiscoverRdf, new RdfXmlParser());
                model.JobId = discovery.JobID;
                
                foreach (var item in discovery.DissambiguationProblems)
                {
                    model.IdDiscoverItem = item.DiscoverItemID;
                    if (!model.DissambiguationProblems.ContainsKey(item.IDOrigin))
                    {
                        model.DissambiguationProblems.Add(item.IDOrigin, new List<string>());
                        model.DissambiguationProblemsTitles.Add(item.IDOrigin, "");
                        SparqlResultSet sparqlResultSet = (SparqlResultSet)dataGraph.ExecuteQuery("select ?title where{<" + item.IDOrigin + "> ?prop ?title. FILTER(?prop in (<http://purl.org/roh#title>,<http://purl.org/roh/mirror/foaf#name>))}");
                        foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                        {
                            model.DissambiguationProblemsTitles[item.IDOrigin]= ((LiteralNode)(sparqlResult["title"])).Value;
                        }
                    }
                    foreach (var problem in item.DissambiguationCandiates)
                    {
                        string opcion = $"{problem.IDCandidate} || {Math.Round(problem.Score,3)}";
                        model.DissambiguationProblems[item.IDOrigin].Add(opcion);
                    }
                }
            }
            return View(model);
        }
    }
}