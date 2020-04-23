using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CronConfigure.Models;
using CronConfigure.Models.Enumeracion;
using CronConfigure.Models.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CronConfigure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        public CronApiService _cronApiService;
        public JobController(CronApiService cronApiService)
        {
            _cronApiService = cronApiService;
        }

        [HttpPost]
        public IActionResult AddExecution(string nombre_fichero, string nombre_job, string fecha_inicio)
        {
            var fechaInicio = DateTime.Parse(fecha_inicio);
            string id = BackgroundJob.Schedule(() => ExampleService.WriteLine(nombre_fichero), fechaInicio);

            return Ok(id);
        }

        [HttpPost("{id}")]
        public IActionResult AddExecution(string id)
        {
            if (_cronApiService.GetJobs(JobType.All).Any(item => item.Id.Equals(id)))
            {
                BackgroundJob.Requeue(id);
                return Ok(id);
            }
            else
            {
                return BadRequest("el job no se encuentra en la lista de ejecutados");
            }
        }

        [HttpGet]
        public IActionResult GetJobs(JobType type)
        {
            return Ok(_cronApiService.GetJobs(type));

        }
    }
}