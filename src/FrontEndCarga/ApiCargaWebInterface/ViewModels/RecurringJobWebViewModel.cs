using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class RecurringJobWebViewModel
    {
        public RecurringJobViewModel RecurringJobViewModel { get; set; }
        public List<JobViewModel> ListJobs { get; set; }
    }
}
