using CronConfigure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    public interface IRepositoryCronService
    {
        public List<RecurringJobViewModel> GetRecurringJobs(Guid repositoryID);
        public List<JobViewModel> GetJobs(Guid repositoryID);
        public List<ScheduledJobViewModel> GetScheduledJobs(Guid repositoryID);
        public List<JobViewModel> GetJobsOfRecurringJob(Guid repositoryID);
        public List<JobViewModel> GetAllJobs(Guid repositoryID);
    }
}
