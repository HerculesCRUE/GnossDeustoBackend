// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Objeto que representa a un objeto que hay dentro de la estructura UriStructures del fichero json de configuración, que simula una estructura uri
using System.Collections.Generic;

namespace UrisFactory.Models.ConfigEntities
{
    ///<summary>
    ///Objeto que representa a un objeto que hay dentro de la estructura UriStructures del fichero json de configuración, que simula una estructura uri
    ///</summary>
    public class UriStructure
    {
        public string Name { get; set; }
        public IList<Component> Components { get; set; }
    }
}
