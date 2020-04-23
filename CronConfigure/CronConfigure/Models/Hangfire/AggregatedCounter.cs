using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("aggregatedcounter", Schema="hangfire")]
    public partial class AggregatedCounter
    {
        [Key]
        [StringLength(100)]
        public string Key { get; set; }

        public long Value { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}
