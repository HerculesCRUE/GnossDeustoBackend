using System;
using System.Collections.Generic;
using System.Text;

namespace API_DISCOVER.Models.Entities.Discover
{
    /// <summary>
    /// Datos para trabajar con la reconciliación
    /// </summary>
    public class ReconciliationData
    {
        /// <summary>
        /// Datos de score de una reconciliación
        /// </summary>
        public class ReconciliationScore
        {
            /// <summary>
            /// URL de la entidad encontrada en la BBDD
            /// </summary>
            public string uri { get; set; }

            /// <summary>
            /// Score de la reconciliación (entre 0 y 1)
            /// </summary>
            public float score { get; set; }
        }

        /// <summary>
        /// Entidades reconciliadas con los sujetos
        /// </summary>
        public HashSet<string> reconciliatedEntitiesWithSubject { get; set; }

        /// <summary>
        /// Entidades reconciliadas con los IDs
        /// </summary>
        public Dictionary<string, string> reconciliatedEntitiesWithIds { get; set; }

        /// <summary>
        /// Entidades reconciliadas con los datos de la BBDD
        /// </summary>
        public Dictionary<string, ReconciliationScore> reconciliatedEntitiesWithBBDD { get; set; }

        /// <summary>
        /// Entidades reconciliadas con la BBDD apoyados con las integraciones externas
        /// </summary>
        public Dictionary<string, ReconciliationScore> reconciliatedEntitiesWithExternalIntegration { get; set; }

        /// <summary>
        /// Entidades reconciliadas por cualquiera de los métodos
        /// </summary>
        public Dictionary<string, string> reconciliatedEntityList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationData()
        {
            reconciliatedEntitiesWithSubject = new HashSet<string>();
            reconciliatedEntitiesWithIds = new Dictionary<string, string>();
            reconciliatedEntitiesWithBBDD = new Dictionary<string, ReconciliationScore>();
            reconciliatedEntitiesWithExternalIntegration = new Dictionary<string, ReconciliationScore>();
            reconciliatedEntityList = new Dictionary<string, string>();
        }
    }
}
