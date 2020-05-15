using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class ShapeConfigEditModel
    {
        [Display(Name = "Identifier")]
        public Guid ShapeConfigID { get; set; }
        [Display(Name = "Nombre del shape")]
        public string Name { get; set; }
        [Display(Name = "identificador del repositorio")]
        public Guid RepositoryID { get; set; }
        public string Shape { get; set; }
        [Display(Name = "Archivo que define el shape")]
        public IFormFile ShapeFile { get; set; }
    }
}
