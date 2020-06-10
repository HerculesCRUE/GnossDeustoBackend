using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    ///<summary>
    ///Clase de ejemplo para mostrar un error a la hora de eliminar una estructura uri
    ///</summary>
    public class DeleteUriStructureErrorResponse : IExamplesProvider<UriErrorExample>
    {
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample()
            {
                Error = "No data of uriStructure uriExampleStructure"
            };
        }
    }
}
