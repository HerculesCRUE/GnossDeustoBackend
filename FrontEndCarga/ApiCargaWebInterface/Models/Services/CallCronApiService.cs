using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallCronApiService
    {
        readonly CallCronService _serviceApi;
        readonly static string _urlRecurringJobApi = "RecurringJob";
        readonly static string _urlJobApi = "Job";
        public CallCronApiService(CallCronService serviceApi)
        {
            _serviceApi = serviceApi;
        }
        public string CreateJob(CreateJobViewModel newJob)
        {//string id_repository, string fecha_inicio, string fecha = null, string set = null
            string guidAdded;
            Object newCreateJob = new
            {
                id_repository = newJob.IdRepository.ToString(),
                fecha_inicio = newJob.FechaIinicio,
                fecha = newJob.FechaFrom,
                set = newJob.Set
            };
            string result = _serviceApi.CallPostApi(_urlJobApi, newCreateJob);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }

        public string CreateRecurringJob(CreateJobViewModel newJob)
        {//string id_repository, string nombre_job, string fecha_inicio, string cron_expression, string fecha = null, string set = null
            string guidAdded;
            Object newCreateRecuringJob = new
            {
                id_repository = newJob.IdRepository.ToString(),
                fecha_inicio = newJob.FechaIinicio,
                fecha = newJob.FechaFrom,
                set = newJob.Set,
                nombre_job = newJob.Nombre_job,
                cron_expression = newJob.CronExpression
            };
            string result = _serviceApi.CallPostApi(_urlRecurringJobApi, newCreateRecuringJob);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }
    }
}
