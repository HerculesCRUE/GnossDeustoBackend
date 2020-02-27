using System;

namespace RepositoryConfigSolution.Extra.Exception
{
    public class PeriodicityFormatException : System.Exception
    {
        public PeriodicityFormatException()
        {
        }

        public PeriodicityFormatException(string message)
            : base(message)
        {
        }

        public PeriodicityFormatException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
