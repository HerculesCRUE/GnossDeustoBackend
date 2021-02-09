// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas a los métodos del apiCron
﻿using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para hacer llamadas a los métodos del apiCron
    /// </summary>
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
        /// <summary>
        /// Crea una tarea
        /// </summary>
        /// <param name="newJob">Tarea a crear</param>
        /// <returns>Identificador de la nueva tarea</returns>
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
            string result = _serviceApi.CallPostApi("",$"{_urlJobApi}?{uriParams}", null, _token);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }
        /// <summary>
        /// Obtiene las tareas ejecutadas a partir de una tarea recurrente
        /// </summary>
        /// <param name="name">Nombre de la tarea recurrente</param>
        /// <returns>Lista de tareas</returns>
        internal List<JobViewModel> GetJobsOfRecurringJob(string name)
        {
            string result = _serviceApi.CallGetApi("", $"{_urlRecurringJobApi}/Jobs/{name}", _token);
            List<JobViewModel> resultObject = JsonConvert.DeserializeObject<List<JobViewModel>>(result);
            return resultObject;
        }

        /// <summary>
        /// Vuelve a encolar una tarea
        /// </summary>
        /// <param name="idJob">Identificador de la tarea a encolar</param>
        public void ReQueueJob(string idJob)
        {
            string result = _serviceApi.CallPostApi("", $"{ _urlJobApi}/{idJob}", null, _token);
        }
        /// <summary>
        /// Crea una tarea recurrente
        /// </summary>
        /// <param name="newJob">Tarea recurrente a crear</param>
        /// <returns>Nombre de la tarea creada</returns>
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
            string result = _serviceApi.CallPostApi("", $"{ _urlRecurringJobApi}?{uriParams}", null, _token);
            guidAdded = JsonConvert.DeserializeObject<string>(result);
            return guidAdded;
        }

        /// <summary>
        /// Elimina una tarea recurrente
        /// </summary>
        /// <param name="id">Nombre de la tarea recurrente a eliminar</param>
        public void DeleteRecurringJob(string id)
        {
            string result = _serviceApi.CallDeleteApi("", $"{_urlRecurringJobApi}?nombre_job={id}", _token);
            result = JsonConvert.DeserializeObject<string>(result);
        }

        /// <summary>
        /// Elimina una tarea programada
        /// </summary>
        /// <param name="id">Identificador de la tarea a eliminar</param>
        public void DeleteScheduledJob(string id)
        {
            string result = _serviceApi.CallDeleteApi("",$"{_urlScheduledJobApi}?id={id}", _token);
            result = JsonConvert.DeserializeObject<string>(result);
        }

        /// <summary>
        /// Obtiene una tarea
        /// </summary>
        /// <param name="id">identificador de la tarea</param>
        /// <returns>una tarea</returns>
        public JobViewModel GetJob(string id)
        {
            string result = _serviceApi.CallGetApi("", $"{_urlJobApi}/{id}", _token);
            JobViewModel resultObject = JsonConvert.DeserializeObject<JobViewModel>(result);
            return resultObject;
        }

        /// <summary>
        /// Obtiene todas las tareas
        /// </summary>
        /// <returns>lista de tareas</returns>
        public List<JobViewModel> GetJobs()
        {
            string result = _serviceApi.CallGetApi("", $"{_urlJobApi}", _token);
            List<JobViewModel> resultObject = JsonConvert.DeserializeObject<List<JobViewModel>>(result);
            return resultObject;
        }
        /// <summary>
        /// Obtiene una tarea recurrente
        /// </summary>
        /// <param name="name">Nombre de la tarea recurrente</param>
        /// <returns>Una tarea recurrente</returns>
        public RecurringJobViewModel GetRecurringJob(string name)
        {
            string result = _serviceApi.CallGetApi("", $"{_urlRecurringJobApi}/{name}", _token);
            RecurringJobViewModel resultObject = JsonConvert.DeserializeObject<RecurringJobViewModel>(result);
            return resultObject;
        }
    }
}
