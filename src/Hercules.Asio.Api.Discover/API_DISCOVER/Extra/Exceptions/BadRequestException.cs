// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción del tipo BadRequest
using System;
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER.Extra.Exceptions
{
    /// <summary>
    /// Excepción del tipo BadRequest
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
