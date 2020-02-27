using System;

namespace RepositoryConfigSolution.Extra.Exception
{
    public class InitialDateException : System.Exception
    {
        public InitialDateException()
        {
        }

        public InitialDateException(string message)
            : base(message)
        {
        }

        public InitialDateException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
