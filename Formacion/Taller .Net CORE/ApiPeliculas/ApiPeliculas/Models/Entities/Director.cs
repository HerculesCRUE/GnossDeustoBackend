using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Entities
{
    public class Director
    {
        [Key]
        public Guid Director_ID { get; set; }
        [ForeignKey("Person")]
        public Guid Person_ID { get; set; }

        public ICollection<Film> Films { get; set; }
    }
}
