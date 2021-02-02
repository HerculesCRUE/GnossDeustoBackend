using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Entities
{
    public class Actor 
    {
        [ForeignKey("Person")]
        public Guid Person_ID { get; set; }
        [Key]
        public Guid Actor_ID { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<FilmActor> Films { get; set; }
    }
}
