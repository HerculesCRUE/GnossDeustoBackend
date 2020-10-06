using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class PageViewModel
    {
        public Guid PageId { get; set; }
        [Display(Name = "Ruta de la página, debe empezar por /")]
        public string Route { get; set; }
        public string RouteProxyLess { get; set; }
        public IFormFile FileHtml { get; set; }
        public DateTime LastModified { get; set; }
    }
}
