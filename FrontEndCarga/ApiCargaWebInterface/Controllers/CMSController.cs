using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services.VirtualPathProvider;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    public class CMSController : Controller
    {
        CallApiVirtualPath _documentationApi;
        public CMSController(CallApiVirtualPath documentationApi)
        {
            _documentationApi = documentationApi;
        }

        [HttpGet("{*url}", Order = int.MaxValue)]
        public IActionResult GetRoute(string url)
        {
            var page = _documentationApi.GetPage($"/{url}");
            if(page == null)
            {
                return NotFound();
            }
            return View($"/{url}");
        }

        public IActionResult Index()
        {
            var pages = _documentationApi.GetPages();
            List<PageViewModel> pagesViewModel = new List<PageViewModel>();
            foreach(var page in pages)
            {
                PageViewModel pageViewModel = new PageViewModel()
                {
                    Route = page.Route
                };
                pagesViewModel.Add(pageViewModel);
            }
            return View(pagesViewModel);
        }

        public IActionResult Details()
        {
            var pages = _documentationApi.GetPages();
            List<PageViewModel> pagesViewModel = new List<PageViewModel>();
            foreach (var page in pages)
            {
                PageViewModel pageViewModel = new PageViewModel()
                {
                    Route = page.Route
                };
                pagesViewModel.Add(pageViewModel);
            }
            return View(pagesViewModel);
        }
    }
}