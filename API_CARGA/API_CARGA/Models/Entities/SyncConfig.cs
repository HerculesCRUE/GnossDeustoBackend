using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Datos de configuración de una sincronización
    /// </summary>
    public class SyncConfig
    {
        /// <summary>
        /// Identificador de la sincronización
        /// </summary>
        public string identifier { get; set; }
        /// <summary>
        /// Nombre de la sincronización
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Hora de inicio, en formato 00:00
        /// </summary>
        public string startHour { get; set; }
        /// <summary>
        /// Frecuencia de la actualización en segundos
        /// </summary>
        public int updateFrequency { get; set; }
        /// <summary>
        /// Identificador del repositorio del que se recuperarán los datos
        /// </summary>
        public string repositoryIdentifier { get; set; }
        /// <summary>
        /// Identificador de los sets del repositorio que se recuperarán en la sincronización
        /// </summary>
        public List<string> repositorySetIdentifiers { get; set; }
    }
}
