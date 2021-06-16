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
        [ExcludeFromCodeCoverage]
        //Exluido del analisis porque el metodo no se usa en ningun test pero debe estar porque hereda de una interfaz
        public JobViewModel GetJob(string id)
        {
            return new JobViewModel();
        }
        [ExcludeFromCodeCoverage]
        //Exluido del analisis porque el metodo no se usa en ningun test pero debe estar porque hereda de una interfaz
        public JobViewModel GetJobData(string id)
        {
            throw new System.NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        //Exluido del analisis porque el metodo no se usa en ningun test pero debe estar porque hereda de una interfaz
        public List<JobViewModel> GetJobs(JobType type, int from, int count)
        {
            return new List<JobViewModel>();
        }
        [ExcludeFromCodeCoverage]
        //Exluido del analisis porque el metodo no se usa en ningun test pero debe estar porque hereda de una interfaz
        public List<JobViewModel> GetJobsOfRecurringJob(string recurringJob)
        {
            return new List<JobViewModel>();
        }
        [ExcludeFromCodeCoverage]
        //Exluido del analisis porque el metodo no se usa en ningun test pero debe estar porque hereda de una interfaz
        public List<RecurringJobViewModel> GetRecurringJobs()
        {
            return new List<RecurringJobViewModel>();
        }
        [ExcludeFromCodeCoverage]
        //Exluido del analisis porque el metodo no se usa en ningun test pero debe estar porque hereda de una interfaz
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
