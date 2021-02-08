// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de cron
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para hacer llamadas al api de cron
    /// </summary>
    public class CallCronService : ICallService
    {
        readonly ConfigUrlCronService _serviceUrl;
        public CallCronService(ConfigUrlCronService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Hace una petición delete al apiCron
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api cron, se puede omitir en este método</param>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallDeleteApi(string urlBase, string urlMethod, TokenBearer token = null)
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
                string url = _serviceUrl.GetUrl();
                response = client.DeleteAsync($"{url}{urlMethod}").Result;
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

        /// <summary>
        /// Hace una petición get al apiCron
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api cron, se puede omitir en este método</param>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
        public string CallGetApi(string urlBase, string urlMethod, TokenBearer token = null)
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

                string url = _serviceUrl.GetUrl();
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

        /// <summary>
        /// Hace una petición post al apiCron
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api cron, se puede omitir en este método</param>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile", bool sparql = false)
        {
            string stringData = JsonConvert.SerializeObject(item);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                string url = _serviceUrl.GetUrl();
                response = client.PostAsync($"{url}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Hace una petición post al apiCron
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api cron, se puede omitir en este método</param>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
        /// <param name="files">diccionario de ficheros a pasar, en el que la clave es el nombre del parametro y el valor el fichero</param>
        public string CallPostApiFiles(string urlBase, string urlMethod, Dictionary<string, IFormFile> files, TokenBearer token = null)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Hace una petición put al apiCron
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api cron, se puede omitir en este método</param>
        /// <param name="urlMethod">Url del método dentro del apiCron</param>
        /// <param name="token">token bearer de seguridad</param>
        ///  <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public string CallPutApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile")
        {
            string stringData = JsonConvert.SerializeObject(item);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                string url = _serviceUrl.GetUrl();
                response = client.PutAsync($"{url}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }
    }
}
