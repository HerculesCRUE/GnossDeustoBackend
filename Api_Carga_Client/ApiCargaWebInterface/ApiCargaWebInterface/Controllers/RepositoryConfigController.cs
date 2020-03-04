using System;
using System.Collections.Generic;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    public class RepositoryConfigController : Controller
    {
        readonly ICallRepositoryConfigService _serviceApi;
        public RepositoryConfigController(ICallRepositoryConfigService serviceApi)
        {
            _serviceApi = serviceApi;
        }
        public IActionResult Index()
        {
            List<RepositoryConfigView> result = _serviceApi.GetRepositoryConfigs();
            return View(result);
        }

        [Route("[Controller]/{id}")]
        public IActionResult Details(Guid id)
        {
            RepositoryConfigView result = _serviceApi.GetRepositoryConfig(id);
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
            RepositoryConfigView result = _serviceApi.GetRepositoryConfig(id);
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
        public IActionResult Edit(RepositoryConfigView repositoryConfigView)
        {
            try
            {
                _serviceApi.ModifyRepositoryConfig(repositoryConfigView);

                return RedirectToAction("Details",new { id = repositoryConfigView.RepositoryConfigID });
            }
            catch(BadResquestException)
            {
                return BadRequest();
            }
            
        }

        public IActionResult Delete(Guid id)
        {
            bool result = _serviceApi.DeleteRepositoryConfig(id);
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RepositoryConfigView repositoryConfigView)
        {
            try
            {
                RepositoryConfigView result = _serviceApi.CreateRepositoryConfigView(repositoryConfigView);
                return RedirectToAction("Details", new { id = result.RepositoryConfigID });
                
            }
            catch (BadResquestException)
            {
                return BadRequest();
            }
        }
    }
}