using ApiCargaWebInterface.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class GetTokenViewModel
    {
        [Display(Name = "Tipo de token")]
        public Dictionary<int, string> TokenOptions { get; set; }
        public string Token { get; set; }
    }
}
