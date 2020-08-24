// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la gestión de plantillas
using GestorDocumentacion.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Clase para la gestión de plantillas
    /// </summary>
    public class TemplatesOperationsService : ITemplatesOperationsServices
    {
        private readonly EntityContext _context;
        public TemplatesOperationsService(EntityContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Elimina una plantilla
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla a eliminar</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool DeleteTemplate(Guid templateID)
        {
            Template template = _context.Template.FirstOrDefault(template => template.TemplateID.Equals(templateID));
            if(template != null)
            {
                _context.Entry(template).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            return true;
        }
        /// <summary>
        /// Obtiene una plantilla por su nombre
        /// </summary>
        /// <param name="name">Nombre de la plantilla a obtener</param>
        /// <returns>Un objeto página</returns>
        public Template GetTemplate(string name)
        {
            return _context.Template.FirstOrDefault(template => template.Name.Equals(name));
        }
        /// <summary>
        /// Obtiene una plantilla por su nombre
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla a obtener</param>
        /// <returns>Un objeto página</returns>
        public Template GetTemplate(Guid templateID)
        {
           return _context.Template.FirstOrDefault(template => template.TemplateID.Equals(templateID));
        }
        /// <summary>
        /// Obtiene una lista de plantillas
        /// </summary>
        /// <returns>Lista de objetos plantilla</returns>
        public List<Template> GetTemplates()
        {
            return _context.Template.ToList();
        }
        /// <summary>
        /// Carga o modifica una plantilla
        /// </summary>
        /// <param name="template">plantilla nueva o a modificar</param>
        /// <param name="isNew">Si el documento es nuevo</param>
        /// <returns>Si se ha realizado con exito</returns>
        public bool LoadTemplate(Template template, bool isNew)
        {
            if (isNew)
            {
                if (template != null && !string.IsNullOrEmpty(template.Content) && !string.IsNullOrEmpty(template.Name) && GetTemplate(template.Name) == null)
                {
                    template.TemplateID = Guid.NewGuid();
                    _context.Template.Add(template);
                    _context.SaveChanges();
                    return true;
                }
            }
            else
            {
                var templateModify = GetTemplate(template.TemplateID);
                if (!string.IsNullOrEmpty(template.Content) && template.Content != templateModify.Content)
                {
                    templateModify.Content = template.Content;
                }
                if (!string.IsNullOrEmpty(template.Name) && template.Name != templateModify.Name)
                {
                    if (GetTemplate(template.Name) == null)
                    {
                        templateModify.Name = template.Name;
                    }
                    else
                    {
                        return false;
                    }
                }
                _context.SaveChanges();
            }
            return false;
        }
    }
}
