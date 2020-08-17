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
        public string ResourceClass { get; set; }
        public string RdfType { get; set; }
        public string LabelResourceClass { get; set; }
        public string ResourceURI { get; set; }
    }
}
