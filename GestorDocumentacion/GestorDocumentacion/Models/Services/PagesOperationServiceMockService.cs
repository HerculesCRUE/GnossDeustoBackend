// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la simulación de la gestión de páginas
using GestorDocumentacion.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    public class PagesOperationServiceMockService : IPagesOperationsServices
    {
        private List<Page> _pageList;
        public PagesOperationServiceMockService()
        {
            _pageList = new List<Page>();
            Page page1 = new Page()
            {
                PageID = Guid.NewGuid(),
                Name = "Page1",
                Route = "/pages/page1.html",
                Content = "<html><head></head><body><p>hola mundo</p></body></html>"
            };
            Page page2 = new Page()
            {
                PageID = Guid.NewGuid(),
                Name = "Page2",
                Route = "/pages/page2.html",
                Content = "<html><head></head><body><p>hola mundo</p></body></html>"
            };
            _pageList.Add(page1);
            _pageList.Add(page2);
        }
        /// <summary>
        /// Elimina una página
        /// </summary>
        /// <param name="pageID">Identificador de la página a eliminar</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeletePage(Guid pageID)
        {
            Page page = _pageList.FirstOrDefault(page => page.PageID.Equals(pageID));
            if (page != null)
            {
                _pageList.Remove(page);
            }
            return true;
        }
        /// <summary>
        /// Obtiene una página por su nombre
        /// </summary>
        /// <param name="name">Nombre de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(string name)
        {
            Page page = _pageList.FirstOrDefault(page => page.Name.Equals(name));
            return page;
        }
        /// <summary>
        /// Obtiene una página por su identificador
        /// </summary>
        /// <param name="pageID">Identificador de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(Guid pageID)
        {
            Page page = _pageList.FirstOrDefault(page => page.PageID.Equals(pageID));
            return page;
        }
        /// <summary>
        /// Obtiene una lista de páginas
        /// </summary>
        /// <returns>Lista de objetos página</returns>
        public List<Page> GetPages()
        {
            return _pageList;
        }
        /// <summary>
        /// Carga o modifica una página
        /// </summary>
        /// <param name="page">página nueva o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadPage(Page page, bool isNew)
        {

            if (isNew)
            {
                if (page != null && !string.IsNullOrEmpty(page.Content) && !string.IsNullOrEmpty(page.Name) && GetPage(page.Name) == null)
                {
                    _pageList.Add(page);
                    return true;
                }
            }
            else
            {
                var pageModify = GetPage(page.PageID);
                if (!string.IsNullOrEmpty(page.Content) && page.Content != pageModify.Content)
                {
                    pageModify.Content = page.Content;
                }
                if (!string.IsNullOrEmpty(page.Name) && page.Name != pageModify.Name)
                {
                    if (GetPage(page.Name) == null)
                    {
                        pageModify.Name = page.Name;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(page.Route) && page.Route != pageModify.Route)
                {
                    pageModify.Route = page.Route;
                }
            }
            return false;
        }
    }
}
