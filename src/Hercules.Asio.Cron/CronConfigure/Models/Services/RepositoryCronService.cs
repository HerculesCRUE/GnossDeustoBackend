// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la información de las tareas vinculadas a un repositorio
using CronConfigure.ViewModels;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase para obtener la información de las tareas vinculadas a un repositorio
    ///</summary>
    public class RepositoryCronService: IRepositoryCronService
    {
        private HangfireEntityContext _context;
        public ICronApiService _cronApiService;
        public RepositoryCronService(HangfireEntityContext context, ICronApiService cronApiService)
        {
            _context = context;
            _cronApiService = cronApiService;
        }

        ///<summary>
        ///Obtiene las tareas recurrentes de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas recurrentes</returns>
        public List<RecurringJobViewModel> GetRecurringJobs(Guid repositoryID)
        {
            List<string> recurringJobsId = _context.JobRepository.Where(item => item.IdRepository.Equals(repositoryID) && item.FechaEjecucion < DateTime.Now && item.IdJob.Contains("_")).Select(item => item.IdJob).ToList();
            List<RecurringJobViewModel> recurringJobs = new List<RecurringJobViewModel>();
            foreach (string id in recurringJobsId)
            {
                if (id.Contains("_")) 
                {
                    
                    var parts = id.Split("_");
                    string jobID = parts[1];
                    if (parts.Length > 2)
                    {
                        var correct = CrontabSchedule.TryParse(parts[parts.Length-1]);
                        if (correct != null)
                        {
                            int partsNum = parts.Length;
                            for(int i = 1; i < partsNum-1; i++)
                            {
                                if (i ==1)
                                {
                                    jobID = $"{parts[i]}";
                                }
                                else
                                {
                                    jobID = $"{jobID}_{parts[i]}";
                                }
                            }
                        }
                        else
                        {
                            int partsNum = parts.Length;
                            for (int i = 1; i < partsNum; i++)
                            {
                                if (i == 1)
                                {
                                    jobID = $"{parts[i]}";
                                }
                                else
                                {
                                    jobID = $"{jobID}_{parts[i]}";
                                }
                            }
                        }

                    }
                    var recurringJob = _cronApiService.GetRecurringJobs(jobID);
                    if (recurringJob != null)
                    {
                        recurringJobs.Add(recurringJob);
                    }
                }
            }
            return recurringJobs;
        }

        ///<summary>
        ///Obtiene las tareas de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas</returns>
        public List<JobViewModel> GetJobs(Guid repositoryID)
        {
            List<JobViewModel> jobs = new List<JobViewModel>();
            List<string> jobsId = _context.JobRepository.Where(item => item.IdRepository.Equals(repositoryID) && item.FechaEjecucion < DateTime.Now && !item.IdJob.Contains("_")).Select(item => item.IdJob).ToList();
            foreach(string id in jobsId)
            {
                var job = _cronApiService.GetJob(id);
                if (job != null)
                {
                    jobs.Add(job);
                }
            }
            return jobs.OrderByDescending(item => item.ExecutedAt).ToList();
        }

        ///<summary>
        ///Obtiene las tareas programadas de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas programadas</returns>
        public List<ScheduledJobViewModel> GetScheduledJobs(Guid repositoryID)
        {
            List<ScheduledJobViewModel> scheduledJobs = new List<ScheduledJobViewModel>();
            List<string> jobsId = _context.JobRepository.Where(item => item.IdRepository.Equals(repositoryID) && item.FechaEjecucion > DateTime.Now).Select(item => item.IdJob).ToList();
           foreach(string idJob in jobsId)
            {
                string idScheduled = idJob;
                if (idScheduled.Contains("_"))
                {
                    idScheduled = idScheduled.Split("_")[0];
                }
                scheduledJobs.Add(_cronApiService.GetScheduledJobs(0, 200).First(item => item.Key.Equals(idScheduled)));
            }
            return scheduledJobs;
        }

        ///<summary>
        ///Obtiene las tareas ejecutadas a partir de todas las tareas recurrentes vinculadas a un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas</returns>
        public List<JobViewModel> GetJobsOfRecurringJob(Guid repositoryID)
        {
            List<RecurringJobViewModel>  listRecurring = GetRecurringJobs(repositoryID);
            List<JobViewModel> listJobs = new List<JobViewModel>();
            foreach (var recurring in listRecurring)
            {
                var listJobsOfRecurringJob = _cronApiService.GetJobsOfRecurringJob(recurring.Id);
                foreach (var job in listJobsOfRecurringJob)
                {
                    listJobs.Add(job);
                }
            }
            return listJobs;
        }

        ///<summary>
        ///Obtiene las tareas recurrentes y las tareas de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas</returns>
        public List<JobViewModel> GetAllJobs(Guid repositoryID)
        {
            List<JobViewModel> listAllJobs = new List<JobViewModel>();
            List<JobViewModel> listJobs = GetJobs(repositoryID);
            List<JobViewModel> listJobsOfRecurring = GetJobsOfRecurringJob(repositoryID);
            listAllJobs = listJobs.Union(listJobsOfRecurring).OrderByDescending(item => item.ExecutedAt).ToList();
            return listAllJobs;
        }
    }
}
