using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Extra.Exceptions
{
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
