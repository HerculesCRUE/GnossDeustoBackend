using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{
    [Table("counter", Schema = "hangfire")]
    public class Count
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Value { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}
