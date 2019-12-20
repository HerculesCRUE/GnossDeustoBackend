using System;

namespace UrisFactory.Extra.Exceptions
{
    public class FailedLoadConfigJsonException : Exception
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
