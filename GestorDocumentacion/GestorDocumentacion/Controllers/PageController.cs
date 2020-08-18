using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GestorDocumentacion.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PageController : ControllerBase
    {
        ///<summary>
        ///Devuelve el HTML de una página web, incluyendo sus metadatos.
        ///</summary>
        ///<remarks>
        ///<param name="name">nombre del fichero html</param>
        [HttpGet("{name}")]
        public IActionResult GetPage(String name)
        {
            return null;
        }

        /// <summary>
        /// Devuelve una lista de las páginas web cargadas.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/list")]
        public IActionResult GetPages()
        {
            return null;
        }

        /// <summary>
        /// Carga o modifica una página web e incluye información acerca de la página, como la URL, metadatos title o description, etc.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]/load")]
        public IActionResult LoadPage()
        {
            return null;
        }

        /// <summary>
        /// Elimina una página web.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("[Controller]/delete")]
        public IActionResult DeletePage()
        {
            return null;
        }
    }
}
