// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve para mostrar un error al añadir un repositorio con un nombre repetido
using API_CARGA.ViewModel;
using Swashbuckle.AspNetCore.Filters;

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
