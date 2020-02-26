using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    public class UrisFactoryErrorReponse : IExamplesProvider<UriErrorExample>
    {
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample()
            {
                Error = "resource class: 'resercher' not configured"
            };
        }
    }
}
