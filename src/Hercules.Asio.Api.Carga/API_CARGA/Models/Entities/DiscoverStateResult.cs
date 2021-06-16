// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.Models.Entities
{
    [ExcludeFromCodeCoverage]

    ///<summary>
    ///Representa el estao de un item de descubrimiento
    ///</summary>
    ///
    public class DiscoverStateResult
    {
        ///<summary>
        ///Representa un problema de desambiguación en un DiscoverItem
        ///</summary>
        public class DiscoverDissambiguation
        {
            ///<summary>
            ///Representa un candidato para un problema de desambiguación en un DiscoverItem
            ///</summary>
            public class DiscoverDissambiguationCandiate
            {
                public DiscoverDissambiguationCandiate()
                {
                }

                /// <summary>
                /// Candidato para la desambiguación
                /// </summary>
                public string IDCandidate { get; set; }

                /// <summary>
                /// Puntuación para la desambiguación
                /// </summary>
                public float Score { get; set; }
            }

            public DiscoverDissambiguation()
            {
            }

            /// <summary>
            /// Identificador de la entidad en la que hay dudas
            /// </summary>
            public string IDOrigin { get; set; }

            /// <summary>
            /// Candiatos de desambiguación de desambiguación
            /// </summary>
            public List<DiscoverDissambiguationCandiate> DissambiguationCandiates { get; set; }
        }

        public enum DiscoverItemStatus
        {
            Pending,
            Processed,
            ProcessedDissambiguationProblem,
            Error
        }

        public DiscoverStateResult()
        {
        }

        /// <summary>
        /// Identificador del item
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Indica el estado del item:
        ///     Pending
        ///     Processing
        ///     Processed
        ///     Error
        /// </summary>
        public DiscoverItemStatus Status { get; set; }

        /// <summary>
        /// RDF original antes del descbrimiento
        /// </summary>
        public string Rdf { get; set; }

        /// <summary>
        /// RDF final tras el descubrimiento
        /// </summary>
        public string DiscoverRdf { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Reporte del proceso de descubrimiento
        /// </summary>
        public string DiscoverReport { get; set; }

        /// <summary>
        /// Problemas de desambiguación
        /// </summary>
        public List<DiscoverDissambiguation> DissambiguationProblems { get; set; }
    }
}