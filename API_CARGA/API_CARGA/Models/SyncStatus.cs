using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models
{
    /// <summary>
    /// Datos del estado de una sincronización
    /// </summary>
    public class SyncStatus
    {
        /// <summary>
        /// Estado de la sincronización
        /// </summary>
        public enum Status
        {
            started,
            stopped,
            disabled
        }

        /// <summary>
        /// Estado de la sincronización (started=arrancada,stopped=parada,disabled=deshabilitada)
        /// </summary>
        public Status status { get; set; }
        /// <summary>
        /// Fecha de la última ejecución de la sincronización
        /// </summary>
        public DateTime lastRunning { get; set; }
        /// <summary>
        /// Fecha de la última entidad actualizada por la sincronización
        /// </summary>
        public DateTime lastUpdate { get; set; }
    }
}
