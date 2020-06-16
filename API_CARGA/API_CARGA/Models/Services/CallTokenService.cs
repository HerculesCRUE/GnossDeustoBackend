using API_CARGA.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class CallTokenService
    {
        private ConfigTokenService _configToken;
        public CallTokenService(ConfigTokenService configToken)
        {
            _configToken = configToken;
        }
        public TokenBearer CallTokenCarga()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeCarga()}&client_id={_configToken.GetClientIdCarga()}&client_secret={_configToken.GetClientSecretCarga()}";
            return CallTokenIdentity(stringData);
        }

        public TokenBearer CallTokenOAIPMH()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeOAIPMH()}&client_id={_configToken.GetClientIdOAIPMH()}&client_secret={_configToken.GetClientSecretOAIPMH()}";
            return CallTokenIdentity(stringData);
        }

        private TokenBearer CallTokenIdentity(string stringData)
        {
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromDays(1);
                string authority = _configToken.GetAuthorityGetToken();
                response = client.PostAsync($"{authority}", contentData).Result;
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
