using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("state", Schema = "hangfire")]
    public partial class State
    {
        [Column(Order = 0)]
        public long Id { get; set; }

        [Column(Order = 1)]
        public long JobId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Data { get; set; }

        public virtual Job Job { get; set; }
    }
}
