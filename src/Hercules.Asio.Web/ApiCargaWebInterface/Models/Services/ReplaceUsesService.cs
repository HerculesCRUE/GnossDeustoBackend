// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para cargar en el modelo los resultados de las directivas
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Utility;
using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Clase para cargar en el modelo los resultados de las directivas
    /// </summary>
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
        /// <summary>
        /// Método que carga en el modelo los resultados obtenidos al llamar a los servicios especificados en las directivas
        /// </summary>
        /// <param name="htmlContent">html de la página con las directivas</param>
        /// <param name="dataModel">modelo donde cargar los datos</param>
        /// <returns>Modelo con los datos cargados</returns>
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

        /// <summary>
        /// Método que devuelve un diccionario con todos los tipos de directiva y posición de su aparición
        /// </summary>
        /// <param name="htmlContent">Contenido de la página</param>
        /// <returns>Diccionario con clave su posición de aparición y valor el tipo de directiva</returns>
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
        /// <summary>
        /// Obtiene el resultado en formato json de la llamada al api
        /// </summary>
        /// <param name="htmlContent">contenido html</param>
        /// <param name="ocurrence">Posición de la cual hay que mirar</param>
        /// <returns>resultado de la llamada en formato json</returns>
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
        /// <summary>
        /// obtiene el formato en csv de la consulta sparql obtenida de la directiva
        /// </summary>
        /// <param name="htmlContent">contenido html</param>
        /// <param name="ocurrence">Posición de la cual hay que mirar</param>
        /// <returns>Resultado de la llamada en formato csv</returns>
        private string Sparql(string htmlContent, int ocurrence)
        {
            int first = htmlContent.IndexOf(DirectivesList.Sparql, ocurrence);
            first = first + DirectivesList.Sparql.Length;
            int last = htmlContent.IndexOf(DirectivesList.EndDirective, first);
            string queryS = $"{htmlContent.Substring(first, last - first)}";

            string url = $"{_configUrlService.GetSaprqlEndpoint()}?{_configUrlService.GetSparqlQuery()}={queryS}&format=text/csv";
            string consulta = HttpUtility.UrlEncode(queryS);
            consulta = $"query={consulta}&format=text/csv";
            //string result = _callService.CallGetApi(url, "");
            string result = _callService.CallPostApi(_configUrlService.GetSaprqlEndpoint(),"",consulta, sparql : true);
            return result;
        }

    }
}
