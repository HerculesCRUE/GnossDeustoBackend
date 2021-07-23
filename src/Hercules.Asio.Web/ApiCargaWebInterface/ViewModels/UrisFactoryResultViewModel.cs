﻿using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class UrisFactoryResultViewModel
    {
        public string UriResult { get; set; }
        public string Error { get; set; }
        [Display(Name = "ResourceClass / RdfType")]
        public string Resource_class { get; set; }
        public string Identifier { get; set; }
        public string Uri_Structure { get; set; }        
        public IFormFile New_Schema_File { get; set; }
        public HashSet<string> listaRdfType { get; set; }
    }
}
