// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
//Controlador para la gestión de páginas
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorDocumentacion.Models.Entities;
using GestorDocumentacion.Models.Services;
using GestorDocumentacion.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GestorDocumentacion.Controllers
{
    ///<summary>
    ///Controlador para la gestión de páginas
    ///</summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class PageController : ControllerBase
    {
        private IPagesOperationsServices _pagesOperationsService;
        private IFileOperationService _fileOperationsService;
        public PageController(IPagesOperationsServices pagesOperationsService, IFileOperationService fileOperationsService)
        {
            _pagesOperationsService = pagesOperationsService;
            _fileOperationsService = fileOperationsService;
        }
        ///<summary>
        ///Devuelve el HTML de una página web, incluyendo sus metadatos.
        ///</summary>
        ///<param name="route">Ruta configurada para el archivo</param>
        [HttpGet]
        public IActionResult GetPage(string route)
        {
            var page = _pagesOperationsService.GetPage(route);
            if(page != null)
            {
                return Ok(page);
            }
            return Ok(null);
        }

        /// <summary>
        /// Devuelve una lista de las páginas web cargadas.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IActionResult GetPages()
        {
            var pages = _pagesOperationsService.GetPages();
            List<PageViewModel> pageViewModelList = new List<PageViewModel>();
            foreach (var page in pages)
            {
                PageViewModel pageModel = new PageViewModel()
                {
                    LastModified = page.LastModified,
                    PageID = page.PageID,
                    Route = page.Route
                };
                pageViewModelList.Add(pageModel);
            }
            return Ok(pageViewModelList);
        }

        /// <summary>
        /// Carga o modifica una página web e incluye información acerca de la página, como la URL, metadatos title o description, etc.
        /// </summary>
        /// <param name="route">Ruta nueva de la página</param>
        /// <param name="pageId">Identificador de la página a modificar, en el caso de que se quiera añadir una nueva hay que dejar este campo vacio</param>
        /// <param name="html_page">Contenido html de la página</param>
        /// <returns></returns>
        [HttpPost]
        [Route("load")]
        public IActionResult LoadPage(string route, Guid pageId, IFormFile html_page)
        {
            Guid guidPage = Guid.Empty;
            string content = "";
            bool isNew = false;
            if (Guid.Empty.Equals(pageId))
            {
                guidPage = Guid.NewGuid();
                isNew = true;
            }
            else
            {
                guidPage = pageId;
            }
            if(html_page != null)
            {
                content = _fileOperationsService.ReadFile(html_page);
            }
            Page page = new Page()
            {
                LastModified = DateTime.Now,
                PageID = guidPage,
                Route = route,
                Content = content
            };
            if(!_pagesOperationsService.LoadPage(page, isNew))
            {
                return BadRequest($"The page with route {route} already exist");
            }
            return Ok(guidPage);
            

        }

        /// <summary>
        /// Elimina una página web.
        /// </summary>
        /// <param name="pageId">Identificador de la página html</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        public IActionResult DeletePage(Guid pageId)
        {
            return Ok(_pagesOperationsService.DeletePage(pageId));
        }
    }
}
