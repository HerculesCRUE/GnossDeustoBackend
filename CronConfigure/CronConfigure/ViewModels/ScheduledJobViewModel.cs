using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.ViewModels
{
    public class ScheduledJobViewModel
    {
        public string Key { get; set; }
        public string Job { get; set; }
        public DateTime EnqueueAt { get; set; }
        public bool InScheduledState { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
