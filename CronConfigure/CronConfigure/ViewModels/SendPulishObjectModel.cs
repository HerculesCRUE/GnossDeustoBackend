using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.ViewModels
{
    public class SendPulishObjectModel
    {
        public Guid identifier { get; set; }
        public DateTime? fecha_from { get; set; }
        public string set { get; set; }
        public string codigo_objeto { get; set; }
    }
}
