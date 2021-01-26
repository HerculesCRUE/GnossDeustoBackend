using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    /// <summary>
    /// Configuración para la presentación de las fichas de Linked_Data_Server
    /// </summary>
    public class Config_Linked_Data_Server
    {
        /// <summary>
        /// Configuración de tablas para un rdfType
        /// </summary>
        public class ConfigTable
        {
            /// <summary>
            /// Configuración de una tabla
            /// </summary>
            public class Table
            {
                /// <summary>
                /// Nombre de la tabla
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
            /// Tabla por cada rdfType
            /// </summary>
            public List<Table> tables { get; set; }
        }

        /// <summary>
        /// Configuraciones para pintar los grafos
        /// </summary>
        public class ConfigArborGraph
        {
            public class Icon
            {
                /// <summary>
                /// RDFtype al que afecta el icono
                /// </summary>
                public string rdfType { get; set; }
                
                /// <summary>
                /// Ruta del icono
                /// </summary>
                public string icon { get; set; }
            }

            /// <summary>
            /// Datos para pintar los grafos
            /// </summary>
            public class ArborGraphRdfType
            {
                public class ArborGraph
                {
                    public class Property
                    {
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
                    /// Nombre del gráfico
                    /// </summary>
                    public string name { get; set; }

                    /// <summary>
                    /// Propiedades a cargar
                    /// </summary>
                    public List<Property> properties { get; set; }
                }

                /// <summary>
                /// Lista de gráficos
                /// </summary>
                public List<ArborGraph> arborGraphs { get; set; }

                /// <summary>
                /// rdf:type al que afecta
                /// </summary>
                public string rdfType { get; set; }

                /// <summary>
                /// Propiedad para pintar en el nombre del elemento
                /// </summary>
                public string propName { get; set; }
            }

            public List<Icon> icons { get; set; }
            public List<ArborGraphRdfType> arborGraphsRdfType { get; set; }
        }


        /// <summary>
        /// Configuraciones de las tablas
        /// </summary>
        public List<ConfigTable> ConfigTables { get; set; }

        /// <summary>
        /// Configuraciones de los grafos para pintar
        /// </summary>
        public ConfigArborGraph ConfigArborGraphs { get; set; }

        /// <summary>
        /// Lista de entidades excluidas para pintar las entidades relacionadas
        /// </summary>
        public List<string> ExcludeRelatedEntity { get; set; }
    }

}

