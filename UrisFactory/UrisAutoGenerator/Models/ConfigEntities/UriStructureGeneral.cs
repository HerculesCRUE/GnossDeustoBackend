using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Models.ConfigEntities
{
    public class UriStructureGeneral
    {
        public string Base { get; set; }
        public IList<Characters> Characters { get; set; }
        public IList<UriStructure> UriStructures { get; set; }
        public IList<ResourcesClass> ResourcesClasses { get; set; }
    }
}
