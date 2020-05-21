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
    public class ProgramingMethodsService : IProgramingMethodService
    {
        private CallApiService _serviceApi;
        private HangfireEntityContext _context;

        public ProgramingMethodsService(CallApiService serviceApi, HangfireEntityContext context)
        {
            _serviceApi = serviceApi;
            _context = context;
        }

        [AutomaticRetry(Attempts = 0, DelaysInSeconds = new int[] { 3600 })]
        [ProlongExpirationTime]
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
                string result = _serviceApi.CallPostApi($"sync/execute", objeto);///{idRepository}
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

        public static void ProgramRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            ConfigUrlService serviceUrl = new ConfigUrlService();
            CallApiService serviceApi = new CallApiService(serviceUrl);
            ProgramingMethodsService service = new ProgramingMethodsService(serviceApi, null);

            RecurringJob.AddOrUpdate(nombreCron, () => service.PublishRepositories(idRepository, fecha, set, codigoObjeto), cronExpression);
        }

        public string ProgramPublishRepositoryJob(Guid idRepository, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            string id = BackgroundJob.Schedule(() => PublishRepositories(idRepository, fecha, set, codigoObjeto), fechaInicio);
            JobRepository jobRepository = new JobRepository()
            {
                IdJob = id,
                IdRepository = idRepository,
                FechaEjecucion = fechaInicio.ToString("dd/MM/yyyy hh:mm")
            };
            _context.JobRepository.Add(jobRepository);
            _context.SaveChanges();
            return id;
        }
        public void ProgramPublishRepositoryRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            string id = BackgroundJob.Schedule(() => ProgramRecurringJob(idRepository,nombreCron,cronExpression,fecha,set,codigoObjeto), fechaInicio);
            JobRepository jobRepository = new JobRepository()
            {
                IdJob = $"{id}_{nombreCron}_{cronExpression}",
                IdRepository = idRepository,
                FechaEjecucion = fechaInicio.ToString("dd/MM/yyyy hh:mm")
            };
            _context.JobRepository.Add(jobRepository);
            _context.SaveChanges();
        }

        private void CreateLoggin(string pTimestamp, string id)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File($"logs/job_{id}/log_{pTimestamp}.txt").CreateLogger();
        }

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
