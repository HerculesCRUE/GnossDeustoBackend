using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Entities
{
    public class InfoUriStructure
    {
        public UriStructure UriStructure{get;set;}

        public ResourcesClass ResourcesClass { get; set; }
    }
}
