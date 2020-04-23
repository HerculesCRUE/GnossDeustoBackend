using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Datos de reporte de un Shape
    /// </summary>
    public class ShapeReport
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
        }
        /// <summary>
        /// 
        /// </summary>
        public bool conforms { get; set; }
        //
        public List<Result> results { get; set; }
    }
}
