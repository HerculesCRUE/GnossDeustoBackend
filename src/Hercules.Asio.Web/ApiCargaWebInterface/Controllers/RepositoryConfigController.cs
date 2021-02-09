// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador repositorios OAIPMH
using System;
using System.Collections.Generic;
using System.Linq;
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador repositorios OAIPMH
    /// </summary>
    public class RepositoryConfigController : Controller
    {
        readonly ICallRepositoryConfigService _serviceApi;
        readonly CallRepositoryJobService _respositoryJobService;
        readonly ProcessDiscoverStateJobBDService _processDiscoverStateJobBDService;
        public RepositoryConfigController(ProcessDiscoverStateJobBDService iProcessDiscoverStateJobBDService, ICallRepositoryConfigService serviceApi, CallRepositoryJobService respositoryJobService)
        {
            _processDiscoverStateJobBDService = iProcessDiscoverStateJobBDService;
            _serviceApi = serviceApi;
            _respositoryJobService = respositoryJobService;
        }
        /// <summary>
        /// Obtiene una lista de todos los repositorio OAIPMH
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<RepositoryConfigViewModel> result = _serviceApi.GetRepositoryConfigs();
            if(result == null)
            {
                result = new List<RepositoryConfigViewModel>();
            }
            return View(result);
        }
        /// <summary>
        /// Obtiene los detalles asociados a un repositorio OAIPMH
        /// </summary>
        /// <param name="id">Identificador del repositorio</param>
        /// <returns></returns>
        [Route("[Controller]/{id}")]
        public IActionResult Details(Guid id)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(id);
            result.ListRecurringJobs = _respositoryJobService.GetRecurringJobsOfRepo(id);
            result.ListJobs = _respositoryJobService.GetJobsOfRepo(id);
            result.ListScheduledJobs = _respositoryJobService.GetScheduledJobsOfRepo(id);
            if (result.ListJobs != null && result.ListJobs.Count > 0)
            {
                var job = result.ListJobs.OrderByDescending(item => item.ExecutedAt).FirstOrDefault();
                result.LastJob = job.Id;
                result.LastState = job.State;
                int succed = result.ListJobs.Count(item => item.State.Equals("Succeeded"));
                double percentage = ((double)succed / result.ListJobs.Count)*100;
                result.PorcentajeTareas = Math.Round(percentage, 2);

                List<ProcessDiscoverStateJob> statesDiscoverJob = _processDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJobs(result.ListJobs.Select(x => x.Id).ToList());
                foreach(JobViewModel jobVM in result.ListJobs)
                {
                    ProcessDiscoverStateJob state = statesDiscoverJob.FirstOrDefault(x => x.JobId == jobVM.Id);
                    if(state!=null)
                    {
                        jobVM.DiscoverState = state.State;
                    }
                    jobVM.IdRepository = id;
                }
            }
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
        /// Obtiene los los shapes del repositorio
        /// </summary>
        /// <param name="id">Identificador del repositorio</param>
        /// <returns></returns>
        [Route("[Controller]/shapes/{id}")]
        public IActionResult RepositoryShapes(Guid id)
        {
            RepositoryConfigViewModel result = _serviceApi.GetRepositoryConfig(id);
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
        /// Devuleve la página de edición con los datos actuales del repositorio OAIPMH
        /// </summary>
        /// <param name="id">Identificador del repositorio</param>
        /// <returns></returns>
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

        /// <summary>
        /// Edita la información de un repositorio OAIPMH
        /// </summary>
        /// <param name="repositoryConfigView">Información modificada del repositorio OAIPMH</param>
        /// <returns></returns>
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
        /// <summary>
        /// Elimina un repositorio OAIPMH
        /// </summary>
        /// <param name="id">Identificador del repositorio</param>
        /// <returns></returns>
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

        /// <summary>
        /// Devuelve la página de creación de un repositorio OAIPMH
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Crea un nuevo repositorio OAIPMH
        /// </summary>
        /// <param name="repositoryConfigView">Información del nuevo repositorio OAIPMH</param>
        /// <returns>La página de detalle del nuevo repositorio OAIPMH</returns>
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