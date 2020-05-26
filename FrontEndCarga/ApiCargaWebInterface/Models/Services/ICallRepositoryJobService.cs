using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallRepositoryJobService
    {
        public List<RecurringJobViewModel> GetRecurringJobsOfRepo(Guid idRepositoy);
        public List<JobViewModel> GetJobsOfRepo(Guid idRepositoy);
        public List<ScheduledJobViewModel> GetScheduledJobsOfRepo(Guid idRepositoy);
    }
}
