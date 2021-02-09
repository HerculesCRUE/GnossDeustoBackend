using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    public class Table
    {
        public class Row
        {
            public List<string> fields { get; set; }
        }
        public string Name { get; set; }
        public List<string> Header { get; set; }
        public List<Row> Rows { get; set; }
    }
}
