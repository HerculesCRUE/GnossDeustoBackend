using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    [Table("jobparameter", Schema = "hangfire")]
    public partial class JobParameter
    {
        [Column("jobid",Order = 0)]
        public long JobId { get; set; }

        [Column("name", Order = 1)]
        [StringLength(40)]
        public string Name { get; set; }

        [Column("value", Order = 2)]
        public string Value { get; set; }

        public virtual Job Job { get; set; }
    }
}
