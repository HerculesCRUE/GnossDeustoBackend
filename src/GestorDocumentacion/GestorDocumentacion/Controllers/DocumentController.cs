// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
//Controlador para la gestión de documentos
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorDocumentacion.Models.Entities;
using GestorDocumentacion.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestorDocumentacion.Controllers
{
    ///<summary>
    ///Controlador para la gestión de documentos
    ///</summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private IDocumentsOperationsService _documentsOperationsService;
        
        public DocumentController(IDocumentsOperationsService documentsOperationsService)
        {
            _documentsOperationsService = documentsOperationsService;
           
        }
        ///<summary>
        ///Devuelve el pdf cargado
        ///</summary>
        ///<param name="id">identificador del documento html</param>
        [HttpGet("{id}")]
        public IActionResult GetDocument(Guid id)
        {
            var documentInfo = _documentsOperationsService.GetDocumentInfo(id);
            if (documentInfo != null)
            {
                return File(documentInfo.Values.First(),"application/pdf", documentInfo.Keys.First());
            }
            return Ok("Not found");
        }

        /// <summary>
        /// Devuelve una lista de los documentos guardados.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IActionResult GetDocuments()
        {
            var documents = _documentsOperationsService.GetDocuments();
            return Ok(documents);
        }

        /// <summary>
        /// Carga o modifica un documento
        /// </summary>
        /// <param name="name">Nombre nuevo del documento</param>
        /// <param name="documentId">Identificador del documento a modificar, en el caso de que se quiera añadir una nueva hay que dejar este campo vacio</param>
        /// <param name="pdf">Documento pdf</param>
        /// <returns></returns>
        [HttpPost]
        [Route("load")]
        public IActionResult LoadDocument(string name, Guid documentId, IFormFile pdf)
        {
            Guid guidDocument = Guid.Empty;
            bool isNew = false;
            if (Guid.Empty.Equals(documentId))
            {
                guidDocument = Guid.NewGuid();
                isNew = true;
            }
            else
            {
                guidDocument = documentId;
            }

            Document page = new Document()
            {
                Name = name,
                DocumentId = guidDocument,
            };
            if (!_documentsOperationsService.LoadDocument(page, isNew, pdf))
            {
                return BadRequest($"The document with name {name} already exist");
            }
            return Ok(guidDocument);


        }

        /// <summary>
        /// Elimina una página web.
        /// </summary>
        /// <param name="pageId">Identificador de la página html</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        public IActionResult DeleteDocument(Guid pageId)
        {
            return Ok(_documentsOperationsService.DeleteDocument(pageId));
        }
    }
}