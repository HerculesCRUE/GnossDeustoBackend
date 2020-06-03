using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    public class UrisFactoryController : Controller
    {
        ICallUrisFactoryApiService _callUrisFactoryService;
        public UrisFactoryController(ICallUrisFactoryApiService callUrisFactoryService)
        {
            _callUrisFactoryService = callUrisFactoryService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetUri(string Resource_class, string Identifier)
        {
            UrisFactoryResultViewModel urisFactoryModel = new UrisFactoryResultViewModel();
            urisFactoryModel.Identifier = Identifier;
            urisFactoryModel.Resource_class = Identifier;
            try
            {
                urisFactoryModel.UriResult = _callUrisFactoryService.GetUri(Resource_class, Identifier);
            }
            catch(HttpRequestException ex)
            {
                ModelState.AddModelError("Resource_class", ex.Message);
            }
            return View("Index", urisFactoryModel);
        }
    }
}