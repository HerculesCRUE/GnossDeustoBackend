// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para hacer llamadas al api de cron
using ApiCargaWebInterface.Extra.Exceptions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallCronService : ICallService
    {
        readonly ConfigUrlCronService _serviceUrl;
        public CallCronService(ConfigUrlCronService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public string CallDeleteApi(string urlMethod)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
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

        public string CallGetApi(string urlMethod)
        {
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
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

        public string CallPostApi(string urlMethod, object item, bool isFile = false)
        {
            string stringData = JsonConvert.SerializeObject(item);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
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

        public string CallPutApi(string urlMethod, object item, bool isFile = false)
        {
            string stringData = JsonConvert.SerializeObject(item);
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
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
