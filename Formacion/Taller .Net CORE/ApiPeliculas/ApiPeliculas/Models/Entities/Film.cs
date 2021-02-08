using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Entities
{
    public class Film
    {

        [Key]
        public Guid Film_ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Year { get; set; }
        public string Released { get; set; }
        [Required]
        public int MinuteRunTime { get; set; }
        [ForeignKey("Director")]
        [Required]
        public Guid Director_ID { get; set; }
        public  virtual Director Director { get; set; }
        public virtual ICollection<FilmActor> Actors { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
