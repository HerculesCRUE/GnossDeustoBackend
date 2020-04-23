using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("job", Schema = "hangfire")]
    public partial class Job
    {
        public Job()
        {
            JobParameter = new HashSet<JobParameter>();
            State = new HashSet<State>();
        }
        [Key]
        public long Id { get; set; }

        public long? StateId { get; set; }

        [StringLength(20)]
        public string StateName { get; set; }

        [Required]
        public string InvocationData { get; set; }

        [Required]
        public string Arguments { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpireAt { get; set; }

        public virtual ICollection<JobParameter> JobParameter { get; set; }

        public virtual ICollection<State> State { get; set; }
    }
}
