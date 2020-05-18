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
        /// <param name="fecha_ejecucion">fecha en la que se ejecutará la tarea,el formato de fecha es: dd/MM/yyyy hh:mm ejemplo de formato de fecha: 07/05/2020 12:23</param>
        /// <param name="id_repository">identificador del repositorio,  este parametro se puede obtener con el método http://herc-as-front-desa.atica.um.es/carga/etl-config/Repository</param>
        /// <param name="fecha">fecha a partir de la cual se debe actualizar,el formato de fecha es: dd/MM/yyyy hh:mm ejemplo de formato de fecha: 07/05/2020 12:23</param>
        /// <param name="set">tipo del objeto</param>
        /// <param name="codigo_objeto">codigo del objeto</param>
        /// <returns></returns> 
        [HttpPost]
        public IActionResult AddScheduledJob(string fecha_ejecucion, string id_repository, string fecha = null, string set = null, string codigo_objeto = null)
        {
            DateTime fechaInicio = DateTime.Now;
            DateTime? fechaDateTime = null;
            if (codigo_objeto != null && set == null)
            {
                return BadRequest("falta el tipo de objeto");
            }
            if (fecha_ejecucion != null)
            {
                try
                {
                    fechaInicio = DateTime.ParseExact(fecha_ejecucion, "dd/MM/yyyy hh:mm", null);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de ejecución inválida");
                }
            }
            else
            {
                return BadRequest("La fecha de ejecución es obligatoria");
            }
            if (fecha != null)
            {
                try
                {
                    fechaDateTime = DateTime.ParseExact(fecha, "dd/MM/yyyy hh:mm", null);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de sincronzación inválida");
                }
            }
            Guid idRep = Guid.Empty;
            try
            {
                idRep = new Guid(id_repository);
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
            _programingMethodsService.ProgramPublishRepositoryJob(idRep, fechaInicio, fechaDateTime, set, codigo_objeto);
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