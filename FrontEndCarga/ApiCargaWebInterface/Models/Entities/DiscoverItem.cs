// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ApiCargaWebInterface.Models.Entities
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
            /// Candiatos de desambiguación
            /// </summary>
            [ForeignKey("DiscoverDissambiguationID")]
            public virtual ICollection<DiscoverDissambiguationCandiate> DissambiguationCandiates { get; set; }
        }

        ///<summary>
        ///Representa descartes para un problema de desambiguación en un DiscoverItem
        ///</summary>
        public class DiscardDissambiguation
        {
            public DiscardDissambiguation()
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
            /// Identificador de la entidad para que le tenemos los descartes
            /// </summary>
            [Required]
            public string IDOrigin { get; set; }

            /// <summary>
            /// Candidatos descartados cargadas
            /// </summary>
            public List<string> DiscardCandidates { get; set; }
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
        ///     Processed
        ///     ProcessedDissambiguationProblem
        ///     Error
        /// </summary>
        [Required]
        public string Status { get; set; }

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
        /// Identificador de la tarea de la que procede (en caso de que proceda de una tarea)
        /// </summary>
        public string JobID { get; set; }

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

        /// <summary>
        /// Descartes de desambiguación
        /// </summary>
        [ForeignKey("DiscoverItemID")]
        public virtual ICollection<DiscardDissambiguation> DiscardDissambiguations { get; set; }

        /// <summary>
        /// Entidades cargadas
        /// </summary>
        public List<string> LoadedEntities { get; set; }

        /// <summary>
        /// Modificamos el objeto DiscoverItem para guardarlo cuando ha habido problemas de desambiguación
        /// </summary>
        /// <param name="pProblems">Problemas de desambiguación</param>
        /// <param name="pLoadedEntities">Entidades ya cargadas en el grafo</param>
        /// <param name="pDiscoverRDF">RDF de descubrimiento</param>
        public void UpdateDissambiguationProblems(Dictionary<string, Dictionary<string, float>> pProblems, List<string> pLoadedEntities, string pDiscoverRDF)
        {
            Status = DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString();
            DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
            Error = "";
            DiscoverReport = "";
            DiscoverRdf = pDiscoverRDF;
            LoadedEntities = pLoadedEntities;
            foreach (string idOrigin in pProblems.Keys)
            {
                DiscoverDissambiguation discoverDissambiguation = DissambiguationProblems.FirstOrDefault(x => x.IDOrigin == idOrigin);
                if (discoverDissambiguation == null)
                {
                    discoverDissambiguation = new DiscoverDissambiguation();
                    discoverDissambiguation.IDOrigin = idOrigin;
                    DissambiguationProblems.Add(discoverDissambiguation);
                }
                if (discoverDissambiguation.DissambiguationCandiates == null)
                {
                    discoverDissambiguation.DissambiguationCandiates = new List<DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate>();
                }

                foreach (string candidate in pProblems[idOrigin].Keys)
                {
                    DiscoverDissambiguation.DiscoverDissambiguationCandiate discoverDissambiguationCandiate = discoverDissambiguation.DissambiguationCandiates.FirstOrDefault(x => x.IDCandidate == candidate);
                    if (discoverDissambiguationCandiate == null)
                    {
                        discoverDissambiguationCandiate = new DiscoverDissambiguation.DiscoverDissambiguationCandiate();
                        discoverDissambiguationCandiate.IDCandidate = candidate;
                        discoverDissambiguationCandiate.Score = pProblems[idOrigin][candidate];
                        discoverDissambiguation.DissambiguationCandiates.Add(discoverDissambiguationCandiate);
                    }
                    else if (discoverDissambiguationCandiate.Score < pProblems[idOrigin][candidate])
                    {
                        discoverDissambiguationCandiate.Score = pProblems[idOrigin][candidate];
                    }
                }
            }
        }

        /// <summary>
        /// Modificamos el objeto DiscoverItem para guardarlo cuando hse han seleccionado descartes para la desmbiguación y se va a volver a procesar
        /// </summary>
        /// <param name="pDiscards">Problemas de desambiguación</param>
        /// <param name="pDiscoverRDF">RDF de descubrimiento</param>
        public void UpdateDissambiguationDiscards(Dictionary<string, List<string>> pDiscards, string pDiscoverRDF)
        {
            Status = DiscoverItem.DiscoverItemStatus.Pending.ToString();
            DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
            Error = "";
            DiscoverReport = "";
            DiscoverRdf = pDiscoverRDF;
            LoadedEntities = new List<string>();
            if(DiscardDissambiguations == null)
            {
                DiscardDissambiguations = new List<DiscardDissambiguation>();
            }
            foreach (string idOrigin in pDiscards.Keys)
            {
                DiscardDissambiguation discardDissambiguation = DiscardDissambiguations.FirstOrDefault(x => x.IDOrigin == idOrigin);
                if (discardDissambiguation == null)
                {
                    discardDissambiguation = new DiscardDissambiguation();
                    discardDissambiguation.IDOrigin = idOrigin;
                    DiscardDissambiguations.Add(discardDissambiguation);
                }
                if (discardDissambiguation.DiscardCandidates == null)
                {
                    discardDissambiguation.DiscardCandidates = new List<string>();
                }
                foreach (string discardCandidate in pDiscards[idOrigin])
                {
                    if (!discardDissambiguation.DiscardCandidates.Contains(discardCandidate))
                    {
                        discardDissambiguation.DiscardCandidates.Add(discardCandidate);
                    }
                }
            }
        }

        /// <summary>
        /// Modificamos el objeto DiscoverItem para guardarlo cuando se procesa para generar un report
        /// </summary>
        /// <param name="pProblems">Problemas de desambiguación</param>
        /// <param name="pDiscoverRDF">RDF de descubrimiento</param>     
        /// <param name="pDiscoverReport">Reporte</param>
        public void UpdateReport(Dictionary<string, Dictionary<string, float>> pProblems, string pDiscoverRDF, string pDiscoverReport)
        {
            DiscoverRdf = pDiscoverRDF;
            Status = DiscoverItem.DiscoverItemStatus.Processed.ToString();
            DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
            Error = "";
            DiscoverReport = pDiscoverReport;

            foreach (string idOrigin in pProblems.Keys)
            {
                DiscoverDissambiguation discoverDissambiguation = DissambiguationProblems.FirstOrDefault(x => x.IDOrigin == idOrigin);
                if (discoverDissambiguation == null)
                {
                    discoverDissambiguation = new DiscoverDissambiguation();
                    discoverDissambiguation.IDOrigin = idOrigin;
                    DissambiguationProblems.Add(discoverDissambiguation);
                }
                if (discoverDissambiguation.DissambiguationCandiates == null)
                {
                    discoverDissambiguation.DissambiguationCandiates = new List<DiscoverItem.DiscoverDissambiguation.DiscoverDissambiguationCandiate>();
                }

                foreach (string candidate in pProblems[idOrigin].Keys)
                {
                    DiscoverDissambiguation.DiscoverDissambiguationCandiate discoverDissambiguationCandiate = discoverDissambiguation.DissambiguationCandiates.FirstOrDefault(x => x.IDCandidate == candidate);
                    if (discoverDissambiguationCandiate == null)
                    {
                        discoverDissambiguationCandiate = new DiscoverDissambiguation.DiscoverDissambiguationCandiate();
                        discoverDissambiguationCandiate.IDCandidate = candidate;
                        discoverDissambiguationCandiate.Score = pProblems[idOrigin][candidate];
                        discoverDissambiguation.DissambiguationCandiates.Add(discoverDissambiguationCandiate);
                    }
                    else if (discoverDissambiguationCandiate.Score < pProblems[idOrigin][candidate])
                    {
                        discoverDissambiguationCandiate.Score = pProblems[idOrigin][candidate];
                    }
                }
            }
        }

        /// <summary>
        /// Modificamos el objeto DiscoverItem para guardarlo cuando se procesa correctamente
        /// </summary>
        public void UpdateProcessed()
        {
            DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
            DiscoverReport = "";
            Status = DiscoverItem.DiscoverItemStatus.Processed.ToString();
            DiscoverRdf = null;
            Rdf = null;
        }

        /// <summary>
        /// Modificamos el objeto DiscoverItem para guardarlo cuando se produce un error
        /// </summary>
        /// <param name="pError">Error</param>
        public void UpdateError(string pError)
        {
            Status = DiscoverItem.DiscoverItemStatus.Error.ToString();
            DissambiguationProblems = new List<DiscoverItem.DiscoverDissambiguation>();
            Error = pError;
            DiscoverRdf = "";
            DiscoverReport = "";
        }
    }
}