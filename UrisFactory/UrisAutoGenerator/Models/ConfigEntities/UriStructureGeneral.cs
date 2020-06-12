// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Objeto que simula el fichero Json de configuración
using System.Collections.Generic;

namespace UrisFactory.Models.ConfigEntities
{
    ///<summary>
    ///Objeto que simula el fichero Json de configuración
    ///</summary>
    public class UriStructureGeneral
    {
        public string Base { get; set; }
        public IList<Characters> Characters { get; set; }
        public IList<UriStructure> UriStructures { get; set; }
        public IList<ResourcesClass> ResourcesClasses { get; set; }
    }
}
