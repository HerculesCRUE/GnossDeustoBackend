using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    public class EntityModelTemplate
    {
        public List<LinkedDataRdfViewModel> linkedDataRDF { get; set; }
        public List<Linked_Data_Server.Models.Services.Config_Linked_Data_Server.PropertyTransform> propsTransform { get; set; }
        public List<Table> tables { get; set; }
        public List<ArborGraph> arborGraphs { get; set; }
        public bool Status405 { get; set; }
        public string Rdf { get; set; }
        public string domain { get; set; }
    }
}
