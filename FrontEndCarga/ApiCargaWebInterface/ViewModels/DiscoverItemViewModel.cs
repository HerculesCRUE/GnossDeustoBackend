using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class DiscoverItemViewModel
    {
        [Display(Name = "Identificador del error")]
        public Guid IdDiscoverItem { get; set; }
        [Display(Name = "Error")]
        public string Error { get; set; }
        [Display(Name = "Problemas de desambiguación")]
        public Dictionary<string, List<string>> DissambiguationProblems { get; set; }
        [Display(Name = "Título de las entidades")]
        public Dictionary<string, string> DissambiguationProblemsTitles { get; set; }
        public string JobId { get; set; }
    }
}
