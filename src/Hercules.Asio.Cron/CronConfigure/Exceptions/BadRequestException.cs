// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepion
using System;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Exceptions
{
    /// <summary>
    /// BadRequestException.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BadRequestException : Exception
    {
        /// <summary>
        /// BadRequestException.
        /// </summary>
        public BadRequestException()
        {
        }

        /// <summary>
        /// BadRequestException.
        /// </summary>
        /// <param name="message"></param>
        public BadRequestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// BadRequestException.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public BadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
