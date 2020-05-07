using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace ApiCargaWebInterface.Controllers
{
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateJobViewModel jobModel)
        {
            var correct = CrontabSchedule.TryParse(jobModel.CronExpression);
            if (correct != null)
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
                    return Ok(jobModel.Nombre_job);
                }
                else
                {
                    return Ok(_serviceApi.CreateJob(jobModel));
                }
            }
            
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