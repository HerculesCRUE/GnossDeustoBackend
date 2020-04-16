﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OAI_PMH_CVN.Models.Services;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using OaiPmhNet.Models.Services;
using RestSharp;

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
            CVN CVN = GetCurriculum(identifier, false,_configService.GetConfig().XML_CVN_Repository);
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
            HashSet<string> ids = GetCurriculumsIDs(inicio, _configService.GetConfig().XML_CVN_Repository);
            List<CVN> listCVN = new List<CVN>();
            foreach (string id in ids)
            {
                listCVN.Add(GetCurriculum(id, arguments.Verb == OaiVerb.ListIdentifiers.ToString(), _configService.GetConfig().XML_CVN_Repository));
            }
            if (arguments.Verb == OaiVerb.ListIdentifiers.ToString())
            {
                container.Records = listCVN.Select(r => ToIdentifiersRecord(r));
            }
            else
            {
                container.Records = listCVN.Select(r => ToRecord(r, arguments.MetadataPrefix));

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
                    Identifier = pCVN.Id.ToString()
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
                    Identifier = pCVN.Id.ToString()
                }
            };

            switch (pMetadataPrefix)
            {
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
        /// <param name="pXML_CVN_Repository">Ruta del repositorio de CVN</param>
        /// <returns>Identificadores de os curriculums</returns>
        private HashSet<string> GetCurriculumsIDs(DateTime pInicio, string pXML_CVN_Repository)
        {
            var client = new RestClient($"{pXML_CVN_Repository}changes?date={pInicio.Year}-{pInicio.Month}-{pInicio.Day}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("application", "asio");
            request.AddHeader("key", "asiokey");
            XML_CVN_Repository_Response respuesta = JsonConvert.DeserializeObject<XML_CVN_Repository_Response>(client.Execute(request).Content);
            return new HashSet<string>(respuesta.ids.Select(x=>x.ToString()));
        }

        /// <summary>
        /// Obtiene un CVN
        /// </summary>
        /// <param name="pId">Identificador</param>
        /// <param name="pOnlyIDs">Obtiene solo el CVN con el IDID</param>
        /// <param name="pXML_CVN_Repository">Ruta del repositorio de CVN</param>
        /// <returns></returns>
        private CVN GetCurriculum(string pId, bool pOnlyIDs, string pXML_CVN_Repository)
        {
            string xml = "";
            if (!pOnlyIDs)
            {
                var client = new RestClient($"{pXML_CVN_Repository}cvn?id={pId}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("application", "asio");
                request.AddHeader("key", "asiokey");
                xml = client.Execute(request).Content;
            }
            return new CVN(xml, pId, _configService.GetConfig().CVN_ROH_converter);
        }

    }
}