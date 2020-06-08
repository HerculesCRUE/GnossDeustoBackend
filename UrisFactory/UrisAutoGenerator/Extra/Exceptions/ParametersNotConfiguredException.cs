using System;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que hay parametros configurados
    ///</summary>
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
