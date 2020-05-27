using ApiCargaWebInterface.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class CallTokenService
    {
        public TokenBearer CallTokenIdentity(string urlMethod, string grantType, string scope, string clientId, string clientSecret)
        {
            /*string stringData = "grant_type=client_credentials&scope=api1&client_id=client&client_secret=secret";*///JsonConvert.SerializeObject(item);
            string stringData = $"grant_type={grantType}& scope={scope}&client_id={clientId}&client_secret={clientSecret}";
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromDays(1);
                response = client.PostAsync($"{urlMethod}", contentData).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                TokenBearer token = JsonConvert.DeserializeObject<TokenBearer>(result);
                return token;
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
        }
    }
}
