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
    [Route("[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private CronApiService _cronApiService;
        private ProgramingMethodsService _programingMethodsService;
        public JobController(CronApiService cronApiService, ProgramingMethodsService programingMethodsService)
        {
            _cronApiService = cronApiService;
            _programingMethodsService = programingMethodsService;
        }

        /// <summary>
        /// Programa una sincronización de repositorios para una fecha concreta
        /// </summary>
        /// <param name="id_repository">identificador del repositorio</param>
        /// <param name="fecha_inicio">fecha de ejecución</param>
        /// <returns>identifdicador de la tarea</returns> 
        [HttpPost]
        public IActionResult AddExecution(string id_repository, string fecha_inicio)
        {
            var fechaInicio = DateTime.Parse(fecha_inicio);
            Guid idRep = Guid.Empty;
            try
            {
                idRep = new Guid(id_repository);
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
            string id = _programingMethodsService.ProgramPublishRepositoryJob(idRep, fechaInicio);

            return Ok(id);
        }

        /// <summary>
        /// Vuelve a ejecutar una tarea ya ejecutada o programada
        /// </summary>
        /// <param name="id">identificador de la tarea</param>
        /// <returns></returns> 
        [HttpPost("{id}")]
        public IActionResult AddExecution(string id)
        {
            if (_cronApiService.ExistJob(id))
            {
                BackgroundJob.Requeue(id);
                return Ok(id);
            }
            else
            {
                return BadRequest("el job no se encuentra en la lista de ejecutados");
            }
        }

        /// <summary>
        /// devuelve una lista de tareas paginadas
        /// </summary>
        /// <param name="type">tipo de las tareas devueltas: 0: para todos los tipos, 1: para las que han fallado, 2: para las correctas </param>
        /// <param name="from">número desde el que tiene que empezar a traer</param>
        /// <param name="count">número de tareas a traer</param>
        /// <returns>listado de tareas</returns> 
        [HttpGet]
        public IActionResult GetJobs(JobType type, int from, int count)
        {
            return Ok(_cronApiService.GetJobs(type, from, count));

        }

        
    }
}