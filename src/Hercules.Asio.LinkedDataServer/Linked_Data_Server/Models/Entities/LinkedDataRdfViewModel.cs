﻿using System;
using System.Collections.Generic;

namespace Linked_Data_Server.Models.Entities
{
    public class LinkedDataRdfViewModel
    {
        public class ProvenanceData
        {
            public string property { get; set; }
            public string value { get; set; }
            public string organization { get; set; }
            public DateTime date { get; set; }
        }
        public List<string> urisRdf { get; set; }
        public string uriEntity { get; set; }
        public bool bnode { get; set; }

        public Dictionary<string, List<string>> stringPropertiesEntity { get; set; }
        public List<ProvenanceData> provenanceData { get; set; }
        public Dictionary<string, List<LinkedDataRdfViewModel>> entitiesPropertiesEntity { get; set; }
        public Dictionary<string, string> communNamePropierties { get; set; }
        public Dictionary<string, string> entitiesNames { get; set; }
        
        public List<string> LoadedEntities { get; set; }
    }
}
