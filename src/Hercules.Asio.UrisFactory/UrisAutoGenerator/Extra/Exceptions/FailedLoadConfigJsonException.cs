// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción para indicar que ha fallado la carga del fichero de configuración
using System;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que ha fallado la carga del fichero de configuración
    ///</summary>
    public class FailedLoadConfigJsonException : Exception
    {
        /// <summary>
        /// FailedLoadConfigJsonException
        /// </summary>
        public FailedLoadConfigJsonException()
        {
        }
        /// <summary>
        /// FailedLoadConfigJsonException
        /// </summary>
        /// <param name="message"></param>
        public FailedLoadConfigJsonException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// FailedLoadConfigJsonException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public FailedLoadConfigJsonException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
