using API_CARGA.ViewModel;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    ///<summary>
    ///Sirve para mostrar un error de respuesta al intentar modificar un repositorio que no existe
    ///</summary>
    public class ModifyRepositoryErrorResponse : IExamplesProvider<ErrorExample>
    {
        public ErrorExample GetExamples()
        {
            return new ErrorExample
            {
                Error = "Check that repository config with id {identifier} exist"
            };
        }
    }
}
