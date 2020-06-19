// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para la obtención de los tokens de acceso
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
        private ConfigTokenService _configToken;
        public CallTokenService(ConfigTokenService configToken)
        {
            _configToken = configToken;
        }
        public TokenBearer CallTokenCarga()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScope()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
            return CallTokenIdentity(stringData);
        }
        public TokenBearer CallTokenCron()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeCron()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
            return CallTokenIdentity(stringData);
        }

        public TokenBearer CallTokenUrisFactory()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeUrisFactory()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
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
                string authority = _configToken.GetAuthority();
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
