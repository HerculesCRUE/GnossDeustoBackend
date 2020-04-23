using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("jobqueue", Schema = "hangfire")]
    public partial class JobQueue
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        public long JobId { get; set; }

        [Column(Order = 1)]
        [StringLength(50)]
        public string Queue { get; set; }

        public DateTime? FetchedAt { get; set; }
    }
}
