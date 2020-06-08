using System;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que ha fallado la carga del fichero de configuración
    ///</summary>
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
