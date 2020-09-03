// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador tareas
using System;
using System.Collections.Generic;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para gestionar las llamadas relacionandas con el Api cron
    /// </summary>
    public class JobController : Controller
    {
        readonly CallCronApiService _serviceApi;
        public JobController(CallCronApiService serviceApi)
        {
            _serviceApi = serviceApi;
        }

        public IActionResult Index()
        {
            List<CreateRecurringJobViewModel> lista = new List<CreateRecurringJobViewModel>();
            return View(lista);
        }
        public IActionResult Create(Guid? IdRepository = null)
        {
            if (IdRepository.HasValue)
            {
                CreateJobViewModel createJobViewModel = new CreateJobViewModel()
                {
                    IdRepository = IdRepository.Value
                };
                return View(createJobViewModel);
            }
            else
            {
                return View();
            }
        }

        [HttpGet("[Controller]/{id}")]
        public IActionResult DetailsJob(string id)
        {
           var job = _serviceApi.GetJob(id);
            return View(job);
        }

        [HttpGet("[Controller]/recurring/{name}")]
        public IActionResult DetailsRecurringJob(string name)
        {
            var recurringJob = _serviceApi.GetRecurringJob(name);
            RecurringJobWebViewModel recurringJobViewModel = new RecurringJobWebViewModel();
            recurringJobViewModel.RecurringJobViewModel = recurringJob;
            recurringJobViewModel.ListJobs = _serviceApi.GetJobsOfRecurringJob(name);
            return View(recurringJobViewModel);
        }

        [HttpPost]
        public IActionResult Create(CreateJobViewModel jobModel)
        {
            var correct = CrontabSchedule.TryParse(jobModel.CronExpression);
            if (correct == null)
            {
                ModelState.AddModelError("CronExpression", "expresión del cron inválida");
            }
            if (jobModel.IdRepository.Equals(Guid.Empty))
            {
                ModelState.AddModelError("IdRepository", "id del repositorio no válido");
            }
            if ((jobModel.Nombre_job != null && jobModel.CronExpression== null) || ((jobModel.CronExpression != null && jobModel.Nombre_job == null)))
            {
                ModelState.AddModelError("Nombre_job", "faltan datos para crear un job recurrente");
            }
            if (!ModelState.IsValid)
            {
                return View("Create", jobModel);
            }
            else
            {
                if (jobModel.Nombre_job != null)
                {
                    _serviceApi.CreateRecurringJob(jobModel);
                    resultCreated item = new resultCreated()
                    {
                        Id = jobModel.Nombre_job
                    };
                    return View("Created", item);
                }
                else
                {
                    string id = _serviceApi.CreateJob(jobModel);
                    resultCreated item = new resultCreated()
                    {
                        Id = id
                    };
                    return View("Created", item);
                }
            }
            
        }

        public IActionResult Delete(string id, string job)
        {
            if (job.Equals("scheduled"))
            {
                _serviceApi.DeleteScheduledJob(id);
            }
            else if (job.Equals("recurring"))
            {
                _serviceApi.DeleteRecurringJob(id);
            }
            return View("Deleted", id);
        }

        public IActionResult Syncro(Guid repositoryId)
        {
            CreateJobViewModel jobModel = new CreateJobViewModel() { IdRepository = repositoryId };
            string id = _serviceApi.CreateJob(jobModel);
            resultCreated item = new resultCreated()
            {
                Id = id
            };
            return View("Created", item);
        }

        public IActionResult ReQueue(string idJob)
        {
            _serviceApi.ReQueueJob(idJob);
            var job = _serviceApi.GetJob(idJob);
            return View("DetailsJob", job);
        }
        public IActionResult CronValid(string CronExpression)
        {
            var correct = CrontabSchedule.TryParse(CronExpression);
            if (correct != null)
            {
                return Json(true);
            }
            return Json(false);
        }
    }
}