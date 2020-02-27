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
    /// API de carga
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class repositoryController : Controller
    {
        /// <summary>
        /// Devuelve la información de la fuente de datos OAI-PMH configurada. 
        /// </summary>
        /// <returns>URI del repositorio OAI-PMH ¿JUNTO CON CONFIGURACIÓN DE SEGURIDAD OAUTH O HTTP BÁSICA?. Si no hay ninguna fuente configurada devuelve NULL</returns>
        [HttpGet("DataSource", Name = "DataSource")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DataSource()
        {
            return Ok("");
        }

        /// <summary>
        /// Configuración de la fuente de datos OAI-PMH. ¿JUNTO CON CONFIGURACIÓN DE SEGURIDAD OAUTH O HTTP BÁSICA?
        /// </summary>
        /// <param name="repositoryUri">URI del repositorio OAI-PMH</param>
        /// <returns>200 si verifica que los datos son correctos</returns>
        [HttpPut("SetDataSource", Name = "SetDataSource")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetDataSource(string repositoryUri)
        {
            return Ok("");
        }

        /// <summary>
        /// Recuperar un registro de metadatos individual del repositorio
        /// </summary>
        /// <param name="identifier">Identificador de la entidad a recolectar</param>
        /// <param name="metadataPrefix">Prefijo del metadata que se desea recuperar</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("GetRecord", Name = "GetRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string GetRecord(string identifier,string metadataPrefix)
        {

            return "";
        }

        /// <summary>
        /// Obtine la información del repositorio
        /// </summary>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("Identify", Name = "Identify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string Identify()
        {

            return "";
        }

        /// <summary>
        /// Es una forma abreviada de ListRecords, que recupera solo encabezados en lugar de registros.
        /// </summary>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva.</param>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListIdentifiers anterior que emitió una lista incompleta.</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListIdentifiers", Name = "ListIdentifiers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListIdentifiers(string metadataPrefix, DateTime? from=null,DateTime? until=null,string set=null,string resumptionToken=null)
        {

            return "";
        }



        /// <summary>
        /// Recupera los formatos de metadatos disponibles del repositorio
        /// </summary>
        /// <param name="identifier">Argumento opcional que especifica el identificador único del elemento para el que se solicitan los formatos de metadatos disponibles. Si se omite este argumento, la respuesta incluye todos los formatos de metadatos admitidos por este repositorio. </param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListMetadataFormats", Name = "ListMetadataFormats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListMetadataFormats(string identifier=null)
        {

            return "";
        }

        /// <summary>
        /// Recupera registros del repositorio
        /// </summary>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva.</param>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListRecords anterior que emitió una lista incompleta.</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListRecords", Name = "ListRecords")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListRecords(string metadataPrefix, DateTime? from = null, DateTime? until = null, string set = null, string resumptionToken = null)
        {

            return "";
        }

        /// <summary>
        /// Se utiliza para recuperar la estructura establecida de un repositorio, útil para la recolección selectiva.
        /// </summary>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListSets anterior que emitió una lista incompleta.</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListSets", Name = "ListSets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListSets(string resumptionToken = null)
        {

            return "";
        }
    }
}