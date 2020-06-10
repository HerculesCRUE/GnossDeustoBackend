using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Entities
{
    ///<summary>
    ///Sirve encapsular los datos provenientes del ListIdentifiers
    ///</summary>
    public class IdentifierOAIPMH
    {
        public string Identifier { get; set; }
        public DateTime Fecha { get; set; }
    }
}
