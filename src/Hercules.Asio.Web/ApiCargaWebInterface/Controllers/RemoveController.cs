using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models;
using ApiCargaWebInterface.Models.Services;
using Hercules.Asio.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.Web.Controllers
{
    [ClaimRequirement("Administrator", "true")]
    public class RemoveController : Controller
    {
        readonly ICallEtlService _callRemoverService;
        public RemoveController(ICallEtlService callRemoverService)
        {
            _callRemoverService = callRemoverService;
        }

        /// <summary>
        /// Obtiene una lista de todos los repositorio OAIPMH
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            RemoveViewModel removerConfigView = new RemoveViewModel();
            return View(removerConfigView);
        }

        /// <summary>
        /// Edita la información de un repositorio OAIPMH
        /// </summary>
        /// <param name="removerConfigView">Información modificada del repositorio OAIPMH</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]")]
        public IActionResult Remove(string pEntity)
        {
            RemoveViewModel removerConfigView = new RemoveViewModel();
            removerConfigView.entity = pEntity;

            try
            {
                _callRemoverService.CallRemover(pEntity);                
                removerConfigView.resultado = $@"Entidad {pEntity} borrada correctamente.";
                return View("Index", removerConfigView);
            }
            catch (Exception)
            {
                removerConfigView.resultado = $@"ERROR: La entidad {pEntity} no se ha podido borrar.";
                return View("Index", removerConfigView);
            }           
        }
    }
}
