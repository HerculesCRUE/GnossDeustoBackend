// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para mostrar los datos de una tarea recurrente
using System;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.ViewModels
{
    ///<summary>
    ///Clase que sirve para mostrar los datos de una tarea recurrente
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class RecurringJobViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Cron
        /// </summary>
        public string Cron { get; set; }
        /// <summary>
        /// Queue
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// NextExecution
        /// </summary>
        public DateTime? NextExecution { get; set; }
        /// <summary>
        /// LastJobId
        /// </summary>
        public string LastJobId { get; set; }
        /// <summary>
        /// LastJobState
        /// </summary>
        public string LastJobState { get; set; }
        /// <summary>
        /// LastExecution
        /// </summary>
        public DateTime? LastExecution { get; set; }
        /// <summary>
        /// CreatedAt
        /// </summary>
        public DateTime? CreatedAt { get; set; }
        /// <summary>
        /// Removed
        /// </summary>
        public bool Removed { get; set; }
        /// <summary>
        /// TimeZoneId
        /// </summary>
        public string TimeZoneId { get; set; }
        /// <summary>
        /// Error
        /// </summary>
        public string Error { get; set; }
    }
}
