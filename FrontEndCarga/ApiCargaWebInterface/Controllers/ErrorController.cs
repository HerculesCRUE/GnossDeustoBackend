// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Redireccionamiento a páginas de errores
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{code}")]
        public IActionResult ErrorHandler(int code)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (code == 404)
            {
                return View("Error404", feature.OriginalPath);
            }
            return View("General", feature.OriginalPath);
        }


        [Route("error/exception")]
        public IActionResult ExceptionHandler()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("Exception", feature.Error);
        }
    }
}