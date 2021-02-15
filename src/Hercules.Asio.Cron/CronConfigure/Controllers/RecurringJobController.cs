// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para gestionar las tareas tareas recurrentes creadas así como crear tareas recurrentes nuevas
using System;
using CronConfigure.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace CronConfigure.Controllers
{
    /// <summary>
    /// Controlador para gestionar las tareas tareas recurrentes creadas así como crear tareas recurrentes nuevas
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class RecurringJobController : ControllerBase
    {

        public ICronApiService _cronApiService;
        private IProgramingMethodService _programingMethodsService;
        private IRepositoryCronService _repositoryCronService;

        public RecurringJobController(ICronApiService cronApiService, IProgramingMethodService programingMethodsService, IRepositoryCronService repositoryCronService)
        {
            _cronApiService = cronApiService;
            _programingMethodsService = programingMethodsService;
            _repositoryCronService = repositoryCronService;
        }

        /// <summary>
        /// Añade una sincornizacion recurrente
        /// </summary>
        /// <param name="id_repository">identificador del repositorio a sincronizar, este parametro se puede obtener con el método http://herc-as-front-desa.atica.um.es/carga/etl-config/Repository </param>
        /// <param name="nombre_job">nombre de la tarea recurrente, no puede haber varias tareas con el mismo nombre, este nombre es elegido por el usuario que crea la tarea recurrente</param>
        /// <param name="fecha_inicio">momento a partir del cúal empieza la sincronización,el formato de fecha es: dd/MM/yyyy hh:mm ejemplo de formato de fecha: 07/05/2020 12:23</param>
        /// <param name="cron_expression">el parametro cron_expresion sigue un patrón de 5 atributos, separados por espacios entre sí: * * * * *. El primero corresponde al minuto, el segundo a la hora, a continuación el día del mes, seguido por el mes y posteriormente el día del mes. Un ejemplo sería: */15 * * * * que correspondería a cada 15 minutos, para probar las expresisiones se puede acudir a https://crontab.guru/</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones, este parametro se puede obtener de http://herc-as-front-desa.atica.um.es/carga/etl/ListSets/{identificador_del_repositorio}</param>
        /// <param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro, este parametro se puede obtener en la respuesta identifier que da el método http://herc-as-front-desa.atica.um.es/carga/etl/ListIdentifiers/{identificador_del_repositorio}?metadataPrefix=rdf</param>
        /// <returns></returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public IActionResult AddExecution(string id_repository, string nombre_job, string fecha_inicio, string cron_expression, string set = null, string codigo_objeto = null)
        {
            Guid idRep = Guid.Empty;
            DateTime fechaInicio = DateTime.Now;
            if (codigo_objeto != null && set == null)
            {
                return BadRequest("falta el tipo de objeto");
            }
            if (fecha_inicio != null)
            {
                try
                {
                    fechaInicio = DateTime.ParseExact(fecha_inicio, "dd/MM/yyyy HH:mm", null);
                }
                catch (Exception)
                {
                    return BadRequest("fecha de inicio inválida");
                }
            }

            try
            {
                idRep = new Guid(id_repository);
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
            bool validCronExpr = true;
            if (_cronApiService.ExistRecurringJob(nombre_job))
            {
                return BadRequest("Ya existe una tarea con ese nombre");
            }
            else if (string.IsNullOrEmpty(nombre_job))
            {
                return BadRequest("El nombre no puede ser vacío");
            }
            else
            {
                var correct = CrontabSchedule.TryParse(cron_expression);
                if (correct != null)
                {
                    _programingMethodsService.ProgramPublishRepositoryRecurringJob(idRep, nombre_job, cron_expression, fechaInicio,set,codigo_objeto);
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
        /// <param name="nombre_job">nombre de la tarea recurrente a eliminar, es el nombre que se le ha puesto a la tarea en el momento de su creación se puede obtener desde http://herc-as-front-desa.atica.um.es/cron-config/RecurringJob</param>
        /// <returns></returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete]
        public IActionResult DeleteRecurringJob(string nombre_job)
        {
            _cronApiService.DeleteRecurringJob(nombre_job);
            return Ok();
        }

        /// <summary>
        /// Obtiene el listado de tareas recurrentes 
        /// </summary>
        /// <returns>listado de tareas recurrentes</returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public IActionResult GetRecurringJob()
        {
            return Ok(_cronApiService.GetRecurringJobs());
        }

        /// <summary>
        /// Obtiene una tarea recurrente
        /// </summary>
        /// <param name="id">nombre de la tarea recurrentea obtener, es el nombre que se le ha puesto a la tarea en el momento de su creación se puede obtener desde http://herc-as-front-desa.atica.um.es/cron-config/RecurringJob</param>
        /// <returns>tarea recurrentes</returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public IActionResult GetRecurringJob(string id)
        {
            return Ok(_cronApiService.GetRecurringJobs(id));
        }

        /// <summary>
        /// Obtiene un listado de tareas que se han ejecutado a partir de una tarea recurrente 
        /// </summary>
        /// <param name="id">nombre de la tarea recurrente de la que se quieren obtener las tareas ejecutadas, es el nombre que se le ha puesto a la tarea en el momento de su creación se puede obtener desde http://herc-as-front-desa.atica.um.es/cron-config/RecurringJob</param>
        /// <returns>listado de tareas</returns> 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("jobs/{id}")]
        public IActionResult GetJobsOfRecurringJob(string id)
        {
            return Ok(_cronApiService.GetJobsOfRecurringJob(id));
        }

        /// <summary>
        /// Obtiene un listado de tareas recurrentes ejecutadas de un repositorio
        /// </summary>
        /// <param name="id">Identidicador del repositorio a obtener las tareas ejecutadas, este parametro se puede obtener con el método http://herc-as-front-desa.atica.um.es/carga/etl-config/Repository</param>
        /// <returns>listado de tareas</returns> 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("repository/{id}")]
        public IActionResult GetJobsOfRepository(string id)
        {

            Guid idRep = Guid.Empty;
            try
            {
                idRep = new Guid(id);
                return Ok(_repositoryCronService.GetRecurringJobs(idRep));
            }
            catch (Exception)
            {
                return BadRequest("identificador invalido");
            }
        }

    }
}