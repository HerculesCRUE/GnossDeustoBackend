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
    [Route("[controller]")]
    [ApiController]
    public class ScheduledJobController : ControllerBase
    {
        public CronApiService _cronApiService;
        private ProgramingMethodsService _programingMethodsService;
        public ScheduledJobController(CronApiService cronApiService, ProgramingMethodsService programingMethodsService)
        {
            _cronApiService = cronApiService;
            _programingMethodsService = programingMethodsService;
        }

        /// <summary>
        /// Obtiene un listado de tareas programadas 
        /// </summary>
        /// <param name="from">número desde el que tiene que empezar a traer</param>
        /// <param name="count">número de tareas a traer</param>
        /// <returns>listado de tareas programadas</returns> 
        [HttpGet]
        public IActionResult GetScheduledJobs(int from, int count)
        {
            return Ok(_cronApiService.GetScheduledJobs(from, count));
        }

        /// <summary>
        /// Añade una nueva tarea programada de única ejecución para sincornización de repositorios
        /// </summary>
        /// <param name="fecha_ejecucion">fecha en la que se ejecutará la tarea</param>
        /// <param name="id_repository">número de tareas a traer</param>
        /// <returns></returns> 
        [HttpPost]
        public IActionResult AddScheduledJob(string fecha_ejecucion, string id_repository)
        {
            Guid idRep = Guid.Empty;
            try
            {
                idRep = new Guid(id_repository);
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
            var fechaInicio = DateTime.Parse(fecha_ejecucion);
            _programingMethodsService.ProgramPublishRepositoryJob(idRep, fechaInicio);
            return Ok();
        }

        /// <summary>
        /// Añade a la cola una tarea que estaba prevista ejecutar en un futuro 
        /// </summary>
        /// <param name="id">identificador de la tarea programada</param>
        /// <returns></returns> 
        [HttpPut]
        public IActionResult EnqueuedScheduledJob(string id)
        {
            if(_cronApiService.ExistScheduledJob(id))
            {
                _cronApiService.EnqueueJob(id);
                return Ok();
            }
            else
            {
                return BadRequest("no existe la tarea programada");
            }
        }

        /// <summary>
        /// Elimina una tarea programada
        /// </summary>
        /// <param name="id">identificador de la tarea programada</param>
        /// <returns></returns> 
        [HttpDelete]
        public IActionResult DeleteScheduledJob(string id)
        {
            if (_cronApiService.ExistScheduledJob(id))
            {
                _cronApiService.DeleteJob(id);
                return Ok();
            }
            else
            {
                return BadRequest("no existe la tarea programada");
            }
        }
    }
}