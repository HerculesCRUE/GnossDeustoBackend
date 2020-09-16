using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class PageViewModel
    {
        public Guid PageId { get; set; }
        public string Route { get; set; }
        public IFormFile FileHtml { get; set; }
        public DateTime LastModified { get; set; }
    }
}
