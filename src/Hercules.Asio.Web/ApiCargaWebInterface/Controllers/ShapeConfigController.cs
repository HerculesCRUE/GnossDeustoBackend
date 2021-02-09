// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador Shapes
using System;
using System.Collections.Generic;
using System.Text;
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador de los shapes (configuración de validación)
    /// </summary>
    public class ShapeConfigController : Controller
    {
        readonly ICallShapeConfigService _serviceApi;
        readonly ICallRepositoryConfigService _repositoryServiceApi;
        public ShapeConfigController(ICallShapeConfigService serviceApi, ICallRepositoryConfigService repositoryServiceApi)
        {
            _serviceApi = serviceApi;
            _repositoryServiceApi = repositoryServiceApi;
        }
        /// <summary>
        /// Devueleve una lista con todas las configuraciones de validación
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<ShapeConfigViewModel> result = _serviceApi.GetShapeConfigs();
            if (result == null)
            {
                result = new List<ShapeConfigViewModel>();
            }
            return View(result);
        }

        /// <summary>
        /// Obtiene una configuración de validación
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("[Controller]/{id}")]
        public IActionResult Details(Guid id)
        {
            ShapeConfigViewModel result = _serviceApi.GetShapeConfig(id);
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtiene el archivo de configuración asociado al shape
        /// </summary>
        /// <param name="id">Identificador del shape</param>
        /// <returns></returns>
        [HttpGet("[Controller]/download/{id}")]
        public IActionResult DownloadShape(Guid id)
        {
            ShapeConfigViewModel result = _serviceApi.GetShapeConfig(id);
            if (result != null)
            {
                var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(result.Shape));
                var contentType = "APPLICATION/octet-stream";
                var fileName = $"{id}_shape.txt";
                return File(content, contentType, fileName);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtiene la página de edición con los datos actuales de la configuración
        /// </summary>
        /// <param name="id">Identificador de la configuración</param>
        /// <returns></returns>
        public IActionResult Edit(Guid id)
        {
            ShapeConfigViewModel result = _serviceApi.GetShapeConfig(id);
            
            if (result != null)
            {
                ShapeConfigEditModel shapeConfigViewModel = new ShapeConfigEditModel()
                {
                    Name = result.Name,
                    RepositoryID = result.RepositoryID,
                    Shape = result.Shape,
                    ShapeConfigID = result.ShapeConfigID
                };

                return View(shapeConfigViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Edita una configuración de validación
        /// </summary>
        /// <param name="shapeConfigViewModel">datos nuevos a modificar</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(ShapeConfigEditModel shapeConfigViewModel)
        {
            try
            {
                _serviceApi.ModifyShapeConfig(shapeConfigViewModel);

                return RedirectToAction("Details", new { id = shapeConfigViewModel.ShapeConfigID });
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Elimina una configuración de validación
        /// </summary>
        /// <param name="id">Identificador de la configuración de validación</param>
        /// <returns></returns>
        public IActionResult Delete(Guid id)
        {
            bool result = _serviceApi.DeleteShapeConfig(id);
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Devuelve la página de creación de una configuración de validación
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio OAIPMH al que se va asociar la configuración</param>
        /// <returns></returns>
        public IActionResult Create(Guid? repositoryId = null)
        {
            if (repositoryId.HasValue)
            {
                ShapeConfigCreateModel shapeConfigViewModel = new ShapeConfigCreateModel()
                {
                    RepositoryID = repositoryId.Value
                };
                return View(shapeConfigViewModel);
            }
            return View();
        }
        /// <summary>
        /// Crea una configuración de validación
        /// </summary>
        /// <param name="shapeConfigViewModel">Información de la nueva configuración de validación</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(ShapeConfigCreateModel shapeConfigViewModel)/*Guid ShapeConfigID,string Name, Guid RepositoryID, IFormFile ShapeFile)*/ 
        { 
            try
            {
                var repository = _repositoryServiceApi.GetRepositoryConfig(shapeConfigViewModel.RepositoryID);
                if(repository == null)
                {
                    ModelState.AddModelError("RepositoryID", "No existe el repositorio");
                    return View("Create", shapeConfigViewModel);
                }
                else
                {
                    ShapeConfigViewModel result = _serviceApi.CreateShapeConfig(shapeConfigViewModel);
                    return RedirectToAction("Details", new { id = result.ShapeConfigID });
                }
               

            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
        }
    }
}