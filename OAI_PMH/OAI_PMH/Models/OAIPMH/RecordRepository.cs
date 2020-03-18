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
    public class RecordRepository : IRecordRepository
    {
        private readonly IOaiConfiguration _configurationOAI;
        private readonly ConfigJson _configJsonHandler;
        private readonly IDateConverter _dateConverter;
        private readonly IDublinCoreMetadataConverter _dublinCoreMetadataConverter;

        public RecordRepository(IOaiConfiguration configurationOAI, ConfigJson configJsonHandler)
        {
            _configurationOAI = configurationOAI;
            _configJsonHandler = configJsonHandler;
            _dateConverter = new DateConverter();
            _dublinCoreMetadataConverter = new DublinCoreMetadataConverter(configurationOAI, _dateConverter);
        }

        public Record GetRecord(string identifier, string metadataPrefix)
        {
            CVN CVN = GetCurriculum(identifier);
            return ToRecord(CVN, metadataPrefix);
        }

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

        public RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            return GetRecords(arguments, resumptionToken);
        }

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
                        Content = System.Xml.Linq.XElement.Parse(pCVN.rdf_cvn)
                    };
                    break;
            }
            return record;
        }


        private HashSet<string> GetCurriculumsIDs(DateTime pInicio)
        {
            /* [ENTORNO]/curriculum/rest/v1/auth/changes?date=[YYYY-MM-DD]	GET	user y key	HTTP 200	"ids": {1, 2, 3, ...} */

            string responseString = "";
            WebRequest request = WebRequest.Create(
              $"{_configJsonHandler.GetConfig().XML_CVN_Repository}/curriculum/rest/v1/auth/changes?date=[{pInicio.Year}-{pInicio.Month}-{pInicio.Day}]");
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseString = reader.ReadToEnd();
            }
            // Close the response.  
            response.Close();

            return JsonConvert.DeserializeObject<HashSet<string>>(responseString);
        }

        private CVN GetCurriculum(string pId)
        {
            /*[ENTORNO]/curriculum/rest/v1/auth/cvn?id=[identificador]	GET	user y key	HTTP 200	XML*/

            string responseString = "";
            WebRequest request = WebRequest.Create(
              $"{_configJsonHandler.GetConfig().XML_CVN_Repository}/curriculum/rest/v1/auth/cvn?id=[{pId}]");
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseString = reader.ReadToEnd();
            }
            // Close the response.  
            response.Close();

            return new CVN(responseString);
        }

    }
}
