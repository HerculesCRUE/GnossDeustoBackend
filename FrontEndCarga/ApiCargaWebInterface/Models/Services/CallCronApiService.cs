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
        readonly static string _urlScheduledJobApi = "ScheduledJob";
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
                set = newJob.Set,
                codigo_objeto = newJob.CodigoObjeto
            };
            string stringData = JsonConvert.SerializeObject(newCreateJob);
            string uriParams = stringData.Replace(':', '=').Replace(',', '&').Replace("\"", "").Replace("{", "").Replace("}", "").Replace("null","");
            if (newJob.FechaIinicio.HasValue)
            {
                uriParams += $"&fecha_inicio={newJob.FechaIinicio.Value.ToString("dd/MM/yyyy HH:mm")}";
            }
            if (newJob.FechaFrom.HasValue)
            {
                uriParams += $"&fecha={newJob.FechaFrom.Value.ToString("dd/MM/yyyy HH:mm")}";
            }
            string result = _serviceApi.CallPostApi($"{_urlJobApi}?{uriParams}", null);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }

        public string CreateRecurringJob(CreateJobViewModel newJob)
        {//string id_repository, string nombre_job, string fecha_inicio, string cron_expression, string fecha = null, string set = null
            string guidAdded;
            Object newCreateRecuringJob = new
            {
                id_repository = newJob.IdRepository.ToString(),
                set = newJob.Set,
                codigo_objeto = newJob.CodigoObjeto,
                nombre_job = newJob.Nombre_job,
                cron_expression = newJob.CronExpression
            };
            string stringData = JsonConvert.SerializeObject(newCreateRecuringJob);
            string uriParams = stringData.Replace(':', '=').Replace(',', '&').Replace("\"", "").Replace("{", "").Replace("}", "").Replace("null", "");
            if (newJob.FechaIinicio.HasValue)
            {
                uriParams += $"&fecha_inicio={newJob.FechaIinicio.Value.ToString("dd/MM/yyyy HH:mm")}";
            }
            if (newJob.FechaFrom.HasValue)
            {
                uriParams += $"&fecha={newJob.FechaFrom.Value.ToString("dd/MM/yyyy HH:mm")}";
            }
            string result = _serviceApi.CallPostApi($"{ _urlRecurringJobApi}?{uriParams}", null);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }

        public void DeleteRecurringJob(string id)
        {
            string result = _serviceApi.CallDeleteApi($"{_urlRecurringJobApi}?nombre_job={id}");
            result = JsonConvert.DeserializeObject<string>(result);
        }

        public void DeleteScheduledJob(string id)
        {
            string result = _serviceApi.CallDeleteApi($"{_urlScheduledJobApi}?id={id}");
            result = JsonConvert.DeserializeObject<string>(result);
        }
    }
}
