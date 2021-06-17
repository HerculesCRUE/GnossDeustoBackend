// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para gestionar las tareas ejecutadas y poder crear tareas nuevas
using System;
using System.Diagnostics.CodeAnalysis;
using CronConfigure.Models.Enumeracion;
using CronConfigure.Models.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CronConfigure.Controllers
{
    /// <summary>
    /// Controlador para gestionar las tareas ejecutadas y poder crear tareas nuevas
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class JobController : ControllerBase
    {
        readonly private ICronApiService _cronApiService;
        readonly private IProgramingMethodService _programingMethodsService;
        readonly private IRepositoryCronService _repositoryCronService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cronApiService">Servicio para interactuar con base de datos</param>
        /// <param name="programingMethodsService">Prgroamador de tareas</param>
        /// <param name="repositoryCronService">Servicios para las vinculaciones con los repositorios</param>
        public JobController(ICronApiService cronApiService, IProgramingMethodService programingMethodsService, IRepositoryCronService repositoryCronService)
        {
            _cronApiService = cronApiService;
            _programingMethodsService = programingMethodsService;
            _repositoryCronService = repositoryCronService;
        }

        /// <summary>
        /// Programa una sincronización de repositorios para una fecha concreta
        /// </summary>
        /// <param name="id_repository">identificador del repositorio, este parametro se puede obtener con el método http://herc-as-front-desa.atica.um.es/carga/etl-config/Repository</param>
        /// <param name="fecha_inicio">fecha a partir de la cual se ejecutará,el formato de fecha es: dd/MM/yyyy hh:mm ejemplo de formato de fecha: 07/05/2020 12:23</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones, este parametro se puede obtener de http://herc-as-front-desa.atica.um.es/carga/etl/ListSets/{identificador_del_repositorio}</param>
        /// <param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro, este parametro se puede obtener en la respuesta identifier que da el método http://herc-as-front-desa.atica.um.es/carga/etl/ListIdentifiers/{identificador_del_repositorio}?metadataPrefix=rdf</param>
        /// <returns>identifdicador de la tarea</returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public IActionResult AddExecution(string id_repository, string fecha_inicio = null, string set = null, string codigo_objeto = null)
        {
            DateTime fechaInicio = DateTime.Now;
            if (codigo_objeto != null && set == null)
            {
                return BadRequest("falta el tipo de objeto");
            }
            if (fecha_inicio != null)
            {
                try
                {
                    fechaInicio = DateTime.ParseExact(fecha_inicio, "dd/MM/yyyy HH:mm",null);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de inicio inválida");
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
            string id = _programingMethodsService.ProgramPublishRepositoryJob(idRep, fechaInicio, set, codigo_objeto);

            return Ok(id);
        }
        
        /// <summary>
        /// Vuelve a ejecutar una tarea ya ejecutada o programada
        /// </summary>
        /// <param name="id">identificador de la tarea, el identificador se puede obterner del método: que lista los jobs http://herc-as-front-desa.atica.um.es/cron-config/Job?type=0&amp;from=0&amp;count=100</param>
        /// <returns></returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{id}")]
        [ExcludeFromCodeCoverage]
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
        /// <param name="type">tipo de las tareas devueltas: 0: para todos los tipos, 1: para las que han fallado, 2: para las correctas, 3: para los que se están procesando </param>
        /// <param name="from">número desde el cual se va a traer las tareas del listado, por defecto 0 para empezar a traer desde el primer elemento de la lista de tareas</param>
        /// <param name="count">número máximo de tareas a traer</param>
        /// <returns>listado de tareas</returns> 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ExcludeFromCodeCoverage]
        public IActionResult GetJobs(JobType type, int count, int from = 0)
        {
            return Ok(_cronApiService.GetJobs(type, from, count));

        }

        /// <summary>
        /// devuelve una tarea
        /// </summary>
        /// <param name="id">identificador de la tarea, el identificador se puede obterner del método: que lista los jobs http://herc-as-front-desa.atica.um.es/cron-config/Job?type=0&amp;from=0&amp;count=100</param>
        /// <returns>tarea</returns> 
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ExcludeFromCodeCoverage]
        public IActionResult GetJob(string id)
        {
            return Ok(_cronApiService.GetJob(id));

        }

        /// <summary>
        /// Obtiene un listado de tareas ejecutadas de un repositorio
        /// </summary>
        /// <param name="id">Identidicador del repositorio a obtener las tareas ejecutadas, este parametro se puede obtener con el método http://herc-as-front-desa.atica.um.es/carga/etl-config/Repository</param>
        /// <returns>listado de tareas</returns> 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("repository/{id}")]
        [ExcludeFromCodeCoverage]
        public IActionResult GetJobsOfRepository(string id)
        {
                
            Guid idRep = Guid.Empty;
            try
            {
                idRep = new Guid(id);
                return Ok(_repositoryCronService.GetAllJobs(idRep));
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
        }

    }
}