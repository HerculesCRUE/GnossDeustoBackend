// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de cron
﻿using ApiCargaWebInterface.Models.Entities;
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
        readonly TokenBearer _token;
        public CallCronApiService(CallCronService serviceApi, CallTokenService tokenService)
        {
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCron();
            }
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
            string result = _serviceApi.CallPostApi($"{_urlJobApi}?{uriParams}", null, _token);
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
            string result = _serviceApi.CallPostApi($"{ _urlRecurringJobApi}?{uriParams}", null, _token);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }

        public void DeleteRecurringJob(string id)
        {
            string result = _serviceApi.CallDeleteApi($"{_urlRecurringJobApi}?nombre_job={id}", _token);
            result = JsonConvert.DeserializeObject<string>(result);
        }

        public void DeleteScheduledJob(string id)
        {
            string result = _serviceApi.CallDeleteApi($"{_urlScheduledJobApi}?id={id}", _token);
            result = JsonConvert.DeserializeObject<string>(result);
        }
    }
}
