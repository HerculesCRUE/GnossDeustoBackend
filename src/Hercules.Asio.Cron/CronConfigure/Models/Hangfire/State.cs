// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada por Hangfire para el guardado de las tareas
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Hangfire
{
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    [ExcludeFromCodeCoverage]
    [Table("state", Schema = "hangfire")]
    public partial class State
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(Order = 0)]
        public long Id { get; set; }

        /// <summary>
        /// JobId
        /// </summary>
        [Column(Order = 1)]
        public long JobId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// Reason
        /// </summary>
        [StringLength(100)]
        public string Reason { get; set; }

        /// <summary>
        /// CreatedAt
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Job
        /// </summary>
        public virtual Job Job { get; set; }
    }
}
