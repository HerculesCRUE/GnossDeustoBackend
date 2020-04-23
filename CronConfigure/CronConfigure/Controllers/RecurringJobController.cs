using System;
using System.Collections.Generic;
using CronConfigure.Models;
using CronConfigure.Models.Enumeracion;
using CronConfigure.Models.Services;
using CronConfigure.ViewModels;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace CronConfigure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringJobController : ControllerBase
    {

        public CronApiService _cronApiService;

        public RecurringJobController(CronApiService cronApiService)
        {
            _cronApiService = cronApiService;
        }

        [HttpPost]
        public IActionResult AddExecution(string nombre_fichero, string nombre_job, string fecha_inicio, string cron_expression, bool execute_inmediatly)
        {
            var fechaInicio = DateTime.Parse(fecha_inicio);
            bool validCronExpr = true;
            if (_cronApiService.GetRecurringJobs(nombre_job)!=null)
            {
                return BadRequest("Ya existe un job con ese nombre");
            }
            else
            {
                var correct = CrontabSchedule.TryParse(cron_expression);
                if (correct != null)
                {
                    BackgroundJob.Schedule(() => ExampleService.PonerEnCola(nombre_job, nombre_fichero, cron_expression, execute_inmediatly), fechaInicio);
                }
                else
                {
                    validCronExpr = false;
                }
            }
            if (validCronExpr)
            {
                return Ok();
            }
            else
            {
                return BadRequest("invalid cron expression");
            }
        }

        [HttpDelete]
        public IActionResult DeleteRecurringJob(string nombre_job)
        {
            RecurringJob.RemoveIfExists(nombre_job);
            BackgroundJob.Delete(nombre_job);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetRecurringJob()
        {
            return Ok(_cronApiService.GetRecurringJobs());
        }

        [HttpGet("{id}")]
        public IActionResult GetRecurringJob(string id)
        {
            return Ok(_cronApiService.GetRecurringJobs(id));
        }



    }
}