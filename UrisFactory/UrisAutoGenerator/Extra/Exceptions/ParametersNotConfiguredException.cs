// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción para indicar que hay parametros configurados
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
