using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CargaDataSetMurcia.Model
{
    public class SparqlObject
    {
        public SparqlObject() { }

        public Head head { get; set; }
        public Results results { get; set; }
        public bool boolean { get; set; }


        [DataContract]
        public class Data
        {
            public Data() { }
            [DataMember(Name = "datatype")]
            public string datatype { get; set; }
            [DataMember(Name = "type")]
            public string type { get; set; }
            [DataMember(Name = "value")]
            public string value { get; set; }
            [DataMember(Name = "xml:lang")]
            public string lang { get; set; }
        }
        public class Head
        {
            public Head() { }

            public HashSet<object> link { get; set; }            

            public HashSet<string> vars { get; set; }            
        }
        public class Results
        {
            public Results() { }

            public List<Dictionary<string, Data>> bindings { get; set; }
            public bool distinct { get; set; }
            public bool ordered { get; set; }
        }
    }
}
