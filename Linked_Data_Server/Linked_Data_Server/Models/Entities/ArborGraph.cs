using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linked_Data_Server.Models.Entities
{
    /// <summary>
    /// Gráfico en el que se pintan los datos
    /// </summary>
    public class ArborGraph
    {
        public class Node
        {
            /// <summary>
            /// Color del nodo
            /// </summary>
            public string color { get; set; }
            /// <summary>
            /// Forma del nodo
            /// </summary>
            public string shape { get; set; }
            /// <summary>
            /// Nombre que se muestra 
            /// </summary>
            public string label { get; set; }
            /// <summary>
            /// Imagen para mostrar como icono
            /// </summary>
            public string image { get; set; }
            /// <summary>
            /// Identificador del nodo
            /// </summary>
            public string link { get; set; }
            /// <summary>
            /// True si es la entidad principal
            /// </summary>
            public bool main { get; set; }
        }

        /// <summary>
        /// Relaciones entre diferentes nodos
        /// </summary>
        public class Relation
        {
            /// <summary>
            /// Relaciones entre diferentes nodos
            /// </summary>
            /// <param name="pname">Nombre de la relación</param>
            /// <param name="pcolor">Color de la línea que une los nodos</param>
            public Relation(string pname,string pcolor)
            {
                name = pname;
                color = pcolor;
            }
            /// <summary>
            /// nombre de la relación
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// Color de la línea que une los nodos
            /// </summary>
            public string color { get; set; }
        }
        public string Name { get; set; }
        public Dictionary<string, Node> nodes { get; set; }
        /// <summary>
        /// Relaciones entre los nodos
        /// </summary>
        public Dictionary<string, Dictionary<string, Relation>> edges { get; set; }
    }
}
