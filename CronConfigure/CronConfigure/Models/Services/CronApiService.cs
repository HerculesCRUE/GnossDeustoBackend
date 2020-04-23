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

        internal void DeleteJob(string id)
        {
            BackgroundJob.Delete(id);
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

        public List<JobViewModel> GetJobs(JobType type)
        {
            List<JobViewModel> listJobViewModel = new List<JobViewModel>();
            var api = JobStorage.Current.GetMonitoringApi();
            if (type.Equals(JobType.All) || type.Equals(JobType.Succeeded))
            { 
                foreach (var succeeded in api.SucceededJobs(0, 100))
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
                foreach (var failed in api.FailedJobs(0, 100))
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

        public List<ScheduledJobViewModel> GetScheduledJobs()
        {
            var api = JobStorage.Current.GetMonitoringApi();
            JobList<ScheduledJobDto> listScheduled = api.ScheduledJobs(0, 100);
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

    }
}
