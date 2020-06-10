using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
