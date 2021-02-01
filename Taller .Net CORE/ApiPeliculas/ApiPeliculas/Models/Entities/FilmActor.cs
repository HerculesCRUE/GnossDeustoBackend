using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Entities
{
    public class FilmActor
    {
        public Guid Film_ID { get; set; }
        public Guid Actor_ID { get; set; }
        public Film Film { get; set; }
        public Actor Actor { get; set; }
    }
}
