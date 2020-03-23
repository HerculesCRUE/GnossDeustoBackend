using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OaiPmhNet;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using OaiPmhNet.Models.OAIPMH;
using OaiPmhNet.Models.Services;
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
        private ConfigService _configService;
        private IOaiConfiguration _configOAI;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configService">Configuración del servicio</param>
        public OAI_PMHController(ConfigService configService)
        {
            _configService = configService;
            _configOAI = OaiConfiguration.Instance;
            _configOAI.SupportSets = _configService.GetConfig().SupportSets;
            _configOAI.RepositoryName = _configService.GetConfig().RepositoryName;
            _configOAI.AdminEmails = _configService.GetConfig().AdminEmails;
            _configOAI.DeletedRecord = _configService.GetConfig().DeletedRecord;
            _configOAI.Granularity = _configService.GetConfig().Granularity;
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
                Uri baseUri = new Uri(string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path));
                return baseUri.AbsoluteUri;
            };


            //MetadataFormatRepository
            MetadataFormatRepository metadataFormatRepository = new MetadataFormatRepository(_configService.GetConfig().MetadataFormats);

            RecordRepository recordRepository = new RecordRepository(_configOAI, _configService);

            //SetRepository
            SetRepository setRepository = new SetRepository(_configOAI, _configService.GetConfig().Sets);

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