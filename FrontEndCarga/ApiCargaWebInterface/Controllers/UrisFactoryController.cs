// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador encargado de gestionar las operaciones de la factoria de uris
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones de la factoria de uris
    /// </summary>
    //[Authorize]
    public class UrisFactoryController : Controller
    {
        ICallUrisFactoryApiService _callUrisFactoryService;
        public UrisFactoryController(ICallUrisFactoryApiService callUrisFactoryService)
        {
            _callUrisFactoryService = callUrisFactoryService;
        }
        /// <summary>
        /// Devuelve la página principal
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Devuelve la página de los esquemas
        /// </summary>
        /// <returns></returns>
        [Route("[Controller]/SchemaConfig")]
        public IActionResult Schema()
        {
            return View();
        }
        /// <summary>
        /// Obtiene una uri
        /// </summary>
        /// <param name="resourceClass">Resource class o rdfType</param>
        /// <param name="identifier">Identificador</param>
        /// <param name="uriGetEnum">Configurador para indicar si el parametro pasado en resourceClass es un resource class o rdf type</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUri(string Resource_class, string Identifier, UriGetEnum uriGetEnum)
        {
            UrisFactoryResultViewModel urisFactoryModel = new UrisFactoryResultViewModel();
            urisFactoryModel.Identifier = Identifier;
            urisFactoryModel.Resource_class = Identifier;
            try
            {
                urisFactoryModel.UriResult = _callUrisFactoryService.GetUri(Resource_class, Identifier, uriGetEnum);
            }
            catch(HttpRequestException ex)
            {
                ModelState.AddModelError("Resource_class", ex.Message);
            }
            return View("Index", urisFactoryModel);
        }
        /// <summary>
        /// Obtiene el esquema de uris configurado
        /// </summary>
        /// <returns></returns>
        [HttpGet("[Controller]/Schema")]
        public IActionResult DownloadSchema()
        {
            string result = _callUrisFactoryService.GetSchema();
            if (result != null)
            {
                var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(result));
                var contentType = "APPLICATION/octet-stream";   
                var fileName = $"UrisFactorySchema.json";
                return File(content, contentType, fileName);
            }
            else
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Remplaza el esquema de uris configurado
        /// </summary>
        /// <param name="New_Schema_File">Nuevo esquema de uris</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ReplaceSchema(IFormFile New_Schema_File)
        {
            if (New_Schema_File != null)
            {
                try
                {
                    _callUrisFactoryService.ReplaceSchema(New_Schema_File);
                }
                catch(BadRequestException badExce)
                {
                    UrisFactoryResultViewModel urisFactoryModel = new UrisFactoryResultViewModel();
                    ModelState.AddModelError("New_Schema_File", badExce.Message);
                    return View("Index", urisFactoryModel);
                }
                
            }
            return View("Index", null);
        }

        /// <summary>
        /// Obtiene los detalles de una estructura de uris
        /// </summary>
        /// <param name="Uri_Structure">Nombre de la estructura de uris</param>
        /// <returns></returns>
        [HttpGet("[Controller]/structure")]
        public IActionResult Details(string Uri_Structure)
        {
            try
            {
                string result = _callUrisFactoryService.GetStructure(Uri_Structure);
                WebUriStructureViewModel structureViewModel = new WebUriStructureViewModel();
                structureViewModel.Name = Uri_Structure;
                dynamic result2 = JsonConvert.DeserializeObject(result);
                structureViewModel.Structure = JsonConvert.SerializeObject(result2, Formatting.Indented);
                return View(structureViewModel);
            }
            catch (BadRequestException badExce)
            {
                UrisFactoryResultViewModel urisFactoryModel = new UrisFactoryResultViewModel();
                ModelState.AddModelError("Uri_Structure", badExce.Message);
                return View("Index", urisFactoryModel);
            }
           
        }
        /// <summary>
        /// Devuelve la página de creación de una estructura
        /// </summary>
        /// <returns></returns>
        [HttpGet("[Controller]/create-structure")]
        public IActionResult CreateStructure()
        {
            WebUriStructureViewModel structureViewModel = new WebUriStructureViewModel();
            structureViewModel.Structure = UriStructureViewModel.GetUriStrcuture();
            return View(structureViewModel);
        }
        /// <summary>
        /// Crea una estructura nueva
        /// </summary>
        /// <param name="structureViewModel">Información de la estructura de uris</param>
        /// <returns></returns>
        [HttpPost("[Controller]/create-structure")]
        public IActionResult CreateStructure(WebUriStructureViewModel structureViewModel)
        {
            _callUrisFactoryService.AddStructure(structureViewModel.Structure);
            return View(structureViewModel);
        }
        /// <summary>
        /// Elimina una estructura de uris
        /// </summary>
        /// <param name="name">Nombre de la estructura a eliminar</param>
        /// <returns></returns>
        public IActionResult Delete(string name)
        {
            try
            {
                _callUrisFactoryService.DeleteStructure(name);
                return View("Index", null);
            }
            catch (BadRequestException badExce)
            {
                UrisFactoryResultViewModel urisFactoryModel = new UrisFactoryResultViewModel();
                ModelState.AddModelError("Uri_Structure", badExce.Message);
                return View("Index", urisFactoryModel);
            }
        }

    }
}