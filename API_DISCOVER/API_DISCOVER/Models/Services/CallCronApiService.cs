// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas a los métodos del apiCron
using API_DISCOVER.Models.Entities;
using API_DISCOVER.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_DISCOVER.Models.Services
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


    }
}
