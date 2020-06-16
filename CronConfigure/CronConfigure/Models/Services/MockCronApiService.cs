// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve de mock del CronApiService para la realización de los test
using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using System.Collections.Generic;

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

        public JobViewModel GetJob(string id)
        {
            return new JobViewModel();
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
