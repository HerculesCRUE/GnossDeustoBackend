using System;

namespace UrisFactory.Extra.Exceptions
{
    public class ParametersNotConfiguredException : Exception
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
