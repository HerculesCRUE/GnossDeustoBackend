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
    [Route("[controller]")]
    [ApiController]
    public class RecurringJobController : ControllerBase
    {

        public CronApiService _cronApiService;
        private ProgramingMethodsService _programingMethodsService;

        public RecurringJobController(CronApiService cronApiService, ProgramingMethodsService programingMethodsService)
        {
            _cronApiService = cronApiService;
            _programingMethodsService = programingMethodsService;
        }

        /// <summary>
        /// Añade una sincornizacion recurrente
        /// </summary>
        /// <param name="id_repository">identificador del repositorio a sincronizar </param>
        /// <param name="nombre_job">nombre de la tarea</param>
        /// <param name="fecha_inicio">momento a partir del cúal empieza la sincronización</param>
        /// <param name="cron_expression">expresión recurrente del cron (https://crontab.guru/)</param>
        /// <param name="fecha">fecha a partir de la cual se debe actualizar</param>
        /// <param name="set">tipo del objeto</param>
        /// <param name="codigo_objeto">codigo del objeto</param>
        /// <returns></returns> 
        [HttpPost]
        public IActionResult AddExecution(CreateRecurringJobModel recurringJobModel)
        {
            Guid idRep = Guid.Empty;
            DateTime fechaInicio = DateTime.Now;
            DateTime? fechaDateTime = null;
            //if (codigo_objeto != null && set == null)
            //{
            //    return BadRequest("falta el tipo de objeto");
            //}
            if (recurringJobModel.fecha_inicio != null)
            {
                try
                {
                    fechaInicio = DateTime.Parse(recurringJobModel.fecha_inicio);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de inicio inválida");
                }
            }

            if (recurringJobModel.fecha != null)
            {
                try
                {
                    fechaDateTime = DateTime.Parse(recurringJobModel.fecha);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de sincronzación inválida");
                }
            }
            try
            {
                idRep = new Guid(recurringJobModel.id_repository);
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
            bool validCronExpr = true;
            if (_cronApiService.ExistRecurringJob(recurringJobModel.nombre_job))
            {
                return BadRequest("Ya existe una tarea con ese nombre");
            }
            else
            {
                var correct = CrontabSchedule.TryParse(recurringJobModel.cron_expression);
                if (correct != null)
                {
                    //BackgroundJob.Schedule(() => ExampleService.PonerEnCola(nombre_job, nombre_fichero, cron_expression, execute_inmediatly), fechaInicio);
                    _programingMethodsService.ProgramPublishRepositoryRecurringJob(idRep, recurringJobModel.nombre_job, recurringJobModel.cron_expression, fechaInicio,fechaDateTime, recurringJobModel.set);
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

        /// <summary>
        /// Elimina una tarea recurrente
        /// </summary>
        /// <param name="nombre_job">nombre de la tarea recurrente a eliminar</param>
        /// <returns></returns> 
        [HttpDelete]
        public IActionResult DeleteRecurringJob(string nombre_job)
        {
            _cronApiService.DeleteRecurringJob(nombre_job);
            return Ok();
        }

        /// <summary>
        /// Obtiene un listado de tareas recurrentes 
        /// </summary>
        /// <returns>listado de tareas recurrentes</returns> 
        [HttpGet]
        public IActionResult GetRecurringJob()
        {
            return Ok(_cronApiService.GetRecurringJobs());
        }

        /// <summary>
        /// Obtiene una tarea recurrente
        /// </summary>
        /// <param name="id">nombre de la tarea recurrente</param>
        /// <returns>tarea recurrentes</returns> 
        [HttpGet("{id}")]
        public IActionResult GetRecurringJob(string id)
        {
            return Ok(_cronApiService.GetRecurringJobs(id));
        }

        /// <summary>
        /// Obtiene un listado de tareas que se han ejecutado a partir de una tarea recurrente 
        /// </summary>
        /// <param name="id">nombre de la tarea recurrente</param>
        /// <returns>listado de tareas</returns> 
        [HttpGet("jobs/{id}")]
        public IActionResult GetJobsOfRecurringJob(string id)
        {
            return Ok(_cronApiService.GetJobsOfRecurringJob(id));
        }

    }
}