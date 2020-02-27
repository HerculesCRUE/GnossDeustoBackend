using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OaiPmhNet;
using OaiPmhNet.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PMH.Controllers
{
    /// <summary>
    /// Configuración
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class settingsController : Controller
    {
        /*
         * dataSourceList-->Esto entiendo que no tiene sentido, porque sólo habrá una fuente
         * dataSynchro-->Esto entiendo que no tiene sentido, ya que la forma de realizar la sincro no debería estar definida por la fuente de datos, esto se decidirá en 'Gestión Procesos de Cargas'
         * dataSourceType-->Esto entiendo que no tiene sentido, porque sólo hay un tipo OAI-PMH
         * dataMapLoad-->Esto entiendo que no tiene sentido, no son necesarios los mapeos porque desde el OAI-PMH ya debería devolverse en formato RDF válido
         * dataMap-->Esto entiendo que no tiene sentido, mismo caso que el punto anterior
         * dataShapeLoad-->Esto entiendo que no tiene sentido aquí, la validación entiendo que no es e función de la fuente de datos, será en función de las ontologías configuradas, habría que hacerla antes de publicar el RDF independientemente de la fuente
         * dataShape-->Esto entiendo que no tiene sentido, mismo caso que el punto anterior
         */
    }
}