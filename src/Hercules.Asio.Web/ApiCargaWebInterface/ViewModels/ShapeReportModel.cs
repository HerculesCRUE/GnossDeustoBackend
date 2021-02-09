// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Datos de reporte de un Shape
using System;
using System.Collections.Generic;

namespace ApiCargaWebInterface.ViewModels
{
    /// <summary>
    /// Datos de reporte de un Shape
    /// </summary>
    public class ShapeReportModel
    {
        public class Result
        {
            /// <summary>
            /// Severidad
            /// </summary>
            public string severity { get; set; }

            /// <summary>
            /// Nodo 
            /// </summary>
            public string focusNode { get; set; }

            /// <summary>
            /// Resultado
            /// </summary>
            public string resultValue { get; set; }

            /// <summary>
            /// Mensaje de error
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// Path del resultado
            /// </summary>
            public string resultPath { get; set; }

            /// <summary>
            /// Nombre del Shape
            /// </summary>
            public string shapeName { get; set; }

            /// <summary>
            /// Identificador del shape SHACL
            /// </summary>
            public string sourceShape { get; set; }

            /// <summary>
            /// Identificador de la configuración del Shape configurado
            /// </summary>
            public Guid shapeID { get; set; }
        }

        /// <summary>
        /// Severidad
        /// </summary>
        public string severity { get; set; }

        /// <summary>
        /// Indica si no se incumple ninguna restricción
        /// </summary>
        public bool conforms { get; set; }

        /// <summary>
        /// Lista con las validaciones que no han pasado
        /// </summary>
        public List<Result> results { get; set; }
    }
}
