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
        /// <param name="pConfigTables">configuraciones</param>
        /// <param name="pExcludeRelatedEntity">Lista de entidades excluidas para pintar las entidades relacionadas</param>
        /// <param name="pConfigGraph">Configuraciones para el grafo</param>
        /// </summary>
        public Linked_Data_Server_Config(List<ConfigTable> pConfigTables, List<string> pExcludeRelatedEntity, ConfigGraph pConfigGraph)
        {
            ConfigTables = pConfigTables;
            ExcludeRelatedEntity = pExcludeRelatedEntity;
            configGraph = pConfigGraph;
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
                /// Nombre de la tabla
                /// </summary>
                public string name { get; set; }
                /// <summary>
                /// Consulta a realizar
                /// </summary>
                public string query { get; set; }
                /// <summary>
                /// Nombre de las columnas
                /// </summary>
                public List<string> fields { get; set; }
            }

            /// <summary>
            /// rdf:type al que afecta
            /// </summary>
            public string rdfType { get; set; }
            /// <summary>
            /// Tabla por cada rdfType
            /// </summary>
            public List<Property> tables { get; set; }
        }

        /// <summary>
        /// Tablas de configuración
        /// </summary>
        public List<ConfigTable> ConfigTables { get; set; }

        /// <summary>
        /// Lista de entidades excluidas para pintar las entidades relacionadas
        /// </summary>
        public List<string> ExcludeRelatedEntity { get; set; }

        /// <summary>
        /// Lista de las configuraciones para los grafos
        /// </summary>
        public class ConfigGraph 
        { 
            public ConfigGraph(List<Property> pIcons, List<ArborGraph> pGraphs)
            {
                Icons = pIcons;
                Graphs = pGraphs;
            }

            public class Property
            {
                public Property(string prdfType, string picon)
                {
                    rdfType = prdfType;
                    icon = picon;
                }
                /// <summary>
                /// 
                /// </summary>
                public string rdfType { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string icon { get; set; }
            }

            /// <summary>
            /// Datos para construir arbor graph
            /// </summary>
            public class ArborGraph
            {
                /// <summary>
                /// <param name="pproperties">Lista de propiedades</param>
                /// <param name="prdfType">rdf:type al que afecta</param>
                /// <param name="ppropName"></param>
                /// </summary>
                public ArborGraph(string prdfType, List<Property> pproperties, string ppropName)
                {
                    rdfType = prdfType;
                    properties = pproperties;
                    propName = ppropName;
                }

                public class Property
                {
                    public Property(string pname, string pquery)
                    {
                        name = pname;
                        query = pquery;
                    }
                    /// <summary>
                    /// Consulta a realizar
                    /// </summary>
                    public string query { get; set; }
                    /// <summary>
                    /// Nombre de la propiedad
                    /// </summary>
                    public string name { get; set; }
                }

                /// <summary>
                /// Lista de propiedades
                /// </summary>
                public List<Property> properties { get; set; }
                /// <summary>
                /// rdf:type al que afecta
                /// </summary>
                public string rdfType { get; set; }

                /// <summary>
                /// 
                /// </summary>
                public string propName { get; set; }
            }

            public List<Property> Icons { get; set; }
            public List<ArborGraph> Graphs { get; set; }
        }

        public ConfigGraph configGraph { get; set; }
    }

}

