// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción para indicar que ha fallado la validación de un RDF
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// Excepción para indicar que ha fallado la validación de un RDF
/// </summary>
namespace API_CARGA.Extras.Excepciones
{

    [ExcludeFromCodeCoverage]
    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
