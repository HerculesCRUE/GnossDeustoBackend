using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Extra.Exceptions
{
    public class ParametersNotConfiguredException: Exception
    {
        public ParametersNotConfiguredException()
        {
        }

        public ParametersNotConfiguredException(string message)
            : base(message)
        {
        }

        public ParametersNotConfiguredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
