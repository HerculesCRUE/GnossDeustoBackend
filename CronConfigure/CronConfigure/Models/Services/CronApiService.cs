using System;
using System.Collections.Generic;
using System.Linq;
using CronConfigure.Models;
using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using NCrontab;

namespace CronConfigure.Models.Services
{
    public class CronApiService
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
            _context.Entry(rel).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _context.SaveChanges();
        }

        public void DeleteRecurringJob(string id)
        {
            RecurringJob.RemoveIfExists(id);
            var rel = _context.JobRepository.FirstOrDefault(item => item.IdJob.Equals(id));
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
                        State = "Succeed"
                    };
                    listJobViewModel.Add(job);
                }
            }
            if (type.Equals(JobType.All) || type.Equals(JobType.Failed))
            {
                foreach (var failed in api.FailedJobs(from, count))
                {
                    JobViewModel job = new JobViewModel()
                    {
                        Id = failed.Key,
                        Job = failed.Value.Job.ToString(),
                        State = "Fail"
                    };
                    listJobViewModel.Add(job);
                }
            }
            listJobViewModel = listJobViewModel.OrderBy(item => item.State).ToList();
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
            return listJobViewModel;

        }

        public bool ExistRecurringJob(string recurringJob)
        {
            return _context.Set.Any(item => item.Key.Equals("recurring-jobs") && item.Value.Equals(recurringJob));
        }

        internal bool ExistScheduledJob(string id)
        {
            return _context.Set.Any(item => item.Key.Equals("schedule") && item.Value.Equals(id));
        }

    }
}
