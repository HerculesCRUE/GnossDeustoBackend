using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ApiCargaWebInterface.ViewModels
{
    [Display(Name = "Config repository")]
    public class RepositoryConfigViewModel
    {
        [Display(Name = "Identifier")]
        public Guid RepositoryConfigID { get; set; }

        public string Name { get; set; }

        [Display(Name = "Oauth token")]
        public string OauthToken { get; set; }

        public string Url { get; set; }
        [Display(Name = "Última tarea ejecutada")]
        public string LastJob { get; set; }
        [Display(Name = "Estado de la última tarea ejecutada")]
        public string LastState { get; set; }
        public double PorcentajeTareas { get; set; }
        [Display(Name = "Shapes Configurados")]
        public List<ShapeConfigViewModel> ShapeConfig { get; set; }
        [Display(Name = "Tareas recurrentes Configuradas")]
        public List<RecurringJobViewModel> ListRecurringJobs { get; set; }
        [Display(Name = "Tareas Ejecutadas")]
        public List<JobViewModel> ListJobs { get; set; }
        [Display(Name = "Tareas Programadas")]
        public List<ScheduledJobViewModel> ListScheduledJobs { get; set; }

    }
}
