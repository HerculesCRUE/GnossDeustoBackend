using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    public class EntityModelTemplate
    {
        public List<LinkedDataRdfViewModel> linkedDataRDF { get; set; }
        public Dictionary<string, string> propsTransform { get; set; }
        public List<Table> tables { get; set; }
        public List<ArborGraph> arborGraphs { get; set; }
    }
}
