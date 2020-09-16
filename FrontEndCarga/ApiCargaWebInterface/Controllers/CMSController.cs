using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services.VirtualPathProvider;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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
                    Route = page.Route,
                    PageId = page.PageId
                };
                pagesViewModel.Add(pageViewModel);
            }
            return View(pagesViewModel);
        }

        public IActionResult Details(string route)
        {
            var page = _documentationApi.GetPage(route);

            PageViewModel pageViewModel = new PageViewModel()
            {
                Route = page.Route,
                LastModified = page.LastModified,
                PageId = page.PageId
            };
            return View(pageViewModel);
        }

        public IActionResult Delete(Guid page_id)
        {
            _documentationApi.DeletePage(page_id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create(Guid page_id, string route)
        {
            
            
            PageViewModel pageModel = new PageViewModel();
            pageModel.PageId = page_id;
            pageModel.Route = route;
            return View(pageModel);
        }

        [HttpPost]
        public IActionResult Create(PageViewModel new_page)
        {
            var page = _documentationApi.GetPage(new_page.Route);
            if (page != null && new_page.PageId.Equals(Guid.Empty))
            {
                ModelState.AddModelError("Route", "Ruta ya usada");
            }
            if (!ModelState.IsValid)
            {
                return View("Create", new_page);
            }
            _documentationApi.CreatePage(new_page.PageId, new_page.Route, new_page.FileHtml);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Obtiene el esquema de uris configurado
        /// </summary>
        /// <returns></returns>
        [HttpGet("[Controller]/download/page")]
        public IActionResult DownloadHtml(string route)
        {
            var page = _documentationApi.GetPage(route);
            if (page != null)
            {
                string name = route.Split("/").Last();
                var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(page.Content));
                var contentType = "APPLICATION/octet-stream";
                var fileName = $"{name}.cshtml";
                return File(content, contentType, fileName);
            }
            else
            {
                return NotFound();
            }

        }
    }
}