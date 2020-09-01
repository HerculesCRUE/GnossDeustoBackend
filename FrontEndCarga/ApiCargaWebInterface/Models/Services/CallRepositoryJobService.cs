// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de Cron
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para hacer llamadas al api de Cron
    /// </summary>
    public class CallRepositoryJobService : ICallRepositoryJobService
    {
        readonly CallCronService _serviceApi;
        readonly TokenBearer _token;
        public CallRepositoryJobService(CallCronService serviceApi, CallTokenService tokenService)
        {
            _serviceApi = serviceApi;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCron();
            }
        }
        /// <summary>
        /// Obtiene las tareas de sincronización ejecutadas de un repositorio OAIPMH
        /// </summary>
        /// <param name="idRepositoy">Identificador del repositorio</param>
        /// <returns>Lista de tareas</returns>
        public List<JobViewModel> GetJobsOfRepo(Guid idRepositoy)
        {
            string result = _serviceApi.CallGetApi("",$"Job/repository/{idRepositoy.ToString()}", _token);
            List<JobViewModel> resultObject = JsonConvert.DeserializeObject<List<JobViewModel>>(result);
            return resultObject;
        }
        /// <summary>
        /// Obtiene las tareas recurrentes de un repositorio OAIPMH
        /// </summary>
        /// <param name="idRepositoy">Identificador del repositorio</param>
        /// <returns>Lista de tareas recurrentes</returns>
        public List<RecurringJobViewModel> GetRecurringJobsOfRepo(Guid idRepositoy)
        {
            string result = _serviceApi.CallGetApi("", $"RecurringJob/repository/{idRepositoy.ToString()}", _token);
            List<RecurringJobViewModel> resultObject = JsonConvert.DeserializeObject<List<RecurringJobViewModel>>(result);
            return resultObject;
        }

        /// <summary>
        /// Obtiene las tareas programadas de un repositorio OAIPMH
        /// </summary>
        /// <param name="idRepositoy">Identificador del repositorio</param>
        /// <returns>Lista de tareas programadas</returns>
        public List<ScheduledJobViewModel> GetScheduledJobsOfRepo(Guid idRepositoy)
        {
            string result = _serviceApi.CallGetApi("", $"ScheduledJob/repository/{idRepositoy.ToString()}", _token);
            List<ScheduledJobViewModel> resultObject = JsonConvert.DeserializeObject<List<ScheduledJobViewModel>>(result);
            return resultObject;
        }
    }
}
