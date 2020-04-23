using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("list", Schema = "hangfire")]
    public partial class List
    {
        [Column(Order = 0)]
        public long Id { get; set; }

        [Column(Order = 1)]
        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}
