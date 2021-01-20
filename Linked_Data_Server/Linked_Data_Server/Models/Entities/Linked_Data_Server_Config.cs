using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    /// <summary>
    /// Configuración de desambiguación utilizada para apoyar en la realización de la desambiguación
    /// </summary>
    public class Linked_Data_Server_Config
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pConfigTables">configuraciones</param>
        public Linked_Data_Server_Config(List<ConfigTable> pConfigTables)
        {
            ConfigTables = pConfigTables;
        }
        /// <summary>
        /// 
        /// </summary>
        public class ConfigTable
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="prdfType">rdf:type al que afecta</param>
            /// <param name="ptables">Consulta a realizar</param>
            public ConfigTable(string prdfType, List<Property> ptables)
            {
                rdfType = prdfType;
                tables = ptables;
            }

            public class Property
            {
                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="pname">Nombre del documento</param>
                /// <param name="pfields">Nombre de los campos</param>
                /// <param name="pquery">Consulta a realizar</param>
                public Property(string pname, List<string> pfields, string pquery)
                {
                    name = pname;
                    fields = pfields;
                    query = pquery;
                }

                /// <summary>
                /// Nombre del documento
                /// </summary>
                public string name { get; set; }
                /// <summary>
                /// Consulta a realizar
                /// </summary>
                public string query { get; set; }
                /// <summary>
                /// Nombre de los campos
                /// </summary>
                public List<string> fields { get; set; }
            }

            /// <summary>
            /// rdf:type al que afecta
            /// </summary>
            public string rdfType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Property> tables { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<ConfigTable> ConfigTables { get; set; }
    }

}

