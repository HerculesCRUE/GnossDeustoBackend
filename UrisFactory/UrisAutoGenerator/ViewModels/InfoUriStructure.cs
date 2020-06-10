// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada para devolver una estructura uri desde los controladores
using System.Collections.Generic;
using UrisFactory.Models.ConfigEntities; 

namespace UrisFactory.ViewModels
{
    ///<summary>
    ///Clase usada para devolver una estructura uri desde los controladores
    ///</summary>
    public class InfoUriStructure
    {
        public UriStructure UriStructure{get;set;}
         
        public List<ResourcesClass> ResourcesClass { get; set; }
    }
}
