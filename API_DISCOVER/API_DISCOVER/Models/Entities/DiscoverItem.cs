// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_DISCOVER.Models.Entities
{
    ///<summary>
    ///Representa un item de descubrimiento
    ///</summary>
    public class DiscoverItem
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
                /// Identificador del problema de desambiguación
                /// </summary>
                [Key]
                public Guid ID { get; set; }

                /// <summary>
                /// Identificador del DiscoverDissambiguation
                /// </summary>
                [Required]
                public Guid DiscoverDissambiguationID { get; set; }

                /// <summary>
                /// Candidato para la desambiguación
                /// </summary>
                [Required]
                public string IDCandidate { get; set; }

                /// <summary>
                /// Puntuación para la desambiguación
                /// </summary>
                [Required]
                public float Score { get; set; }
            }

            public DiscoverDissambiguation()
            {
            }

            /// <summary>
            /// Identificador del problema de desambiguación
            /// </summary>
            [Key]
            public Guid ID { get; set; }

            /// <summary>
            /// Identificador del discoverItem
            /// </summary>
            [Required]
            public Guid DiscoverItemID { get; set; }

            /// <summary>
            /// Identificador de la entidad en la que hay dudas
            /// </summary>
            [Required]
            public string IDOrigin { get; set; }

            /// <summary>
            /// Candiatos de desambiguación de desambiguación
            /// </summary>
            [ForeignKey("DiscoverDissambiguationID")]
            public virtual ICollection<DiscoverDissambiguationCandiate> DissambiguationCandiates { get; set; }
        }

        public enum DiscoverItemStatus
        {
            Pending,
            Processed,
            ProcessedDissambiguationProblem,
            Error
        }

        public DiscoverItem()
        {
        }

        /// <summary>
        /// Identificador del item
        /// </summary>
        [Key]
        public Guid ID { get; set; }

        /// <summary>
        /// Indica el estado del item:
        ///     Pending
        ///     Processing
        ///     Processed
        ///     Error
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// RDF original antes del descbrimiento
        /// </summary>
        [Required]
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
        /// Identificador de la tarea de la que procede (en caso de que proceda de una tarea)
        /// </summary>
        public string JobID { get; set; }

        /// <summary>
        /// Fecha de creación de la tarea (en caso de que proceda de una tarea)
        /// </summary>
        public string JobCreatedDate { get; set; }

        /// <summary>
        /// Indica si hay que publicar el resultado o no 
        /// </summary>
        public bool Publish { get; set; }

        /// <summary>
        /// Indica si están ya resueltos los problemas de desambiguación
        /// </summary>
        public bool DissambiguationProcessed { get; set; }

        /// <summary>
        /// Reporte de las tareas del descubrimiento
        /// </summary>
        public string DiscoverReport { get; set; }

        /// <summary>
        /// Problemas de desambiguación
        /// </summary>
        [ForeignKey("DiscoverItemID")]
        public virtual ICollection<DiscoverDissambiguation> DissambiguationProblems { get; set; }
    }
}