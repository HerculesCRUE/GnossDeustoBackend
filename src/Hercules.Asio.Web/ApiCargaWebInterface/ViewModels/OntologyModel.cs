using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class OntologyModel
    {
        public IFormFile Ontology_uri { get; set; }
        public string Messagge { get; set; }
    }
}
