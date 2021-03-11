// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para realizar llamadas al api de uris factory
using API_DISCOVER.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// clase para realizar llamadas al api de uris factory
    /// </summary>
    public class CallUrisFactoryApiService : ICallUrisFactoryApiService
    {
        readonly TokenBearer _token;
        readonly ConfigUrlService _serviceUrl;
        public CallUrisFactoryApiService(CallTokenService tokenService,ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenUrisFactory();
            }
        }

        /// <summary>
        /// Obtiene una rui
        /// </summary>
        /// <param name="resourceClass">Resource class o rdfType</param>
        /// <param name="identifier">Identificador</param>
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier)
        {
            string result = CallGetApi($"Factory?identifier={HttpUtility.UrlEncode(identifier)}&resource_class={HttpUtility.UrlEncode(resourceClass)}", _token);
            return result;
        }

        /// <summary>
        /// Hace una petición get al urisFactory
        /// </summary>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallGetApi(string urlMethod, TokenBearer token = null)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();

                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }

                string url = _serviceUrl.GetUrlUrisFactory();
                response = client.GetAsync($"{url}{urlMethod}").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
            return result;
        }
    }
}
