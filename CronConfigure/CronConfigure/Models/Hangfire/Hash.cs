using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("hash", Schema = "hangfire")]
    public partial class Hash
    {
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        [Column(Order = 1)]
        [StringLength(100)]
        public string Field { get; set; }

        public string Value { get; set; }

        //[Column(TypeName = "datetime2")]
        public DateTime? ExpireAt { get; set; }
    }
}
