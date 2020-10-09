using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Utility;
using ApiCargaWebInterface.ViewModels;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class ReplaceUsesService
    {
        public ICallService _callService;
        public ConfigUrlService _configUrlService;
        public ConfigUrlCronService _configUrlCronService;
        CallTokenService _callTokenService;
        public ReplaceUsesService(ICallService callService, ConfigUrlService configUrlService, ConfigUrlCronService configUrlCronService, CallTokenService callTokenService)
        {
            _callService = callService;
            _configUrlService = configUrlService;
            _configUrlCronService = configUrlCronService;
            _callTokenService = callTokenService;
        }
        public CmsDataViewModel PageWithDirectives(string htmlContent, CmsDataViewModel dataModel)
        {
            
            if (htmlContent.Contains(DirectivesList.Api))
            {
                dataModel.Results = Api(htmlContent);
            }
            else if (htmlContent.Contains(DirectivesList.Sparql))
            {
                dataModel.Results = Sparql(htmlContent);
            }

            return dataModel;
        }

        private string Api(string htmlContent)
        {
            int first = htmlContent.IndexOf(DirectivesList.Api);
            first = first + DirectivesList.Api.Length;
            int last = htmlContent.LastIndexOf("/%>*@");
            string url = htmlContent.Substring(first, last - first).Trim();
            TokenBearer token = null;
            if (url.Contains(_configUrlService.GetUrl()))
            {
                token = _callTokenService.CallTokenCarga();
            }
            else if (url.Contains(_configUrlService.GetUrlDocumentacion()))
            {
                token = _callTokenService.CallTokenApiDocumentacion();
            }
            else if (url.Contains(_configUrlCronService.GetUrl()))
            {
                token = _callTokenService.CallTokenCron();
            }
            string result = _callService.CallGetApi(url, "", token);
            return result;
        }

        private string Sparql(string htmlContent)
        {
            int first = htmlContent.IndexOf(DirectivesList.Sparql);
            first = first + DirectivesList.Sparql.Length;
            int last = htmlContent.LastIndexOf("/%>*@");
            string query = $"{htmlContent.Substring(first, last - first)}";

            string url = $"{_configUrlService.GetSaprqlEndpoint()}?{_configUrlService.GetSparqlQuery()}={query}&format=text/csv";
            string result = _callService.CallGetApi(url, "");
            byte[] byteArray = Encoding.UTF8.GetBytes(result);
            MemoryStream stream = new MemoryStream(byteArray);
            var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<PruebaSparql>();
            foreach (var record in records)
            {
               int count = record.count;
                string type = record.type;
            }
            return result;
        }

    }
    public class PruebaSparql
    {
        public string type { get; set; }
        public int count { get; set; }
    }
}
