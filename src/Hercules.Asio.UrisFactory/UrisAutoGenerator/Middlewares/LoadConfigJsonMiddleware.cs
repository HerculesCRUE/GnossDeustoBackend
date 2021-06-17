// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que hace de middleware y carga el fichero de configuración
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace UrisFactory.Middlewares
{
    ///<summary>
    ///Clase que hace de middleware y carga el fichero de configuración
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class LoadConfigJsonMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// LoadConfigJsonMiddleware
        /// </summary>
        /// <param name="next"></param>
        public LoadConfigJsonMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            await _next(context);
        }
    }
}
