// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve de mock del CronApiService para la realización de los test
using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase que sirve de mock del CronApiService para la realización de los test
    ///</summary>
    public class MockCronApiService : ICronApiService
    {
        /// <summary>
        /// DeleteJob
        /// </summary>
        /// <param name="id"></param>
        public void DeleteJob(string id)
        {
           // Método sobreescrito.
        }

        /// <summary>
        /// DeleteRecurringJob
        /// </summary>
        /// <param name="id"></param>
        public void DeleteRecurringJob(string id)
        {
            // Método sobreescrito.
        }

        /// <summary>
        /// EnqueueJob
        /// </summary>
        /// <param name="id"></param>
        public void EnqueueJob(string id)
        {
            // Método sobreescrito.
        }

        /// <summary>
        /// ExistJob
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ExistRecurringJob
        /// </summary>
        /// <param name="recurringJob"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ExistScheduledJob
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// GetJob
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public JobViewModel GetJob(string id)
        {
            return new JobViewModel();
        }

        /// <summary>
        /// GetJobData
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public JobViewModel GetJobData(string id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// GetJobs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public List<JobViewModel> GetJobs(JobType type, int from, int count)
        {
            return new List<JobViewModel>();
        }

        /// <summary>
        /// GetJobsOfRecurringJob
        /// </summary>
        /// <param name="recurringJob"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public List<JobViewModel> GetJobsOfRecurringJob(string recurringJob)
        {
            return new List<JobViewModel>();
        }

        /// <summary>
        /// GetRecurringJobs
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public List<RecurringJobViewModel> GetRecurringJobs()
        {
            return new List<RecurringJobViewModel>();
        }

        /// <summary>
        /// GetRecurringJobs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public RecurringJobViewModel GetRecurringJobs(string id)
        {
            return new RecurringJobViewModel();
        }

        /// <summary>
        /// GetScheduledJobs
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ScheduledJobViewModel> GetScheduledJobs(int from, int count)
        {
            return new List<ScheduledJobViewModel>();
        }
    }
}
