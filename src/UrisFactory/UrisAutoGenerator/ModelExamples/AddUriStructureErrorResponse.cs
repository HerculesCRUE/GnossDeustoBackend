// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase de ejemplo para mostrar un error a la hora de añadir una estructura uri
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace UrisFactory.ModelExamples
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase de ejemplo para mostrar un error a la hora de añadir una estructura uri
    ///</summary>
    public class AddUriStructureErrorResponse : IExamplesProvider<UriErrorExample>
    {
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample()
            {
                Error = "UriStructure name: uriExamptructure and ResourcesClass ResourceURI: uriExampleStructure no match"
            };
        }
    }
}
