// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para mostrar los datos de una tarea programada
using System;

namespace CronConfigure.ViewModels
{
    ///<summary>
    ///Clase que sirve para mostrar los datos de una tarea programada
    ///</summary>
    public class ScheduledJobViewModel
    {
        public string Key { get; set; }
        public string Job { get; set; }
        public DateTime EnqueueAt { get; set; }
        public bool InScheduledState { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
