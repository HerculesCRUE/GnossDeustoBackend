// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Objeto que representa a un objeto que hay dentro de la estructura ResourcesClasses del fichero json de configuración
namespace UrisFactory.Models.ConfigEntities
{
    ///<summary>
    ///Objeto que representa a un objeto que hay dentro de la estructura ResourcesClasses del fichero json de configuración
    ///</summary>
    public class ResourcesClass
    {
        /// <summary>
        /// ResourceClass
        /// </summary>
        public string ResourceClass { get; set; }
        /// <summary>
        /// RdfType
        /// </summary>
        public string RdfType { get; set; }
        /// <summary>
        /// LabelResourceClass
        /// </summary>
        public string LabelResourceClass { get; set; }
        /// <summary>
        /// ResourceURI
        /// </summary>
        public string ResourceURI { get; set; }
        /// <summary>
        /// BlankNode
        /// </summary>
        public bool BlankNode { get; set; }
    }
}
