// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar los distintos tipos de tareas
using System;
using System.Collections.Generic;
using System.Linq;
using CronConfigure.Models.Entitties;
using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using NCrontab;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase para gestionar los distintos tipos de tareas
    ///</summary>
    public class CronApiService : ICronApiService
    {
        private HangfireEntityContext _context;
        public CronApiService(HangfireEntityContext context)
        {
            _context = context;
        }

        ///<summary>
        ///Obtiene una lista de tareas recurrentes
        ///</summary>
        public List<RecurringJobViewModel> GetRecurringJobs()
        {
            List<RecurringJobDto> recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            List<RecurringJobViewModel> recurringJobsView = new List<RecurringJobViewModel>();
            foreach (RecurringJobDto recurringJob in recurringJobs)
            {
                RecurringJobViewModel recurringJobViewModel = new RecurringJobViewModel()
                {
                    CreatedAt = recurringJob.CreatedAt,
                    Cron = recurringJob.Cron,
                    Error = recurringJob.Error,
                    Id = recurringJob.Id,
                    LastExecution = recurringJob.LastExecution,
                    LastJobId = recurringJob.LastJobId,
                    LastJobState = recurringJob.LastJobState,
                    NextExecution = recurringJob.NextExecution,
                    Queue = recurringJob.Queue,
                    Removed = recurringJob.Removed,
                    TimeZoneId = recurringJob.TimeZoneId
                };
                recurringJobsView.Add(recurringJobViewModel);
            }
            return recurringJobsView;
        }

        ///<summary>
        ///Comprueba que exista una tarea
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public bool ExistJob(string id)
        {
            return _context.Job.Any(item => item.Id.ToString().Equals(id));
        }

        ///<summary>
        ///Elimina una tarea
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public void DeleteJob(string id)
        {
            BackgroundJob.Delete(id);
            var rel = _context.JobRepository.FirstOrDefault(item => item.IdJob.Equals(id));
            if (rel == null)
            {
                var list = _context.JobRepository.Where(item => item.IdJob.Contains($"{id}_") && item.FechaEjecucion > DateTime.Now).ToList();
                if (list.Count>1)
                {
                    foreach(var job in list)
                    {
                        if (job.IdJob.Split("_")[0].Equals(id))
                        {
                            rel = job;
                        }
                    }
                }
                else
                {
                    rel = list[0];
                }
            }
            _context.Entry(rel).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _context.SaveChanges();
        }

        ///<summary>
        ///Elimina una tarea recurrente
        ///</summary>
        ///<param name="id">nombre de la tarea</param>
        public void DeleteRecurringJob(string id)
        {
            RecurringJob.RemoveIfExists(id);
            JobRepository rel = null;
            var list = _context.JobRepository.Where(item => item.IdJob.Contains($"_{id}") && item.FechaEjecucion < DateTime.Now).ToList();
            foreach(var job in list)
            {
                var parts = job.IdJob.Split("_");
                string jobID = parts[1];
                if (parts.Length > 2)
                {
                    var correct = CrontabSchedule.TryParse(parts[parts.Length - 1]);
                    if (correct != null)
                    {
                        int partsNum = parts.Length;
                        for (int i = 1; i < partsNum - 1; i++)
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
                if (jobID.Equals(id))
                {
                    rel = job;
                }
            }
            _context.Entry(rel).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _context.SaveChanges();
        }

        ///<summary>
        ///Pone en la cola una tarea una tarea
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public void EnqueueJob(string id)
        {
            BackgroundJob.Requeue(id);
        }

        ///<summary>
        ///Obtiene una tarea recurrente
        ///</summary>
        ///<param name="id">nombre de la tarea recurrente</param>
        public RecurringJobViewModel GetRecurringJobs(string id)
        {
            RecurringJobViewModel recurringJob = GetRecurringJobs().FirstOrDefault(item => item.Id.Equals(id));
            return recurringJob;
        }

        ///<summary>
        ///Obtiene una lista de tareas ejecutadas
        ///</summary>
        ///<param name="type">tipo de la tarea</param>
        /// <param name="from">número desde el cual se va a traer las tareas del listado, por defecto 0 para empezar a traer desde el primer elemento de la lista de tareas</param>
        /// <param name="count">número máximo de tareas a traer</param>
        public List<JobViewModel> GetJobs(JobType type,int from, int count)
        {
            List<JobViewModel> listJobViewModel = new List<JobViewModel>();
            var api = JobStorage.Current.GetMonitoringApi();
            if (type.Equals(JobType.All) || type.Equals(JobType.Succeeded))
            { 
                foreach (var succeeded in api.SucceededJobs(from, count))
                {
                    JobViewModel job = new JobViewModel()
                    {
                        Id = succeeded.Key,
                        Job = succeeded.Value.Job.ToString(),
                        State = "Succeed",
                        ExecutedAt = succeeded.Value.SucceededAt
                    };
                    listJobViewModel.Add(job);
                }
            }
            if (type.Equals(JobType.All) || type.Equals(JobType.Failed))
            {
                foreach (var failed in api.FailedJobs(from, count))
                {
                    string name = "";
                    if (failed.Value.Job != null)
                    {
                        name = failed.Value.Job.ToString();
                    }

                    JobViewModel job = new JobViewModel()
                    {
                        Id = failed.Key,
                        ExceptionDetails = failed.Value.ExceptionDetails,
                        Job = name,
                        State = "Fail",
                        ExecutedAt = failed.Value.FailedAt
                    };
                    listJobViewModel.Add(job);
                }
            }
            listJobViewModel = listJobViewModel.OrderByDescending(item => item.State).ToList();
            return listJobViewModel;
        }


        ///<summary>
        ///Obtiene una lista de tareas programadas
        ///</summary>
        /// <param name="from">número desde el cual se va a traer las tareas del listado, por defecto 0 para empezar a traer desde el primer elemento de la lista de tareas</param>
        /// <param name="count">número máximo de tareas a traer</param>
        public List<ScheduledJobViewModel> GetScheduledJobs(int from, int count)
        {
            var api = JobStorage.Current.GetMonitoringApi();
            JobList<ScheduledJobDto> listScheduled = api.ScheduledJobs(from, count);
            List<ScheduledJobViewModel> listScheduledViewModel = new List<ScheduledJobViewModel>();
            foreach (var scheduled in listScheduled)
            {
                ScheduledJobViewModel scheduledViewModel = new ScheduledJobViewModel()
                {
                    Key = scheduled.Key,
                    EnqueueAt = scheduled.Value.EnqueueAt,
                    InScheduledState = scheduled.Value.InScheduledState,
                    Job = scheduled.Value.Job.ToString(),
                    ScheduledAt = scheduled.Value.ScheduledAt
                };
                listScheduledViewModel.Add(scheduledViewModel);
            }
            return listScheduledViewModel;
        }

        ///<summary>
        ///Obtiene una lista de tareas ejecutadas a partir de una tarea recurrente
        ///</summary>
        ///<param name="recurringJob">nombre de la tarea recurrente</param>
        public List<JobViewModel> GetJobsOfRecurringJob(string recurringJob)
        {
            recurringJob = $"\"{recurringJob}\"";
            var ids = _context.JobParameter.Where(item => item.Value.Equals(recurringJob)).Select(item => item.JobId).Distinct().ToList();
            List<JobViewModel> listJobViewModel = new List<JobViewModel>();
            var api = JobStorage.Current.GetMonitoringApi();
            foreach (long id in ids)
            {
                var jobDetails = api.JobDetails(id.ToString());
                string state = "";
                if (jobDetails != null)
                {
                    if (jobDetails.History.Count > 0)
                    {
                        state = jobDetails.History[0].StateName;
                    }
                    JobViewModel job = new JobViewModel()
                    {
                        Id = id.ToString(),
                        Job = jobDetails.Job.ToString(),
                        State = state
                    };
                    listJobViewModel.Add(job);
                }
            }
            return listJobViewModel;

        }
        ///<summary>
        ///Comprueba si existe una tarea recurrente
        ///</summary>
        ///<param name="recurringJob">nombre de la tarea recurrente</param>
        public bool ExistRecurringJob(string recurringJob)
        {
            return _context.Set.Any(item => item.Key.Equals("recurring-jobs") && item.Value.Equals(recurringJob));
        }
        ///<summary>
        ///Comprueba si existe una tarea programada
        ///</summary>
        ///<param name="id">identificador de la tarea programada</param>
        public bool ExistScheduledJob(string id)
        {
            return _context.Set.Any(item => item.Key.Equals("schedule") && item.Value.Equals(id));
        }

        ///<summary>
        ///Obtiene los datos de una tarea concreta
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public JobViewModel GetJob(string id)
        {
            var api = JobStorage.Current.GetMonitoringApi();
            var jobDto = api.JobDetails(id);
            JobViewModel job = null;
            string state = "";
            
            if (jobDto != null)
            {
                job = new JobViewModel();
                if (jobDto.History.Count > 0)
                {
                    state = jobDto.History[0].StateName;
                    if (state.Equals("Failed"))
                    {
                        if (jobDto.History[0].Data.ContainsKey("ExceptionMessage"))
                        {
                            job.ExceptionDetails = jobDto.History[0].Data["ExceptionMessage"];
                        }
                        else
                        {
                            job.ExceptionDetails = jobDto.History[0].Reason;
                        }
                    }
                    job.ExecutedAt = jobDto.History[0].CreatedAt;
                }
                
                job.Id = id;
                if (jobDto.Job != null) 
                {
                    job.Job = jobDto.Job.ToString();
                }
                job.State = state;
               //ob.ExecutedAt = jobDto.
            }
            return job;
        }

    }
}
