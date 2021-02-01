using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Entities
{
    public class Rating
    {
        [Key]
        public Guid Rating_ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        [ForeignKey("Film")]
        public Guid Film_ID { get; set; }
       
    }
}
