using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ViewModel
{
    public class PublishRepositoryModel
    {
        [Required]
        public Guid repository_identifier { get; set; }
        public DateTime? fecha_from { get; set; }
        public string set { get; set; }
        //public string codigo_objeto { get; set; }

    }
}
