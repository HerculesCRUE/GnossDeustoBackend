using CronConfigure.Models.Enumeracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models
{
    public class Recurrent
    {
        public JobType DateType { get; set; }

        public int Interval { get; set; }

        public Recurrent(JobType dateType, int interval)
        {
            DateType = dateType;
            Interval = interval;
        }
    }
}
