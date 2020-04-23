using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Hangfire
{

    [Table("schema", Schema = "hangfire")]
    public partial class Schema
    {
        [Key]
        public int Version { get; set; }
    }
}
