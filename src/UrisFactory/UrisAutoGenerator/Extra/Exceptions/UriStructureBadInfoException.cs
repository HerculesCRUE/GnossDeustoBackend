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
        public UriStructureBadInfoException()
        {
        }

        public UriStructureBadInfoException(string message)
            : base(message)
        {
        }

        public UriStructureBadInfoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
