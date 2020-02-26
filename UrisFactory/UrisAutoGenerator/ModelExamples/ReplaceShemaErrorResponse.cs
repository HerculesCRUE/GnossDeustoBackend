using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    public class ReplaceShemaErrorResponse : IExamplesProvider<UriErrorExample>
    {
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample()
            {
                Error = "new config file is not correctly formed."
            };
        }
    }
}
