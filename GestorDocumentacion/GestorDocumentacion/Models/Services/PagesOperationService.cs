using GestorDocumentacion.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="name">Nombre de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(string name)
        {
            return _context.Page.FirstOrDefault(page => page.Name.Equals(name));
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
            if (isNew)
            {
                if (page != null && !string.IsNullOrEmpty(page.Content) && !string.IsNullOrEmpty(page.Name) && GetPage(page.Name) == null)
                {
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
                    pageModify.Content = page.Content;
                }
                if (!string.IsNullOrEmpty(page.Name) && page.Content != pageModify.Name)
                {
                    if (GetPage(page.Name) != null)
                    {
                        pageModify.Name = page.Name;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(page.Route) && page.Content != pageModify.Route)
                {
                    pageModify.Route = page.Route;
                }
                _context.SaveChanges();
            }
            return false;
        }
    }
}
