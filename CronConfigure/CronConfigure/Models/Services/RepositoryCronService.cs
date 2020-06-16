using CronConfigure.ViewModels;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    public class RepositoryCronService: IRepositoryCronService
    {
        private HangfireEntityContext _context;
        public ICronApiService _cronApiService;
        public RepositoryCronService(HangfireEntityContext context, ICronApiService cronApiService)
        {
            _context = context;
            _cronApiService = cronApiService;
        }
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
