using System;
using System.Collections.Generic;
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace ApiCargaWebInterface.Controllers
{
    public class RepositoryConfigController : Controller
    {
        readonly ICallRepositoryConfigService _serviceApi;
        readonly CallRepositoryJobService _respositoryJobService;
        public RepositoryConfigController(ICallRepositoryConfigService serviceApi, CallRepositoryJobService respositoryJobService)
        {
            _serviceApi = serviceApi;
            _respositoryJobService = respositoryJobService;
        }
        public IActionResult Index()
        {
            List<RepositoryConfigViewModel> result = _serviceApi.GetRepositoryConfigs();
            return View(result);
        }

        [Route("[Controller]/{id}")]
        public IActionResult Details(Guid id)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(id);
            result.ListRecurringJobs = _respositoryJobService.GetRecurringJobsOfRepo(id);
            result.ListJobs = _respositoryJobService.GetJobsOfRepo(id);
            result.ListScheduledJobs = _respositoryJobService.GetScheduledJobsOfRepo(id);
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
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(id);
            if (result != null)
            {
                return View("Create", result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Edit(RepositoryConfigViewModel repositoryConfigView)
        {
            try
            {
                _serviceApi.ModifyRepositoryConfig(repositoryConfigView);

                return RedirectToAction("Details",new { id = repositoryConfigView.RepositoryConfigID });
            }
            catch(BadRequestException)
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
        public IActionResult Create(RepositoryConfigViewModel repositoryConfigView)
        {
            try
            {
                repositoryConfigView.RepositoryConfigID = Guid.NewGuid();
                RepositoryConfigViewModel result = _serviceApi.CreateRepositoryConfigView(repositoryConfigView);
                return RedirectToAction("Details", new { id = result.RepositoryConfigID });
                
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
        }
    }
}