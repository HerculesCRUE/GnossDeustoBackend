using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que no concuerdan los datos de la estructura uri
    ///</summary>
    public class UriStructureBadInfoException : Exception
    {
        public UriStructureBadInfoException()
        {
        }

        public UriStructureBadInfoException(string message)
            : base(message)
        {
        }

        public UriStructureBadInfoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
