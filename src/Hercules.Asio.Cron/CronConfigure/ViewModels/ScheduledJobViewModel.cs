// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para mostrar los datos de una tarea programada
using System;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.ViewModels
{
    ///<summary>
    ///Clase que sirve para mostrar los datos de una tarea programada
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class ScheduledJobViewModel
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Job
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// EnqueueAt
        /// </summary>
        public DateTime EnqueueAt { get; set; }
        /// <summary>
        /// InScheduledState
        /// </summary>
        public bool InScheduledState { get; set; }
        /// <summary>
        /// ScheduledAt
        /// </summary>
        public DateTime? ScheduledAt { get; set; }
    }
}
