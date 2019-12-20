
using System.Collections.Generic;

namespace UrisFactory.Models.ConfigEntities
{
    public class UriStructure
    {
        public string Name { get; set; }
        public IList<Component> Components { get; set; }
    }
}
