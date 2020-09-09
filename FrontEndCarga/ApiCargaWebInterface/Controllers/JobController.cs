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
        /// <summary>
        /// Devuelve una página principal con una lista de tareas vacía
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<CreateRecurringJobViewModel> lista = new List<CreateRecurringJobViewModel>();
            return View(lista);
        }
        /// <summary>
        /// Devuelve la página de creación de una tarea con el idetificador del repositorio asociado
        /// </summary>
        /// <param name="IdRepository">Identificador del repositorio</param>
        /// <returns></returns>
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

        /// <summary>
        /// Obtiene los detalles de una tarea
        /// </summary>
        /// <param name="id">Identificador de una tarea</param>
        /// <returns></returns>
        [HttpGet("[Controller]/{id}")]
        public IActionResult DetailsJob(string id)
        {
           var job = _serviceApi.GetJob(id);
            return View(job);
        }

        /// <summary>
        /// Obtiene los detalles de una tarea recurrente
        /// </summary>
        /// <param name="name">Nombre de la tarea recurrente</param>
        /// <returns></returns>
        [HttpGet("[Controller]/recurring/{name}")]
        public IActionResult DetailsRecurringJob(string name)
        {
            var recurringJob = _serviceApi.GetRecurringJob(name);
            RecurringJobWebViewModel recurringJobViewModel = new RecurringJobWebViewModel();
            recurringJobViewModel.RecurringJobViewModel = recurringJob;
            recurringJobViewModel.ListJobs = _serviceApi.GetJobsOfRecurringJob(name);
            return View(recurringJobViewModel);
        }

        /// <summary>
        /// Crea una tarea nueva
        /// </summary>
        /// <param name="jobModel">Detalles de la tarea a crear</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(CreateJobViewModel jobModel)
        {
            
            if (jobModel.IdRepository.Equals(Guid.Empty))
            {
                ModelState.AddModelError("IdRepository", "id del repositorio no válido");
            }
            if ((jobModel.Nombre_job != null && jobModel.CronExpression== null) || (jobModel.CronExpression != null && jobModel.Nombre_job == null))
            {
                ModelState.AddModelError("Nombre_job", "faltan datos para crear un job recurrente");
            }
            else if (!string.IsNullOrEmpty(jobModel.Nombre_job) && !string.IsNullOrEmpty(jobModel.CronExpression))
            {
                var correct = CrontabSchedule.TryParse(jobModel.CronExpression);
                if (correct == null)
                {
                    ModelState.AddModelError("CronExpression", "expresión del cron inválida");
                }
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
                    return RedirectToAction("Details", "RepositoryConfig", new { id = jobModel.IdRepository });
                }
                else
                {
                    string id = _serviceApi.CreateJob(jobModel);
                    resultCreated item = new resultCreated()
                    {
                        Id = id
                    };
                    return RedirectToAction("Details", "RepositoryConfig", new { id = jobModel.IdRepository });
                }
            }
            
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="id">identificador de la tarea</param>
        /// <param name="job">tipo de tarea (programada: scheduled o recurrente: recurring)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Crea una tarea de sincronización para un repositorio
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio</param>
        /// <returns></returns>
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

        /// <summary>
        /// Vuelve a encolar una tarea
        /// </summary>
        /// <param name="idJob">Identificador de la tarea</param>
        /// <returns></returns>
        public IActionResult ReQueue(string idJob)
        {
            _serviceApi.ReQueueJob(idJob);
            var job = _serviceApi.GetJob(idJob);
            return View("DetailsJob", job);
        }
        /// <summary>
        /// Comprueba que una expresión cron es válida
        /// </summary>
        /// <param name="CronExpression">Expresión a comprobar</param>
        /// <returns></returns>
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