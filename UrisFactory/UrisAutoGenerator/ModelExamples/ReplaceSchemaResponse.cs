using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.ModelExamples
{
    ///<summary>
    ///Clase de ejemplo para mostrar el correcto reemplazo de un fichero de configuración
    ///</summary>
    public class ReplaceSchemaResponse : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "new config file loaded";
        }

    }
}
