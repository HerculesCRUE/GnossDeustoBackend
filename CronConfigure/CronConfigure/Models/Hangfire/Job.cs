// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada por Hangfire para el guardado de las tareas
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CronConfigure.Models.Hangfire
{
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>

    [Table("job", Schema = "hangfire")]
    public partial class Job
    {
        public Job()
        {
            JobParameter = new HashSet<JobParameter>();
            State = new HashSet<State>();
        }
        [Column("id")]
        [Key]
        public long Id { get; set; }

        [Column("stateid")]
        public long? StateId { get; set; }

        [Column("statename")]
        [StringLength(20)]
        public string StateName { get; set; }

        [Column("invocationdata")]
        [Required]
        public string InvocationData { get; set; }

        [Column("arguments")]
        [Required]
        public string Arguments { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("expireat")]
        public DateTime? ExpireAt { get; set; }

        [Column("jobparameter")]
        public virtual ICollection<JobParameter> JobParameter { get; set; }

        [Column("state")]
        public virtual ICollection<State> State { get; set; }
    }
}
