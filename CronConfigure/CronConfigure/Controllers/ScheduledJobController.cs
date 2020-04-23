using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CronConfigure.Models;
using CronConfigure.Models.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CronConfigure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduledJobController : ControllerBase
    {
        public CronApiService _cronApiService;
        public ScheduledJobController(CronApiService cronApiService)
        {
            _cronApiService = cronApiService;
        }
            [HttpGet]
        public IActionResult GetScheduledJobs()
        {
            return Ok(_cronApiService.GetScheduledJobs());
        }

        [HttpPost]
        public IActionResult AddScheduledJob(string fecha_ejecucion, string nombre_fichero)
        {
            var fechaInicio = DateTime.Parse(fecha_ejecucion);
            BackgroundJob.Schedule(() => ExampleService.WriteLine(nombre_fichero), fechaInicio);
            return Ok();
        }

        [HttpPut]
        public IActionResult EnqueuedScheduledJob(string id)
        {
            if(_cronApiService.GetScheduledJobs().Any(item => item.Key.Equals(id)))
            {
                _cronApiService.EnqueueJob(id);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public IActionResult DeleteScheduledJob(string id)
        {
            if (_cronApiService.GetScheduledJobs().Any(item => item.Key.Equals(id)))
            {
                _cronApiService.DeleteJob(id);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}