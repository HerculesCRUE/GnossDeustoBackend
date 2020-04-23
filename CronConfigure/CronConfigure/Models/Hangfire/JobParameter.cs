using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("jobparameter", Schema = "hangfire")]
    public partial class JobParameter
    {
        [Column(Order = 0)]
        public long JobId { get; set; }

        [Column(Order = 1)]
        [StringLength(40)]
        public string Name { get; set; }

        public string Value { get; set; }

        public virtual Job Job { get; set; }
    }
}
