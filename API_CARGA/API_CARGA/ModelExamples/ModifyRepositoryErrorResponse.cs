using API_CARGA.ViewModel;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    public class ModifyRepositoryErrorResponse : IExamplesProvider<UriErrorExample>
    {
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample
            {
                Error = "Check that repository config with id {identifier} exist"
            };
        }
    }
}
