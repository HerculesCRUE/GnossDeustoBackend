// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para la obtención de los tokens de acceso
using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    /// <summary>
    /// clase para la obtención de los tokens de acceso
    /// </summary>
    public class CallTokenService
    {
        private ConfigTokenService _configToken; 
        readonly IWebHostEnvironment _env;
        private IConfiguration _configuration { get; }

        public CallTokenService(ConfigTokenService configToken, IWebHostEnvironment env,IConfiguration configuration)
        {
            _configToken = configToken;
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene el token de acceso al api de carga
        /// </summary>
        public TokenBearer CallTokenCarga()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeCarga", "AccessTokenCarga");
            }
            else
            {
                string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeCarga()}&client_id={_configToken.GetClientIdCarga()}&client_secret={_configToken.GetClientSecretCarga()}";
                return CallTokenIdentity(stringData);
            }
        }

        /// <summary>
        /// Obtiene el token de acceso al api de OAIPMH
        /// </summary>
        public TokenBearer CallTokenOAIPMH()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeOAIPMH", "AccessTokenOAIPMH");
            }
            else
            {
                string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeOAIPMH()}&client_id={_configToken.GetClientIdOAIPMH()}&client_secret={_configToken.GetClientSecretOAIPMH()}";
                return CallTokenIdentity(stringData);
            }
        }

        /// <summary>
        /// Obtiene el token de acceso al api de OAIPMH
        /// </summary>
        public TokenBearer CallTokenConversor()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeConversor", "AccessTokenConversor");
            }
            else
            {
                string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeConversor()}&client_id={_configToken.GetClientIdConversor()}&client_secret={_configToken.GetClientSecretConversor()}";
                return CallTokenIdentity(stringData);
            }
        }

        /// <summary>
        /// Realiza la llamada para la obtención del token de acceso con el endpoint configurado en AuthorityGetToken
        /// </summary>
        /// <param name="stringData">Datos con el scope, el cliente id, el grantType y el secret</param>
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

        /// <summary>
        /// Obtiene los parametros desde el appsettings.
        /// </summary>
        /// <param name="pTypeToken">Tipo del token.</param>
        /// <param name="pAccessToken">Token.</param>
        /// <returns></returns>
        private TokenBearer TokenAppsettings(string pTypeToken, string pAccessToken)
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            TokenBearer token = new TokenBearer();

            if (environmentVariables.Contains(pAccessToken) && environmentVariables.Contains(pTypeToken))
            {
                token.access_token = environmentVariables[pAccessToken] as string;
                token.token_type = environmentVariables[pTypeToken] as string;
            }
            else
            {
                token.access_token = _configuration[pAccessToken];
                token.token_type = _configuration[pTypeToken];
            }

            return token;
        }
    }
}
