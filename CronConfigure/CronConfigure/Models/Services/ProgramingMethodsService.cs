using CronConfigure.Filters;
using CronConfigure.Models.Entitties;
using CronConfigure.ViewModels;
using Hangfire;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase para la programación de tareas
    ///</summary>
    public class ProgramingMethodsService : IProgramingMethodService
    {
        private CallApiService _serviceApi;
        private HangfireEntityContext _context;
        readonly TokenBearer _token;

        public ProgramingMethodsService(CallApiService serviceApi, HangfireEntityContext context, CallTokenService tokenService)
        {
            _serviceApi = serviceApi;
            _context = context;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCarga();
            }
        }

        [AutomaticRetry(Attempts = 0, DelaysInSeconds = new int[] { 3600 })]
        [ProlongExpirationTime]
        ///<summary>
        ///Método para la sincronización de repositorios
        ///</summary>
        ///<param name="idRepositoryGuid">identificador del repositorio a sincronizar</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones
        /// <param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public string PublishRepositories(Guid idRepositoryGuid, DateTime? fecha = null, string pSet = null, string codigoObjeto = null)
        {
            string idRepository = idRepositoryGuid.ToString();
            try
            {
                object objeto = new
                {
                    repository_identifier = idRepositoryGuid,
                    codigo_objeto = codigoObjeto,
                    fecha_from = fecha,
                    set = pSet
                };
                string result = _serviceApi.CallPostApi($"sync/execute", objeto, _token);///{idRepository}
                result = JsonConvert.DeserializeObject<string>(result);
                return result;
            }
            catch (Exception ex)
            {
                string timeStamp = CreateTimeStamp();
                CreateLoggin(timeStamp, idRepository);
                Log.Error($"{ex.Message}\n{ex.StackTrace}\n");
                throw new Exception(ex.Message);
                //return ex.Message;
            } 
        }

        ///<summary>
        ///Método para programar una sincronización recurrente
        ///</summary>
        ///<param name="idRepository">identificador del repositorio a sincronizar</param>
        ///<param name="nombreCron">Nombre de la tarea recurrente</param>
        ///<param name="cronExpression">expresión de recurrencia</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        ///<param name="set">tipo del objeto, usado para filtrar por agrupaciones
        ///<param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public static void ProgramRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            ConfigUrlService serviceUrl = new ConfigUrlService();
            CallApiService serviceApi = new CallApiService(serviceUrl);
            ProgramingMethodsService service = new ProgramingMethodsService(serviceApi, null, null);

            RecurringJob.AddOrUpdate(nombreCron, () => service.PublishRepositories(idRepository, fecha, set, codigoObjeto), cronExpression);
        }

        ///<summary>
        ///Método para programar una tarea
        ///</summary>
        ///<param name="idRepository">identificador del repositorio a sincronizar</param>
        ///<param name="fechaInicio">Fecha de la ejecución</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        ///<param name="set">tipo del objeto, usado para filtrar por agrupaciones
        ///<param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public string ProgramPublishRepositoryJob(Guid idRepository, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            string id = BackgroundJob.Schedule(() => PublishRepositories(idRepository, fecha, set, codigoObjeto), fechaInicio);
            JobRepository jobRepository = new JobRepository()
            {
                IdJob = id,
                IdRepository = idRepository,
                FechaEjecucion = fechaInicio
            };
            _context.JobRepository.Add(jobRepository);
            _context.SaveChanges();
            return id;
        }

        ///<summary>
        ///Método para programar una tarea con una sincronización recurrente
        ///</summary>
        ///<param name="idRepository">identificador del repositorio a sincronizar</param>
        ///<param name="nombreCron">Nombre de la tarea recurrente</param>
        ///<param name="cronExpression">expresión de recurrencia</param>
        ///<param name="fechaInicio">Fecha en la que se ejecutará la tarea y se activará la tarea recurrente</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        ///<param name="set">tipo del objeto, usado para filtrar por agrupaciones
        ///<param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public void ProgramPublishRepositoryRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            string id = BackgroundJob.Schedule(() => ProgramRecurringJob(idRepository,nombreCron,cronExpression,fecha,set,codigoObjeto), fechaInicio);
            JobRepository jobRepository = new JobRepository()
            {
                IdJob = $"{id}_{nombreCron}_{cronExpression}",
                IdRepository = idRepository,
                FechaEjecucion = fechaInicio
            };
            _context.JobRepository.Add(jobRepository);
            _context.SaveChanges();
        }

        ///<summary>
        ///Creación de log
        ///</summary>
        ///<param name="pTimestamp">String de fecha
        ///<param name="id">Identificador del job
        private void CreateLoggin(string pTimestamp, string id)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File($"logs/job_{id}/log_{pTimestamp}.txt").CreateLogger();
        }

        ///<summary>
        ///Creación de una cadena de texto con la fecha actual
        ///</summary>
        private string CreateTimeStamp()
        {
            DateTime time = DateTime.Now;
            string month = time.Month.ToString();
            if (month.Length == 1)
            {
                month = $"0{month}";
            }
            string day = time.Day.ToString();
            if (day.Length == 1)
            {
                day = $"0{day}";
            }
            string timeStamp = $"{time.Year.ToString()}{month}{day}";
            return timeStamp;
        }
    }
}
