using System;
using System.Collections.Generic;
using System.Linq;

namespace API_CARGA.Models.Entities
{
    public class SparqlConfig
    {
        public string graph { get; set; }
        public string endpoint { get; set; }
        public string queryparam { get; set; }
    }
}
