using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiCargaWebInterface.ViewModels
{
    [Display(Name = "RepositoryConfigSet")]
    public class RepositoryConfigSetViewModel
    {
        [Display(Name = "Identifier")]
        public Guid RepositoryConfigSetID { get; set; }
        public string Set { get; set; }
        [Display(Name = "identificador del repositorio")]
        public Guid RepositoryID { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
