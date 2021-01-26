using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    public class ArborGraph
    {
        public class Node
        {
            public string color { get; set; }
            public string shape { get; set; }
            public string label { get; set; }
            public string image { get; set; }
            public string link { get; set; }
        }

        public class Relation
        {
            public Relation(string pname)
            {
                name = pname;
            }
            public string name { get; set; }
        }
        public string Name { get; set; }
        public Dictionary<string, Node> nodes { get; set; }
        public Dictionary<string, Dictionary<string, Relation>> edges { get; set; }
    }
}
