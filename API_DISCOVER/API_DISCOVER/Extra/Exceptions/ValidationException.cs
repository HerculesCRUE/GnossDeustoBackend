﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Excepción de validación
using API_DISCOVER.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER.Extra.Exceptions
{
    /// <summary>
    /// Excepción de validación
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ValidationException : Exception
    {
        public ShapeReportModel Report { get; set; }
        public ValidationException()
        {
        }

        public ValidationException(ShapeReportModel report)
        {
            Report = report;
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
