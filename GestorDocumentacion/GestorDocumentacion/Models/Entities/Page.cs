using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.Models.Entities
{
    public class Page
    {
        [Key]
        public Guid PageID { get; set; }
        public string Route { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
