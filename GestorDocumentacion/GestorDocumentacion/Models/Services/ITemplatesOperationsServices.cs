// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz  para la gestión de plantillas
using GestorDocumentacion.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Interfaz  para la gestión de plantillas
    /// </summary>
    public interface ITemplatesOperationsServices
    {
        /// <summary>
        /// Obtiene una plantilla por su nombre
        /// </summary>
        /// <param name="name">Nombre de la plantilla a obtener</param>
        /// <returns>Un objeto página</returns>
        public Template GetTemplate(string name);
        /// <summary>
        /// Obtiene una plantilla por su nombre
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla a obtener</param>
        /// <returns>Un objeto página</returns>
        public Template GetTemplate(Guid templateID);
        /// <summary>
        /// Carga o modifica una plantilla
        /// </summary>
        /// <param name="template">plantilla nueva o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadTemplate(Template template, bool isNew);
        /// <summary>
        /// Obtiene una lista de plantillas
        /// </summary>
        /// <returns>Lista de objetos plantilla</returns>
        public List<Template> GetTemplates();
        /// <summary>
        /// Elimina una plantilla
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla a eliminar</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeleteTemplate(Guid templateID);
    }
}
