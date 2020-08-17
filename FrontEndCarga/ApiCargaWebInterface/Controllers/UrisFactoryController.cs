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
    //[Authorize]
    public class UrisFactoryController : Controller
    {
        ICallUrisFactoryApiService _callUrisFactoryService;
        public UrisFactoryController(ICallUrisFactoryApiService callUrisFactoryService)
        {
            _callUrisFactoryService = callUrisFactoryService;
        }
        public IActionResult Index()
        {
            return View();
        }
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

        [HttpGet("[Controller]/create-structure")]
        public IActionResult CreateStructure()
        {
            WebUriStructureViewModel structureViewModel = new WebUriStructureViewModel();
            structureViewModel.Structure = UriStructureViewModel.GetUriStrcuture();
            return View(structureViewModel);
        }

        [HttpPost("[Controller]/create-structure")]
        public IActionResult CreateStructure(WebUriStructureViewModel structureViewModel)
        {
            _callUrisFactoryService.AddStructure(structureViewModel.Structure);
            return View(structureViewModel);
        }

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