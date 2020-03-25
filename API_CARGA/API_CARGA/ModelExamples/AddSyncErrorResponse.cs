using API_CARGA.ViewModel;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    public class AddSyncErrorResponse : IExamplesProvider<ErrorExample>
    {
        public ErrorExample GetExamples()
        {
            return new ErrorExample
            {
                Error = "config sync {name} already exist"
            };
        }
    }
}
