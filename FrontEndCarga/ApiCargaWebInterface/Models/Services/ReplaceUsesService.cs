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
            dataModel.Results = new List<string>();
            Dictionary<int, string>  directiveList = Directives(htmlContent);
            foreach(var item in directiveList)
            {
                if (item.Value.Equals("api"))
                {
                    dataModel.Results.Add(Api(htmlContent, item.Key));
                }
                else if (item.Value.Equals("sparql"))
                {
                    dataModel.Results.Add(Sparql(htmlContent, item.Key));
                }
            }
            return dataModel;
        }

        private Dictionary<int, string> Directives(string htmlContent)
        {
            Dictionary<int, string> directives = new Dictionary<int, string>();
            int count = 0;
            int countFinal = 1;
            int first = 0;
            int last = 0;
            while (count < countFinal)
            {
                count = directives.Count;
                first = htmlContent.IndexOf(DirectivesList.Directive,last);
                if (first != -1)
                {
                    last = htmlContent.IndexOf("/%>*@", first);
                    string content = htmlContent.Substring(first, last - first);
                    if (content.Contains("api"))
                    {
                        directives.Add(first, "api");
                    }
                    else if (content.Contains("sparql"))
                    {
                        directives.Add(first, "sparql");
                    }
                }
                countFinal = directives.Count;
            }  
            return directives;
        }

        private string Api(string htmlContent, int ocurrence)
        {
            int first = htmlContent.IndexOf(DirectivesList.Api, ocurrence);
            first = first + DirectivesList.Api.Length;
            int last = htmlContent.IndexOf(DirectivesList.EndDirective, first);
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

        private string Sparql(string htmlContent, int ocurrence)
        {
            int first = htmlContent.IndexOf(DirectivesList.Sparql, ocurrence);
            first = first + DirectivesList.Sparql.Length;
            int last = htmlContent.IndexOf(DirectivesList.EndDirective, first);
            string query = $"{htmlContent.Substring(first, last - first)}";

            string url = $"{_configUrlService.GetSaprqlEndpoint()}?{_configUrlService.GetSparqlQuery()}={query}&format=text/csv";
            string result = _callService.CallGetApi(url, "");
            return result;
        }

    }
    public class PruebaSparql
    {
        public string type { get; set; }
        public int count { get; set; }
    }
}
