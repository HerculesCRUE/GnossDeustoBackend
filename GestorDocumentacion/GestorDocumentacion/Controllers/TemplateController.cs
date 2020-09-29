// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
//Controlador para la gestión de plantillas
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

namespace GestorDocumentacion.Controllers
{
    ///<summary>
    ///Controlador para la gestión de plantillas
    ///</summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TemplateController : ControllerBase
    {
        private ITemplatesOperationsServices _templatesOperationsService;
        private IFileOperationService _fileOperationsService;
        public TemplateController(ITemplatesOperationsServices templatesOperationsService, IFileOperationService fileOperationsService)
        {
            _templatesOperationsService = templatesOperationsService;
            _fileOperationsService = fileOperationsService;
        }
        ///<summary>
        ///Devuelve una plantilla HTML, incluyendo sus metadatos.
        ///</summary>
        ///<param name="templateId">identificador de la plantilla html</param>
        [HttpGet("{name}")]
        public IActionResult GetTemplate(Guid templateId)
        {
            var template = _templatesOperationsService.GetTemplate(templateId);
            if (template != null)
            {
                return Ok(template.Content);
            }
            return Ok("Not found");
        }

        ///<summary>
        ///Devuelve una lista de las plantillas cargadas.
        ///</summary>
        [HttpGet]
        [Route("list")]
        public IActionResult GetTemplates()
        {
            var templates = _templatesOperationsService.GetTemplates();
            List<TemplateViewModel> templateViewModelList = new List<TemplateViewModel>();
            foreach (var template in templates)
            {
                TemplateViewModel templateModel = new TemplateViewModel()
                {
                    Name = template.Name,
                    TemplateID = template.TemplateID,
                };
                templateViewModelList.Add(templateModel);
            }
            return Ok(templateViewModelList);
        }

        /// <summary>
        /// Carga o modifica una plantilla web e incluye información acerca de la plantilla, como metadatos title o description.
        /// </summary>
        /// <param name="name">Nombre nuevo de la página </param>
        /// <param name="templateId">Identificador de la plantilla a modificar, en el caso de que se quiera añadir una nueva hay que dejar este campo vacío</param>
        /// <param name="html_template">Contenido html de la plantilla</param>
        /// <returns></returns>
        [HttpPost]
        [Route("load")]
        public IActionResult LoadTemplate(string name, Guid templateId, IFormFile html_template)
        {
            Guid guidTemplate = Guid.Empty;
            bool isNew = false;
            if (Guid.Empty.Equals(templateId))
            {
                guidTemplate = Guid.NewGuid();
                isNew = true;
            }
            else
            {
                guidTemplate = templateId;
            }

            Template template = new Template()
            {
                Name = name,
                TemplateID = guidTemplate,
                Content = _fileOperationsService.ReadFile(html_template)
            };
            if (!_templatesOperationsService.LoadTemplate(template, isNew))
            {
                return BadRequest($"The template with name {name} already exist");
            }
            return Ok(guidTemplate);
        }

        /// <summary>
        /// Elimina una plantilla
        /// </summary>
        /// <param name="templateID">Identificador de la plantilla a eliminar</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        public IActionResult DeleteTemplate(Guid templateID)
        {
            return Ok(_templatesOperationsService.DeleteTemplate(templateID));
        }
    }
}