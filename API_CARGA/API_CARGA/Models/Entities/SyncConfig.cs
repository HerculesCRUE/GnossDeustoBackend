using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Key]
        public Guid SyncConfigID { get; set; }
        /// <summary>
        /// Nombre de la sincronización
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Hora de inicio, en formato 00:00
        /// </summary>
        public string StartHour { get; set; }
        /// <summary>
        /// Frecuencia de la actualización en segundos
        /// </summary>
        public int UpdateFrequency { get; set; }
        /// <summary>
        /// Identificador del repositorio del que se recuperarán los datos
        /// </summary>
        public Guid RepositoryIdentifier { get; set; }
        /// <summary>
        /// Identificador de los sets del repositorio que se recuperarán en la sincronización
        /// </summary>
        public List<string> RepositorySetIdentifiers { get; set; }
    }
}
