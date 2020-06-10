using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    ///<summary>
    ///Clase de ejemplo para mostrar una insercción correcta de una estructura uri
    ///</summary>
    public class AddUriStructureResponse : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "uriStructure: uriExampleStructure has been added and the new config schema is loaded";
        }
    }
}
