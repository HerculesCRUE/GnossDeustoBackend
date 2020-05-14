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
    /// Controlador OAI-PMH
    /// </summary>
    [ApiController]
    [Route("[controller]")]
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
        /// <param name="identifier">Identificador</param>
        /// <param name="metadataPrefix">Prefijo del metadata</param>
        /// <param name="from">Fecha 'desde'</param>
        /// <param name="until">Fecha 'hasta'</param>
        /// <param name="set">Agrupador</param>
        /// <param name="resumptionToken">Token de reanudación</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult Get(OaiVerb verb, string identifier = "", string metadataPrefix = "", string from = "", string until = "", string set = "", string resumptionToken = "")
        {
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