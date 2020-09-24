using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
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
            if (discovery.Status.Equals("Error"))
            {
                model.Error = discovery.Error;
                model.JobId = discovery.JobID;
                model.IdDiscoverItem = discovery.ID;
            }
            else
            {
                model.JobId = discovery.JobID;
                
                foreach (var item in discovery.DissambiguationProblems)
                {
                    model.IdDiscoverItem = item.DiscoverItemID;
                    if (!model.DissambiguationProblems.ContainsKey(item.IDOrigin))
                    {
                        model.DissambiguationProblems.Add(item.IDOrigin, new List<string>());
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