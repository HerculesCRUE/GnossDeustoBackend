using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que hay un fallo con la configuración de la estructura Uri
    ///</summary>
    public class UriStructureConfiguredException : Exception
    {
        public UriStructureConfiguredException()
        {
        }

        public UriStructureConfiguredException(string message)
            : base(message)
        {
        }

        public UriStructureConfiguredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
