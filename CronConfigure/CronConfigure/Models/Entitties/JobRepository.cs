using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Entitties
{
    [Table("JobRepository", Schema = "hangfire")]
    public class JobRepository
    {
        [Key]
        public string IdJob { get; set; }
        [Required]
        public Guid IdRepository { get; set; }
        public string Tipo { get; set; }
    }
}
