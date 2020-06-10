using API_CARGA.ViewModel;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    ///<summary>
    ///Sirve para mostrar un error al añadir un repositorio con un nombre repetido
    ///</summary>
    public class AddRepositoryErrorResponse : IExamplesProvider<ErrorExample>
    {
        public ErrorExample GetExamples()
        {
            return new ErrorExample
            {
                Error = "config repository {name} already exist"
            };
        }
    }
}
