// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas a los métodos del apiCron
using API_DISCOVER.Models.Entities;
using API_DISCOVER.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using System.Collections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// Servicio para hacer llamadas a los métodos del apiCron
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CallCronApiService
    {
        readonly ConfigUrlService _serviceUrl;
        readonly TokenBearer _token;
        /// <summary>
        /// CallCronApiService
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="tokenService"></param>
        public CallCronApiService(ConfigUrlService serviceUrl, CallTokenService tokenService)
        {
            _serviceUrl = serviceUrl;
            if (tokenService != null)
            {
                _token = tokenService.CallTokenCron();                
            }
        }
        
        /// <summary>
        /// Obtiene una tarea
        /// </summary>
        /// <param name="id">identificador de la tarea</param>
        /// <returns>una tarea</returns>
        public JobViewModel GetJob(string id)
        {
            string result = CallGetApi($"Job/{id}", _token);
            JobViewModel resultObject = JsonConvert.DeserializeObject<JobViewModel>(result);
            return resultObject;
        }
        /// <summary>
        /// Hace una petición get al apiCron
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

                string url = _serviceUrl.GetUrlCron();
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
