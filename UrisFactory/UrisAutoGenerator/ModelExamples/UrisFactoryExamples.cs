using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    public class UrisFactoryExamples: IExamplesProvider<List<Uri>>
    {
        public List<Uri> GetExamples()
        {
            return new List<Uri>
            {
                new Uri{Parameters = "Factory?resource_class=Researcher&identifier=121s", UriResult ="http://data.um.es/res/researcher/121s"},
                new Uri{Parameters = "Factory?resource_class=Keyword&identifier=4hda11182",UriResult ="http://data.um.es/kos/keyword/4hda11182"}
            };
        }
 
    }
}
