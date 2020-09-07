using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Entities
{
    public class CheckSystemReport
    {
        public bool ApiCarga { get; set; }
        public bool ApiCron { get; set; }
        public bool IdentityServer { get; set; }
    }
}
