using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.ViewModels
{
    ///<summary>
    ///Clase que sirve para mostrar los datos de una tarea
    ///</summary>
    public class JobViewModel
    {
        public string Job { get; set; }
        public string State { get; set; }
        public string Id { get; set; }

        public string ExceptionDetails { get; set; }
    }
}
