// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la gestión de páginas
using GestorDocumentacion.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Clase para la gestión de páginas
    /// </summary>
    public class PagesOperationService : IPagesOperationsServices
    {
        private readonly EntityContext _context;
        public PagesOperationService(EntityContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Elimina una página
        /// </summary>
        /// <param name="pageID">Identificador de la página a eliminar</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeletePage(Guid pageID)
        {
            Page page = _context.Page.FirstOrDefault(page => page.PageID.Equals(pageID));
            if (page != null)
            {
                _context.Entry(page).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            return true;
        }
        /// <summary>
        /// Obtiene una página por su nombre
        /// </summary>
        /// <param name="route">Ruta de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(string route)
        {
            Page page = _context.Page.FirstOrDefault(page => page.Route.Equals(route));
            if (page != null)
            {
                page.LastRequested = DateTime.Now;
                _context.SaveChanges();
            }
            
            return page;
        }
        /// <summary>
        /// Obtiene una página por su identificador
        /// </summary>
        /// <param name="pageID">Identificador de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(Guid pageID)
        {

            return _context.Page.FirstOrDefault(page => page.PageID.Equals(pageID));
        }
        /// <summary>
        /// Obtiene una lista de páginas
        /// </summary>
        /// <returns>Lista de objetos página</returns>
        public List<Page> GetPages()
        {
            var consulta = _context.Page.Where(item => item.Route.Contains("Public"));
            return _context.Page.ToList();
        }
        /// <summary>
        /// Carga o modifica una página
        /// </summary>
        /// <param name="page">página nueva o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadPage(Page page, bool isNew)
        {
            StringBuilder layout = new StringBuilder();
            layout.AppendLine("@{");
            layout.AppendLine("Layout = \"_Layout\";");
            layout.AppendLine("}");
            if (isNew)
            {
                if (page != null && !string.IsNullOrEmpty(page.Content) && !string.IsNullOrEmpty(page.Route) && GetPage(page.Route) == null)
                {
                    //if (!page.Content.Contains("\"_Layout\""))
                    //{
                    //    page.Content = $"{layout.ToString()}{page.Content}";
                    //}
                    page.LastModified = DateTime.Now;


                    _context.Page.Add(page);
                    _context.SaveChanges();
                    return true;
                }
            }
            else
            {
                var pageModify = GetPage(page.PageID);
                if(!string.IsNullOrEmpty(page.Content) && page.Content != pageModify.Content)
                {
                    //if (!page.Content.Contains("\"_Layout\""))
                    //{
                    //    page.Content = $"{layout.ToString()}{page.Content}";
                    //}
                    pageModify.Content = page.Content;
                    pageModify.LastModified = DateTime.Now;
                }
                if (!string.IsNullOrEmpty(page.Route) && page.Route != pageModify.Route)
                {
                    if (GetPage(page.Route) == null)
                    {
                        pageModify.Route = page.Route;
                        pageModify.LastModified = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                }
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
