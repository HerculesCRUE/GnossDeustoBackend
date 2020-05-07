using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class CreateJobViewModel
    {
        [Display(Name = "ID repositorio")]
        public Guid IdRepository { get; set; }
        [Display(Name = "fecha de inicio")]
        public DateTime? FechaIinicio { get; set; }
        [Display(Name = "Fecha desde la que hay que hacer la sincronización")]
        public DateTime? FechaFrom { get; set; }
        [Display(Name = "set")]
        public string Set { get; set; }
        [Display(Name = "nombre del trabajo")]
        public string Nombre_job { get; set; }
        [Display(Name = "nexpresión de cron: * * * * * (minuto hora día del mes mes día de la semana)")]
        public string CronExpression { get; set; }
    }
}
