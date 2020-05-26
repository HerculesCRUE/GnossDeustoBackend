using System;
using System.Collections.Generic;
using System.Linq;
using CronConfigure.Models;
using CronConfigure.Models.Entitties;
using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace CronConfigure.Models.Services
{
    public class CronApiService : ICronApiService
    {
        private HangfireEntityContext _context;
        public CronApiService(HangfireEntityContext context)
        {
            _context = context;
        }
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

        public bool ExistJob(string id)
        {
            return _context.Job.Any(item => item.Id.ToString().Equals(id));
        }

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

        public void EnqueueJob(string id)
        {
            BackgroundJob.Requeue(id);
        }

        public RecurringJobViewModel GetRecurringJobs(string id)
        {
            RecurringJobViewModel recurringJob = GetRecurringJobs().FirstOrDefault(item => item.Id.Equals(id));
            return recurringJob;
        }

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

        public bool ExistRecurringJob(string recurringJob)
        {
            return _context.Set.Any(item => item.Key.Equals("recurring-jobs") && item.Value.Equals(recurringJob));
        }

        public bool ExistScheduledJob(string id)
        {
            return _context.Set.Any(item => item.Key.Equals("schedule") && item.Value.Equals(id));
        }

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
                    job.ExecutedAt = jobDto.History[0].CreatedAt;
                }
                job.ExceptionDetails = "";
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
