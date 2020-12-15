// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para gestionar elar las páginas creadas por los usuarios
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.Models.Services.VirtualPathProvider;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para gestionar elar las páginas creadas por los usuarios
    /// </summary>
    public class CMSController : Controller
    {
        CallApiVirtualPath _documentationApi;
        ReplaceUsesService _replaceUsesService;
        public CMSController(CallApiVirtualPath documentationApi, ReplaceUsesService replaceUsesService)
        {
            _documentationApi = documentationApi;
            _replaceUsesService = replaceUsesService;
        }

        /// <summary>
        /// Es el método encargado de enrutar las páginas creadas por los usuarios
        /// </summary>
        /// <param name="url">url de la página a buscar</param>
        /// <returns></returns>
        [HttpGet("{*url}", Order = int.MaxValue)]
        public IActionResult GetRoute(string url)
        {
            var page = _documentationApi.GetPage($"/{url}");
            if(page == null)
            {
                return NotFound();
            }
            //Cambiar el contendio de los uses
            CmsDataViewModel dataModel = new CmsDataViewModel();
            if (page.Content.Contains("@*<%"))
            {
                dataModel = _replaceUsesService.PageWithDirectives(page.Content, dataModel);
            }
            return View($"/{url}", dataModel);
        }

        /// <summary>
        /// Obtiene el listado de las páginas creadas
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var pages = _documentationApi.GetPages();
            pages = pages.OrderBy(item => item.Route).ToList();
            List<PageViewModel> pagesViewModel = new List<PageViewModel>();
            foreach (var page in pages)
            {  
                PageViewModel pageViewModel = new PageViewModel()
                {
                    Route = page.Route,
                    PageId = page.PageId,
                    LastModified = page.LastModified
                };
                pagesViewModel.Add(pageViewModel);
            }
            return View(pagesViewModel);
        }
        /// <summary>
        /// Obtiene los datos de una página
        /// </summary>
        /// <param name="route">Ruta de la página</param>
        /// <returns></returns>
        public IActionResult Details(string route)
        {
            var page = _documentationApi.GetPage(route);
            ConfigUrlService urlService = new ConfigUrlService();
            string routeProxy = $"{urlService.GetProxy()}{page.Route}";
            PageViewModel pageViewModel = new PageViewModel()
            {
                Route = routeProxy,
                RouteProxyLess = page.Route,
                LastModified = page.LastModified,
                PageId = page.PageId
            };
            return View(pageViewModel);
        }
        /// <summary>
        /// Elimina una página
        /// </summary>
        /// <param name="page_id">Identificador de la página</param>
        /// <returns></returns>
        public IActionResult Delete(Guid page_id)
        {
            _documentationApi.DeletePage(page_id);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// devuelve el formulario de creación/edición de una página
        /// </summary>
        /// <param name="page_id">Identificador de la página en el caso que sea una edición</param>
        /// <param name="route">Ruta de la página en el caso de que sea una edición</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create(Guid page_id, string route)
        {
            
            
            PageViewModel pageModel = new PageViewModel();
            pageModel.PageId = page_id;
            pageModel.Route = route;
            return View(pageModel);
        }
        /// <summary>
        /// Crea o modifica una página
        /// </summary>
        /// <param name="new_page">Datos nuevos de la página</param>
        /// <returns></returns>
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
            return RedirectToAction("Details", new { route = new_page.Route });
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
                var content = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(page.Content));
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