// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción para indicar que hay un fallo con la configuración de la estructura Uri
using System;

namespace UrisFactory.Extra.Exceptions
{
    ///<summary>
    ///Excepción para indicar que hay un fallo con la configuración de la estructura Uri
    ///</summary>
    public class UriStructureConfiguredException : Exception
    {
        public UriStructureConfiguredException()
        {
        }

        public UriStructureConfiguredException(string message)
            : base(message)
        {
        }

        public UriStructureConfiguredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
