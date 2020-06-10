
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
