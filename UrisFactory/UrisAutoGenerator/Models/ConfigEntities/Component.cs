using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Models.ConfigEntities
{
    public class Component
    {
        public string UrlComponent { get; set; }
        public string UrlComponentValue { get; set; }
        public int UrlComponentOrder { get; set; }
        public bool Mandatory { get; set; }
        public string FinalCharacter { get; set; }
    }
}
