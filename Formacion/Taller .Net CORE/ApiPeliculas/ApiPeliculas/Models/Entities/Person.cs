using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Entities
{
    public class Person
    {
        [Key]
        public Guid Person_ID { get; set; }
        public string Name { get; set; }
    }
}
