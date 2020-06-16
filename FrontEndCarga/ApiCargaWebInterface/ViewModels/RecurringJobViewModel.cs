using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class RecurringJobViewModel
    {
        public string Id { get; set; }
        public string Cron { get; set; }
        public string Queue { get; set; }
        public DateTime? NextExecution { get; set; }
        public string LastJobId { get; set; }
        public string LastJobState { get; set; }
        public DateTime? LastExecution { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool Removed { get; set; }
        public string TimeZoneId { get; set; }
        public string Error { get; set; }
    }
}
