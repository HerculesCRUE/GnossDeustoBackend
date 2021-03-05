using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.Asio.Api.Carga.Models.Services
{
    public class CallConversor
    {
        readonly TokenBearer _token;
        public CallConversor(CallTokenService tokenService)
        {
            if (tokenService != null)
            {
                _token = tokenService.CallTokenConversor();
            }
        }

        /// <summary>
        /// Obtiene el resultado
        /// </summary>
        /// <param name="url">url a llamar</param>
        /// <returns></returns>
        public string GetString(string url)
        {
            string result;
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (_token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{_token.token_type} {_token.access_token}");
                }
                try
                {
                    response = client.GetAsync(url).Result;
                }
                catch (TaskCanceledException ex)
                {
                    throw new TaskCanceledException(ex.Message);
                }
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
        /// Obtiene el resultado
        /// </summary>
        /// <param name="pRecord">XML con el record</param>
        /// <param name="pType">Tipo</param>
        /// <returns></returns>
        public string GetRDF(string pRecord,string pType,string pUrlConversor)
        {
            MultipartFormDataContent contentData = contentData = new MultipartFormDataContent();
            byte[] bytedata = Encoding.UTF8.GetBytes(pRecord);
            ByteArrayContent bytes = new ByteArrayContent(bytedata);
            contentData.Add(bytes, "pXmlFile", "pXmlFile");

            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                if (_token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{_token.token_type} {_token.access_token}");
                }
                client.Timeout = TimeSpan.FromMinutes(15);
                response = client.PostAsync($"{pUrlConversor}Conversor/Convert?pType=" + pType, contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
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
