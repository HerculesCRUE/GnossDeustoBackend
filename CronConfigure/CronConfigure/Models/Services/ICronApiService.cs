using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    public interface ICronApiService
    {
        public List<RecurringJobViewModel> GetRecurringJobs();
        public bool ExistJob(string id);
        public void DeleteJob(string id);
        public void DeleteRecurringJob(string id);
        public void EnqueueJob(string id);
        public RecurringJobViewModel GetRecurringJobs(string id);
        public List<JobViewModel> GetJobs(JobType type, int from, int count);
        public List<ScheduledJobViewModel> GetScheduledJobs(int from, int count);
        public List<JobViewModel> GetJobsOfRecurringJob(string recurringJob);
        public bool ExistRecurringJob(string recurringJob);
        public bool ExistScheduledJob(string id);
        public JobViewModel GetJob(string id);
    }
}
