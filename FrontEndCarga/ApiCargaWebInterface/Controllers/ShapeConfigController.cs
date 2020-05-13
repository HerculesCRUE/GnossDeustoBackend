using System;
using System.Collections.Generic;
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    public class ShapeConfigController : Controller
    {
        readonly ICallShapeConfigService _serviceApi;
        public ShapeConfigController(ICallShapeConfigService serviceApi)
        {
            _serviceApi = serviceApi;
        }

        public IActionResult Index()
        {
            List<ShapeConfigViewModel> result = _serviceApi.GetShapeConfigs();
            return View(result);
        }

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

        public IActionResult Edit(Guid id)
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

        [HttpPost]
        public IActionResult Edit(ShapeConfigViewModel shapeConfigViewModel)
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

        public IActionResult Create(Guid? repositoryId = null)
        {
            if (repositoryId != null )
            {
                ShapeConfigCreateModel shapeConfigViewModel = new ShapeConfigCreateModel()
                {
                    RepositoryID = repositoryId.Value
                };
                return View(shapeConfigViewModel);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(ShapeConfigViewModel shapeConfigViewModel)
        {
            try
            {
                ShapeConfigViewModel result = _serviceApi.CreateShapeConfig(shapeConfigViewModel);
                return RedirectToAction("Details", new { id = result.ShapeConfigID });

            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
        }
    }
}