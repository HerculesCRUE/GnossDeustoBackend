﻿using GestorDocumentacion.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Interfaz para la gestión de paginas
    /// </summary>
    public interface IPagesOperationsServices
    {
        /// <summary>
        /// Obtiene una página por su nombre
        /// </summary>
        /// <param name="name">Nombre de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(string name);
        /// <summary>
        /// Obtiene una página por su identificador
        /// </summary>
        /// <param name="pageID">Identificador de la página a obtener</param>
        /// <returns>Un objeto página</returns>
        public Page GetPage(Guid pageID);
        /// <summary>
        /// Carga o modifica una página
        /// </summary>
        /// <param name="page">página nueva o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadPage(Page page, bool isNew);
        /// <summary>
        /// Obtiene una lista de páginas
        /// </summary>
        /// <returns>Lista de objetos página</returns>
        public List<Page> GetPages();
        /// <summary>
        /// Elimina una página
        /// </summary>
        /// <param name="pageID">Identificador de la página a eliminar</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeletePage(Guid pageID);
    }
}
