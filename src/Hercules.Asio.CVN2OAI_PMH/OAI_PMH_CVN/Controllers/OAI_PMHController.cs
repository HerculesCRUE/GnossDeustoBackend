﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador OAI-PMH que sirve los datos de los curículums de los investigadores de la Universidad de Murcia en formato RDF.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAI_PMH_CVN.Models.Services;
using OaiPmhNet;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using OaiPmhNet.Models.OAIPMH;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace OAI_PMH.Controllers
{
    /// <summary>
    /// Controlador OAI-PMH que sirve los datos de los curículums de los investigadores de la Universidad de Murcia en formato RDF.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OAI_PMHController : Controller
    {
        private IOaiConfiguration _configOAI;

        readonly ConfigOAI_PMH_CVN _configOAI_PMH_CVN;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configOAI_PMH_CVN">Configuración del servicio</param>
        public OAI_PMHController(ConfigOAI_PMH_CVN configOAI_PMH_CVN)
        {
            _configOAI_PMH_CVN = configOAI_PMH_CVN;
            _configOAI = OaiConfiguration.Instance;
            _configOAI.SupportSets = true;
            _configOAI.RepositoryName = "OAI_PMH_CVN";
            _configOAI.Granularity = "yyyy-MM-ddTHH:mm:ssZ";
        }



        /// <summary>
        /// API OAI-PMH para la recuperación de CVN, más información del protocolo OAI-PMH visita https://www.openarchives.org/OAI/openarchivesprotocol.html
        /// </summary>
        /// <param name="verb">Verbo OAI-PMH</param>
        /// <param name="identifier">Identificador de la entidad a recuperar (los identificadores se obtienen con el metodo ListIdentifiers)</param>
        /// <param name="metadataPrefix">Especifica que los encabezados deben devolverse solo si el formato de metadatos que coincide con el metadataPrefix proporcionado está disponible o, según el soporte del repositorio para las eliminaciones, se ha eliminado. Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListMetadataFormats.</param>
        /// <param name="from">Fecha de inicio desde la que se desean recuperar las cabeceras de las entidades (Codificado con ISO8601 y expresado en UTC, YYYY-MM-DD o YYYY-MM-DDThh:mm:ssZ)</param>
        /// <param name="until">Fecha de fin hasta la que se desean recuperar las cabeceras de las entidades (Codificado con ISO8601 y expresado en UTC, YYYY-MM-DD o YYYY-MM-DDThh:mm:ssZ)</param>
        /// <param name="set">Argumento con un valor setSpec, que especifica los criterios establecidos para la recolección selectiva. Los formatos de sets admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud ListSets.</param>
        /// <param name="resumptionToken">Token de reanudación (No disponible)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult Get(OaiVerb verb, string identifier = "", string metadataPrefix = "", string from = "", string until = "", string set = "", string resumptionToken = "")
        {
            if(!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(until))
            {
                until = "3000-01-01T00:00:00Z";
            }
            //CONFIG OAI-PMH
            _configOAI.BaseUrl = () =>
            {
                Uri baseUri = new Uri(_configOAI_PMH_CVN.GetConfigUrl());
                return baseUri.AbsoluteUri;
            };
         

            //MetadataFormatRepository
            MetadataFormatRepository metadataFormatRepository = new MetadataFormatRepository();

            RecordRepository recordRepository = new RecordRepository(_configOAI, _configOAI_PMH_CVN);

            //SetRepository
            SetRepository setRepository = new SetRepository(_configOAI);

            DataProvider provider = new DataProvider(_configOAI, metadataFormatRepository, recordRepository, setRepository);

            ArgumentContainer arguments = new ArgumentContainer(verb.ToString(), metadataPrefix, resumptionToken, identifier, from, until, set);
            XDocument document = provider.ToXDocument(DateTime.Now.AddMinutes(100), arguments);

            var memoryStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(memoryStream);

            document.WriteTo(xmlWriter);
            xmlWriter.Flush();
            byte[] array = memoryStream.ToArray();
            return File(array, "application/xml");
        }
    }
}