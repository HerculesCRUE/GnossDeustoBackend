// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Redireccionamiento a páginas de errores
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Clase para gestionar los errores y sus respectivos redireccionamientos
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Redirecciona a una página con el código de error dado
        /// </summary>
        /// <param name="code">codigo de error</param>
        /// <returns>página de error</returns>
        [Route("error/{code}")]
        public IActionResult ErrorHandler(int code)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (code == 404)
            {
                return View("Error404", feature.OriginalPath);

                //string route = "";
                //if (feature != null)
                //{
                //    route = feature.OriginalPath;
                //}
                //try
                //{

                //    return View(route, route);
                //}
                //catch (Exception ex)
                //{
                //    return View("Error404", route);
                //}

            }
            return View("General", feature.OriginalPath);
        }

        /// <summary>
        /// Redirecciona a una página de error cuando ocurre una excepción
        /// </summary>
        /// <returns>página de error</returns>
        [Route("error/exception")]
        public IActionResult ExceptionHandler()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("Exception", feature.Error);
        }
    }
}