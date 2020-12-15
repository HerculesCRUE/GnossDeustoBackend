// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que hace de middleware y carga el fichero de configuración
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace UrisFactory.Middlewares
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase que hace de middleware y carga el fichero de configuración
    ///</summary>
    public class LoadConfigJsonMiddleware
    {
        private readonly RequestDelegate _next;
        public LoadConfigJsonMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            //ConfigJsonHandler.InitializerConfigJson();
            await _next(context);
        }
    }
}
