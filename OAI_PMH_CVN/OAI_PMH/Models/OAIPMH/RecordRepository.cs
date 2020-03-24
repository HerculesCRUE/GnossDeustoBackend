using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using OaiPmhNet.Models.Services;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Implementación de IRecordRepository
    /// </summary>
    public class RecordRepository : IRecordRepository
    {
        private readonly IOaiConfiguration _configurationOAI;
        private readonly ConfigService _configService;
        private readonly IDateConverter _dateConverter;
        private readonly IDublinCoreMetadataConverter _dublinCoreMetadataConverter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationOAI">Configuración OAI-PMH</param>
        /// <param name="configService">Configuración del servicio</param>
        public RecordRepository(IOaiConfiguration configurationOAI, ConfigService configService)
        {
            _configurationOAI = configurationOAI;
            _configService = configService;
            _dateConverter = new DateConverter();
            _dublinCoreMetadataConverter = new DublinCoreMetadataConverter(configurationOAI, _dateConverter);
        }

        /// <summary>
        /// Obtiene un Record OAI-PMH
        /// </summary>
        /// <param name="identifier">Identificador del record</param>
        /// <param name="metadataPrefix">Prefijo del metadata</param>
        /// <returns></returns>
        public Record GetRecord(string identifier, string metadataPrefix)
        {
            CVN CVN = GetCurriculum(identifier);
            return ToRecord(CVN, metadataPrefix);
        }

        /// <summary>
        /// Obtiene los records del repositorio en función de los argumentos pasados
        /// </summary>
        /// <param name="arguments">Parámetros de la consulta</param>
        /// <param name="resumptionToken">Token de reanudación</param>
        /// <returns></returns>
        public RecordContainer GetRecords(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            RecordContainer container = new RecordContainer();
            DateTime inicio = DateTime.MinValue;
            if (_dateConverter.TryDecode(arguments.From, out DateTime from))
            {
                inicio = from;
            }

            HashSet<string> ids = GetCurriculumsIDs(inicio);
            List<CVN> listCVN = new List<CVN>();
            foreach (string id in ids)
            {
                listCVN.Add(GetCurriculum(id));
            }

            if (arguments.Verb == OaiVerb.ListIdentifiers.ToString())
            {
                container.Records = listCVN.Select(r => ToRecord(r, arguments.MetadataPrefix));
            }
            else
            {
                container.Records = listCVN.Select(r => ToIdentifiersRecord(r));
            }
            return container;
        }

        /// <summary>
        /// Obtiene los identificadores del repositorio en función de los argumentos pasados
        /// </summary>
        /// <param name="arguments">Parámetros de la consulta</param>
        /// <param name="resumptionToken">Token de reanudación</param>
        /// <returns></returns>
        public RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            return GetRecords(arguments, resumptionToken);
        }

        /// <summary>
        /// Convierte un CVN en un Record OAI-PMH sólo con cabecera
        /// </summary>
        /// <param name="pCVN">CVN</param>
        /// <returns>Record OAI-PMH</returns>
        private Record ToIdentifiersRecord(CVN pCVN)
        {
            if (pCVN == null)
                return null;
            Record record = new Record()
            {
                Header = new RecordHeader()
                {
                    Identifier = pCVN.Id.ToString(),
                    Datestamp = pCVN.Date
                }
            };

            return record;
        }

        /// <summary>
        /// Convierte un CVN en un Record OAI-PMH completo
        /// </summary>
        /// <param name="pCVN">CVN</param>
        /// <param name="pMetadataPrefix">Prefijo de metadatos</param>
        /// <returns>Record OAI-PMH</returns>
        private Record ToRecord(CVN pCVN, string pMetadataPrefix)
        {
            if (pCVN == null)
                return null;
            Record record = new Record()
            {
                Header = new RecordHeader()
                {
                    Identifier = pCVN.Id.ToString(),
                    Datestamp = pCVN.Date
                }
            };

            switch (pMetadataPrefix)
            {
                case "oai_dc":
                    record.Metadata = new RecordMetadata()
                    {
                        Content = _dublinCoreMetadataConverter.Encode(new DublinCoreMetadata()
                        {
                            Title = new List<string>() { pCVN.Name },
                            Date = new List<DateTime>() { pCVN.Date },
                            Identifier = new List<string> { pCVN.Id.ToString() }
                        })
                    };
                    break;
                case "rdf":
                    record.Metadata = new RecordMetadata()
                    {
                        Content = System.Xml.Linq.XElement.Parse(pCVN.rdf)
                    };
                    break;
            }
            return record;
        }

        /// <summary>
        /// Obtiene los IDs de los curriculums desde una fecha de inicio
        /// </summary>
        /// <param name="pInicio">Fecha de inicio</param>
        /// <returns>Identificadores de os curriculums</returns>
        private HashSet<string> GetCurriculumsIDs(DateTime pInicio)
        {
            HashSet<string> listaIDS = new HashSet<string>();
            listaIDS.Add("0000-0001-8055-6823");//Diego
            listaIDS.Add("0000-0002-7558-2880");//Jesualdo
            return listaIDS;

            /* [ENTORNO]/curriculum/rest/v1/auth/changes?date=[YYYY-MM-DD]	GET	user y key	HTTP 200	"ids": {1, 2, 3, ...} */
            //string responseString = "";
            //WebRequest request = WebRequest.Create(
            //  $"{_configJsonHandler.GetConfig().XML_CVN_Repository}/curriculum/rest/v1/auth/changes?date=[{pInicio.Year}-{pInicio.Month}-{pInicio.Day}]");
            //WebResponse response = request.GetResponse();
            //using (Stream dataStream = response.GetResponseStream())
            //{
            //    StreamReader reader = new StreamReader(dataStream);
            //    responseString = reader.ReadToEnd();
            //}
            //// Close the response.  
            //response.Close();
            //return JsonConvert.DeserializeObject<HashSet<string>>(responseString);          
        }

        private CVN GetCurriculum(string pId)
        {
            string responseString = System.IO.File.ReadAllText($"Config/{pId}.xml");
            return new CVN(responseString,pId, _configService.GetConfig().PythonExe, _configService.GetConfig().PythonScript);

            //WebRequest request = WebRequest.Create(
            //  $"{_configJsonHandler.GetConfig().XML_CVN_Repository}/curriculum/rest/v1/auth/cvn?id=[{pId}]");
            //WebResponse response = request.GetResponse();
            //using (Stream dataStream = response.GetResponseStream())
            //{
            //    StreamReader reader = new StreamReader(dataStream);
            //    responseString = reader.ReadToEnd();
            //}
            //// Close the response.  
            //response.Close();

            //return new CVN(responseString);


            /*[ENTORNO]/curriculum/rest/v1/auth/cvn?id=[identificador]	GET	user y key	HTTP 200	XML*/
            //string responseString = "";
            //WebRequest request = WebRequest.Create(
            //  $"{_configJsonHandler.GetConfig().XML_CVN_Repository}/curriculum/rest/v1/auth/cvn?id=[{pId}]");
            //WebResponse response = request.GetResponse();
            //using (Stream dataStream = response.GetResponseStream())
            //{
            //    StreamReader reader = new StreamReader(dataStream);
            //    responseString = reader.ReadToEnd();
            //}
            //// Close the response.  
            //response.Close();

            //return new CVN(responseString);
        }

    }
}
