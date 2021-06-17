// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada por Hangfire para el guardado de las tareas
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Hangfire
{
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    [ExcludeFromCodeCoverage]
    [Table("job", Schema = "hangfire")]
    public partial class Job
    {
        /// <summary>
        /// Job
        /// </summary>
        public Job()
        {
            JobParameter = new HashSet<JobParameter>();
            State = new HashSet<State>();
        }

        /// <summary>
        /// Id
        /// </summary>
        [Column("id")]
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// StateId
        /// </summary>
        [Column("stateid")]
        public long? StateId { get; set; }

        /// <summary>
        /// StateName
        /// </summary>
        [Column("statename")]
        [StringLength(20)]
        public string StateName { get; set; }

        /// <summary>
        /// InvocationData
        /// </summary>
        [Column("invocationdata")]
        [Required]
        public string InvocationData { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        [Column("arguments")]
        [Required]
        public string Arguments { get; set; }

        /// <summary>
        /// CreatedAt
        /// </summary>
        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ExpireAt
        /// </summary>
        [Column("expireat")]
        public DateTime? ExpireAt { get; set; }

        /// <summary>
        /// JobParameter
        /// </summary>
        [Column("jobparameter")]
        public virtual ICollection<JobParameter> JobParameter { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [Column("state")]
        public virtual ICollection<State> State { get; set; }
    }
}
