using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Wrappers
{
    public class FilmWrapper
    {
        public Guid Film_ID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Released { get; set; }
        public int MinuteRunTime { get; set; }
        public Guid Director_ID { get; set; }
    }
}
