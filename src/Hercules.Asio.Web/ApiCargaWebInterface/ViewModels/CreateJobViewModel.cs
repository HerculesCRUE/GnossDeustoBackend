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
        [Display(Name = "Identificador del repositorio")]
        public Guid IdRepository { get; set; }
        [Display(Name = "Fecha de inicio de la tarea (ej: 20-5-2020)")]
        public DateTime? FechaIinicio { get; set; }
        [Display(Name = "(Opcional) Fecha de última sincronización (se obtendrán los datos actualizados en el repositorio a partir de esa fecha. Ej: 20-5-2018), en el caso de que este dato esté vacío se sincronizarán todos los datos, en el caso contrario se obtendrán los datos a sincronizar a partir de esa fecha")]
        public DateTime? FechaFrom { get; set; }
        [Display(Name = "(Opcional) set de objetos a obtener")]
        public string Set { get; set; }
        [Display(Name = "(Opcional) Codigo del objeto (especificar para sincronizar un único objeto del repositorio)")]
        public string CodigoObjeto { get; set; }
        [Display(Name = "Nombre de la tarea")]
        public string Nombre_job { get; set; }
        [Display(Name = "Expresión de cron: * * * * * (cada \"*\" se corresponde con minuto, hora, día del mes, mes y día de la semana, por ese orden. Ej: 0 8 * * 1 ejecutaría la tarea los lunes a las 8:00)")]
        public string CronExpression { get; set; }
    }
}
