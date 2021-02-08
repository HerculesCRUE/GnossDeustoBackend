// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la simulación de la gestión de plantillas
using GestorDocumentacion.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Services
{
    /// <summary>
    /// Clase para la simulación de la gestión de plantillas
    /// </summary>
    public class TemplatesOperationMockService : ITemplatesOperationsServices
    {
        private List<Template> _pageTemplate;
        public TemplatesOperationMockService()
        {
            _pageTemplate = new List<Template>();
            Template template1 = new Template()
            {
                TemplateID = Guid.NewGuid(),
                Name = "Template1",
                Content = "<html><head></head><body><p>hola mundo</p></body></html>"
            };
            Template Template2 = new Template()
            {
                TemplateID = Guid.NewGuid(),
                Name = "Template2",
                Content = "<html><head></head><body><p>hola mundo</p></body></html>"
            };
            _pageTemplate.Add(template1);
            _pageTemplate.Add(Template2);
        }
        /// <summary>
        /// Elimina una plantilla
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla</param>
        /// <returns></returns>
        public bool DeleteTemplate(Guid templateID)
        {
            Template template = _pageTemplate.FirstOrDefault(page => page.TemplateID.Equals(templateID));
            if (template != null)
            {
                _pageTemplate.Remove(template);
            }
            return true;
        }
        /// <summary>
        /// Obtiene una plantilla a partir de su nombre
        /// </summary>
        /// <param name="name">Nombre de la plantila</param>
        /// <returns></returns>
        public Template GetTemplate(string name)
        {
            return _pageTemplate.FirstOrDefault(page => page.Name.Equals(name));
        }
        /// <summary>
        /// Obtiene una plantilla a partir de su identificador
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla</param>
        /// <returns></returns>
        public Template GetTemplate(Guid templateID)
        {
            return _pageTemplate.FirstOrDefault(page => page.TemplateID.Equals(templateID));
        }
        /// <summary>
        /// Obtiene una lista de plantillas
        /// </summary>
        /// <returns></returns>
        public List<Template> GetTemplates()
        {
            return _pageTemplate;
        }
        /// <summary>
        /// Carga una nueva plantilla o modifica una existente
        /// </summary>
        /// <param name="template">Datos nuevos de la plantilla</param>
        /// <param name="isNew">Indica si la plantilla es nueva o no</param>
        /// <returns></returns>
        public bool LoadTemplate(Template template, bool isNew)
        {
            if (isNew)
            {
                if (template != null && !string.IsNullOrEmpty(template.Content) && !string.IsNullOrEmpty(template.Name) && GetTemplate(template.Name) == null)
                {
                    _pageTemplate.Add(template);
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
            }
            return false;
        }
    }
}
