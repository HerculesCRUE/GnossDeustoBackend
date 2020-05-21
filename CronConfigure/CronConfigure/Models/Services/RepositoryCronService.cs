using CronConfigure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    public class RepositoryCronService
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
            List<string> recurringJobsId = _context.JobRepository.Where(item => item.IdRepository.Equals(repositoryID) && item.FechaEjecucion < DateTime.Now).Select(item => item.IdJob).ToList();
            List<RecurringJobViewModel> recurringJobs = new List<RecurringJobViewModel>();
            foreach (string id in recurringJobsId)
            {
                var jobID = id.Split("_")[1];
                var recurringJob = _cronApiService.GetRecurringJobs(jobID);
                recurringJobs.Add(recurringJob);
            }
            return recurringJobs;
        }
    }
}
