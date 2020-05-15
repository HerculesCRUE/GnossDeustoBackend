using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    [Display(Name = "Shape config")]
    public class ShapeConfigCreateModel
    {
        [Display(Name = "Identifier")]
        public Guid ShapeConfigID { get; set; }
        [Display(Name = "Nombre del shape")]
        public string Name { get; set; }
        [Display(Name = "Identificador del repositorio")]
        public Guid RepositoryID { get; set; }
        [Display(Name = "Archivo que define el shape")]
        public IFormFile ShapeFile { get; set; }
    }
}
