using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("set", Schema = "hangfire")]
    public partial class Set
    {
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        public double Score { get; set; }

        [Column(Order = 1)]
        [StringLength(256)]
        public string Value { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}
