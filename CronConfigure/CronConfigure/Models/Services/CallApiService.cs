using CronConfigure.Exceptions;
using CronConfigure.Models.Entitties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Sirve para hacer llamadas a un API cuya url base esta configurada en UrlConfig del appSettings.json
    ///</summary>
    public class CallApiService
    {
        readonly ConfigUrlService _serviceUrl;
        public CallApiService(ConfigUrlService serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }
        ///<summary>
        ///Hace llamadas Post
        ///</summary>
        ///<param name="urlMethod">url del método a llamar, esta url se encdaenará con url configurada</param>
        ///<param name="item">objeto a pasar</param>
        ///<param name="token">Token del tipo Bearer para incluir seguridad si hiciese falta a la llamada de las apis</param>
        public string CallPostApi(string urlMethod, object item, TokenBearer token = null)
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
                client.Timeout = TimeSpan.FromDays(1);
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
    }
}
