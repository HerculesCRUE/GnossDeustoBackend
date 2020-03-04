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

        [Route("error/{code}/{message}")]
        public IActionResult ErrorHandler(int code, string message)
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