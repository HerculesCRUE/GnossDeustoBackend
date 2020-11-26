using System;
using System.Collections.Generic;

namespace Linked_Data_Server.Models.Entities
{
    public class DiscoverRdfViewModel
    {
        public List<string> urisRdf { get; set; }
        public string uriEntity { get; set; }

        public Dictionary<string, List<String>> stringPropertiesEntity { get; set; }
        public Dictionary<string, Dictionary<string, List<String>>> stringPropertiesEntityGraph { get; set; }
        public Dictionary<string, List<DiscoverRdfViewModel>> entitiesPropertiesEntity { get; set; }
        public Dictionary<string, string> communNamePropierties { get; set; }
        public List<string> LoadedEntities { get; set; }
    }
}
