using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryConfigSolution.Models.Entities
{
    public class RepositoryConfig
    {
        [Key]
        public Guid RepositoryConfigID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime? LastEjecutionDate { get; set; }
        public DateTime NextEjecutionDate { get; set; }
        public string Periodicity { get; set; }
    }
}
