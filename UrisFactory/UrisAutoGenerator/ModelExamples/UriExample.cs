using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    public class UriExample: IExamplesProvider<List<Uri>>
    {
        public List<Uri> GetExamples()
        {
            return new List<Uri>
            {
                new Uri{UriResult ="asadas"},
                new Uri{UriResult ="asadas"}
            };
        }
 
    }
}
