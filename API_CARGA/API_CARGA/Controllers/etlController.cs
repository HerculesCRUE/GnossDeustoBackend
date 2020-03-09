using System;
using System.Collections.Generic;
using System.Linq;
using API_CARGA.Models.Entities;
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
    public class etlController : Controller
    {
        /// <summary>
        /// Ejecuta el último paso del proceso de carga, por el que el RDF generado se almacena en el Triple Store. Permite cargar una fuente RDF arbitraria.
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <returns></returns>
        [HttpPost("data-publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataPublish(IFormFile rdfFile)
        {

            return Ok("");
        }

        /// <summary>
        /// Valida un RDF mediante el shape SHACL configurado
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <returns></returns>
        [HttpPost("data-validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataValidate(IFormFile rdfFile)
        {

            return Ok("");
        }

        /// <summary>
        /// Reconcilia entidades y descubre enlaces o equivalencias. Permite efectuar el descubrimiento en fuentes RDF arbitrarias.
        /// </summary>
        /// <param name="rdfFile">Fichero RDF</param>
        /// <returns></returns>
        [HttpPost("data-discover")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult dataDiscover(IFormFile rdfFile)
        {

            return Ok("");
        }


        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera un registro de metadatos individual del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="identifier">Identificador de la entidad a recolectar</param>
        /// <param name="metadataPrefix">Prefijo del metadata que se desea recuperar</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("GetRecord/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string GetRecord(string repositoryIdentifier,string identifier, string metadataPrefix)
        {

            return "";
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Obtiene la información del repositorio OAI-PMH configurado en formato XML OAI-PMH.
        /// </summary>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("Identify/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string Identify(string repositoryIdentifier)
        {

            return "";
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Es una forma abreviada de ListRecords, que recupera solo encabezados en formato XML OAI-PMH en lugar de registros.        
        /// </summary>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva.</param>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListIdentifiers anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListIdentifiers/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListIdentifiers(string repositoryIdentifier, string metadataPrefix, DateTime? from = null, DateTime? until = null, string set = null, string resumptionToken = null)
        {

            return "";
        }


        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera los formatos de metadatos disponibles del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="identifier">Argumento opcional que especifica el identificador único del elemento para el que se solicitan los formatos de metadatos disponibles. Si se omite este argumento, la respuesta incluye todos los formatos de metadatos admitidos por este repositorio. </param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListMetadataFormats/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListMetadataFormats(string repositoryIdentifier, string identifier = null)
        {

            return "";
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recupera registros del repositorio en formato XML OAI-PMH.        
        /// </summary>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva.</param>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListRecords anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListRecords/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListRecords(string repositoryIdentifier, string metadataPrefix, DateTime? from = null, DateTime? until = null, string set = null, string resumptionToken = null)
        {

            return "";
        }

        /// <summary>
        /// Este método hace de PROXY entre el API y el proveedor OAI-PMH.
        /// Recuperar la estructura establecida de un repositorio en formato XML OAI-PMH, útil para la recolección selectiva.        
        /// </summary>
        /// <param name="resumptionToken">Argumento exclusivo con un valor que es el token de control de flujo devuelto por una solicitud ListSets anterior que emitió una lista incompleta.</param>
        /// <param name="repositoryIdentifier">Identificador del repositorio OAI-PMH</param>
        /// <returns>XML devuelto por el repositorio OAI-PMH</returns>
        [HttpGet("ListSets/{repositoryIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string ListSets(string repositoryIdentifier, string resumptionToken = null)
        {
            return "";
        }
    }
}