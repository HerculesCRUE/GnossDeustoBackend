// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para hacer llamadas api
using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    /// <summary>
    /// Clase para hacer llamadas api
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CallApiService : ICallService
    {
        /// <summary>
        /// Hace la llamada post a una url
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">url del método</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="token">token bearer en caso de que sea necesario para la autenticación</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero</param>
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile")
        {
            HttpContent contentData = null;
            if (!isFile)
            {
                string stringData = JsonConvert.SerializeObject(item);
                contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                byte[] data;
                using (var br = new BinaryReader(((IFormFile)item).OpenReadStream()))
                {
                    data = br.ReadBytes((int)((IFormFile)item).OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                contentData = new MultipartFormDataContent();
                ((MultipartFormDataContent)contentData).Add(bytes, fileName, ((IFormFile)item).FileName);
            }
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
                }
                client.Timeout = TimeSpan.FromMinutes(60);
                response = client.PostAsync($"{urlBase}{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {

                if (response != null && !string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (response != null)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
        }
    }
}
