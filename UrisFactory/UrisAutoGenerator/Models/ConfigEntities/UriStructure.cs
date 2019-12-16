using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Models.ConfigEntities
{
    public class UriStructure
    {
        public string Base { get; set; }
        public IList<Characters> Characters { get; set; }
        public IList<UrlStructure> UrlStructures { get; set; }
        public IList<ResourcesClass> ResourcesClasses { get; set; }
    }
}
