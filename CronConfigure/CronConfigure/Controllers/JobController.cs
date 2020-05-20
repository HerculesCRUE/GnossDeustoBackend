using System;
using System.Collections.Generic;
using System.Globalization;
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
        private ICronApiService _cronApiService;
        private IProgramingMethodService _programingMethodsService;
        public JobController(ICronApiService cronApiService, IProgramingMethodService programingMethodsService)
        {
            _cronApiService = cronApiService;
            _programingMethodsService = programingMethodsService;
        }

        /// <summary>
        /// Programa una sincronización de repositorios para una fecha concreta
        /// </summary>
        /// <param name="id_repository">identificador del repositorio, este parametro se puede obtener con el método http://herc-as-front-desa.atica.um.es/carga/etl-config/Repository</param>
        /// <param name="fecha_inicio">fecha a partir de la cual se ejecutará,el formato de fecha es: dd/MM/yyyy hh:mm ejemplo de formato de fecha: 07/05/2020 12:23</param>
        /// <param name="fecha">fecha a partir de la cual se debe actualizar,el formato de fecha es: dd/MM/yyyy hh:mm ejemplo de formato de fecha: 07/05/2020 12:23</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones, este parametro se puede obtener de http://herc-as-front-desa.atica.um.es/carga/etl/ListSets/{identificador_del_repositorio}</param>
        /// <param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro, este parametro se puede obtener en la respuesta identifier que da el método http://herc-as-front-desa.atica.um.es/carga/etl/ListIdentifiers/{identificador_del_repositorio}?metadataPrefix=rdf</param>
        /// <returns>identifdicador de la tarea</returns> 
        [HttpPost]
        public IActionResult AddExecution(string id_repository, string fecha_inicio = null, string fecha = null, string set = null, string codigo_objeto = null)
        {
            DateTime fechaInicio = DateTime.Now;
            DateTime? fechaDateTime = null;
            if (codigo_objeto != null && set == null)
            {
                return BadRequest("falta el tipo de objeto");
            }
            if (fecha_inicio != null)
            {
                try
                {
                    fechaInicio = DateTime.ParseExact(fecha_inicio, "dd/MM/yyyy hh:mm",null);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de inicio inválida");
                }
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
            string id = _programingMethodsService.ProgramPublishRepositoryJob(idRep, fechaInicio, fechaDateTime, set, codigo_objeto);

            return Ok(id);
        }

        /// <summary>
        /// Vuelve a ejecutar una tarea ya ejecutada o programada
        /// </summary>
        /// <param name="id">identificador de la tarea, el identificador se puede obterner del método: que lista los jobs http://herc-as-front-desa.atica.um.es/cron-config/Job?type=0&amp;from=0&amp;count=100</param>
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
        /// <param name="from">número desde el cual se va a traer las tareas del listado, por defecto 0 para empezar a traer desde el primer elemento de la lista de tareas</param>
        /// <param name="count">número máximo de tareas a traer</param>
        /// <returns>listado de tareas</returns> 
        [HttpGet]
        public IActionResult GetJobs(JobType type, int count, int from = 0)
        {
            return Ok(_cronApiService.GetJobs(type, from, count));

        }

        
    }
}