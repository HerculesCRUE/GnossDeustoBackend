using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    ///<summary>
    ///Clase de ejemplo para mostrar un error a la hora de añadir una estructura uri
    ///</summary>
    public class AddUriStructureErrorResponse : IExamplesProvider<UriErrorExample>
    {
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample()
            {
                Error = "UriStructure name: uriExamptructure and ResourcesClass ResourceURI: uriExampleStructure no match"
            };
        }
    }
}
