using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    [Display(Name = "recurring job")]
    public class CreateRecurringJobViewModel
    {
        [Display(Name = "ID repositorio")]
        public string IdRepository { get; set; }
        [Display(Name = "nombre del trabajo")]
        public string Nombre_job { get; set; }
        [Display(Name = "fecha de inicio")]
        public DateTime FechaIinicio { get; set; }
        [Display(Name = "nexpresión de cron")]
        public string CronExpression { get; set; }
    }
}
