// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción para indicar que no concuerdan los datos de la estructura uri
using System;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que no concuerdan los datos de la estructura uri
    ///</summary>
    public class UriStructureBadInfoException : Exception
    {
        /// <summary>
        /// UriStructureBadInfoException
        /// </summary>
        public UriStructureBadInfoException()
        {
        }
        /// <summary>
        /// UriStructureBadInfoException
        /// </summary>
        /// <param name="message"></param>
        public UriStructureBadInfoException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// UriStructureBadInfoException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public UriStructureBadInfoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
