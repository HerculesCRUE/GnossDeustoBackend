using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    public class DeleteUriStructureResponse : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "uriStructure: uriExampleStructure has been deleted and the new config schema is loaded";
        }
    }
}
