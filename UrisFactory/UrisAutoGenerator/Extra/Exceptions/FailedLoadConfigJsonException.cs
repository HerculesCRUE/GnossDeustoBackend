using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Extra.Exceptions
{
    public class FailedLoadConfigJsonException: Exception
    {
        public FailedLoadConfigJsonException()
        {
        }

        public FailedLoadConfigJsonException(string message)
            : base(message)
        {
        }

        public FailedLoadConfigJsonException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
