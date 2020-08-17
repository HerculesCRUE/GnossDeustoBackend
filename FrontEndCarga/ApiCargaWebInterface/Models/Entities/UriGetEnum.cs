using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Entities
{
    public enum UriGetEnum
    {
        [Display(Name = "Resource class")]
        Resource_class = 0,
        [Display(Name = "RDFType")]
        RDFtype = 1
    }
}
