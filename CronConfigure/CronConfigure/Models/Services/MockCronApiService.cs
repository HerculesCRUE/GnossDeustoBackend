using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase que sirve de mock del CronApiService para la realización de los test
    ///</summary>
    public class MockCronApiService : ICronApiService
    {
        public void DeleteJob(string id)
        {
           
        }

        public void DeleteRecurringJob(string id)
        {
            
        }

        public void EnqueueJob(string id)
        {
            
        }

        public bool ExistJob(string id)
        {
            if (id != "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExistRecurringJob(string recurringJob)
        {
            if (recurringJob == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExistScheduledJob(string id)
        {
            if (id != "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<JobViewModel> GetJobs(JobType type, int from, int count)
        {
            return new List<JobViewModel>();
        }

        public List<JobViewModel> GetJobsOfRecurringJob(string recurringJob)
        {
            return new List<JobViewModel>();
        }

        public List<RecurringJobViewModel> GetRecurringJobs()
        {
            return new List<RecurringJobViewModel>();
        }

        public RecurringJobViewModel GetRecurringJobs(string id)
        {
            return new RecurringJobViewModel();
        }

        public List<ScheduledJobViewModel> GetScheduledJobs(int from, int count)
        {
            return new List<ScheduledJobViewModel>();
        }
    }
}
