using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestorDocumentacion.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        ///<summary>
        ///Devuelve una plantilla HTML, incluyendo sus metadatos.
        ///</summary>
        ///<remarks>
        ///<param name="name">nombre del fichero html</param>
        [HttpGet("{name}")]
        public IActionResult GetTemplate(String name)
        {
            return null;
        }

        ///<summary>
        ///Devuelve una lista de las plantillas cargadas.
        ///</summary>
        [HttpGet]
        [Route("[Controller]/list")]
        public IActionResult GetTemplates()
        {
            return null;
        }

        /// <summary>
        /// Carga o modifica una plantilla web e incluye información acerca de la plantilla, como metadatos title o description.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Controller]/load")]
        public IActionResult LoadTemplate()
        {
            return null;
        }

        /// <summary>
        /// Elimina una plantilla
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("[Controller]/delete")]
        public IActionResult DeleteTemplate()
        {
            return null;
        }
    }
}