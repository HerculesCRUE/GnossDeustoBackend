using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    public class SearchModelTemplate
    {
        public class Entidad {
            public Entidad(string pname, string prdfType){
                name = pname;
                rdfType = prdfType;
            }
            public string name;
            public string rdfType;
        }

        public Dictionary<string, Entidad> entidades { get; set; }
        public int paginaAnterior { get; set; }
        public int paginaSiguiente { get; set; }
    }
}
