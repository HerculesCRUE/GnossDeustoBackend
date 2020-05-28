using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
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
        public List<JobViewModel> GetJobsOfRepo(Guid idRepositoy)
        {
            string result = _serviceApi.CallGetApi($"Job/repository/{idRepositoy.ToString()}", _token);
            List<JobViewModel> resultObject = JsonConvert.DeserializeObject<List<JobViewModel>>(result);
            return resultObject;
        }

        public List<RecurringJobViewModel> GetRecurringJobsOfRepo(Guid idRepositoy)
        {
            string result = _serviceApi.CallGetApi($"RecurringJob/repository/{idRepositoy.ToString()}", _token);
            List<RecurringJobViewModel> resultObject = JsonConvert.DeserializeObject<List<RecurringJobViewModel>>(result);
            return resultObject;
        }

        public List<ScheduledJobViewModel> GetScheduledJobsOfRepo(Guid idRepositoy)
        {
            string result = _serviceApi.CallGetApi($"ScheduledJob/repository/{idRepositoy.ToString()}", _token);
            List<ScheduledJobViewModel> resultObject = JsonConvert.DeserializeObject<List<ScheduledJobViewModel>>(result);
            return resultObject;
        }
    }
}
