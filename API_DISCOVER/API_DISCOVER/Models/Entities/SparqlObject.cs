// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace API_DISCOVER.Models.Entities
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
