using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorDocumentacion.ViewModel
{
    public class PageViewModel
    {
        public Guid PageID { get; set; }
        public string Route { get; set; }
        public string Name { get; set; }
    }
}
