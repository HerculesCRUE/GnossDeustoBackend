using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using UrisFactory.Models.Services;

namespace UrisFactory.Middlewares
{
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
