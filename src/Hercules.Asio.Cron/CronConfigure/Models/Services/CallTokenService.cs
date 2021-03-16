// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para la obtención de los tokens de acceso
using CronConfigure.Models.Entitties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    /// <summary>
    /// Clase para la obtención de los tokens de acceso
    /// </summary>
    public class CallTokenService
    {
        private ConfigTokenService _configToken; 
        readonly IWebHostEnvironment _env;
        private IConfiguration _configuration { get; set; }

        public CallTokenService(ConfigTokenService configToken, IWebHostEnvironment env, IConfiguration configuration)
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
                Log.Information("Ha entrado por development");
                return TokenAppsettings("TokenTypeCarga", "AccessTokenCarga");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiCarga&client_id=cron&client_secret=secretCron";
                return CallTokenIdentity(stringData);
            }
        }

        /// <summary>
        /// Realiza la llamada para la obtención del token de acceso con el endpoint configurado en AuthorityGetToken
        /// </summary>
        /// <param name="stringData">Datos con el scope, el cliente id, el grantType y el secret</param>
        private TokenBearer CallTokenIdentity(string stringData)
        {
            Log.Information($"CallTokenIdentity string data: {stringData}");
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromDays(1);
                string authority = _configToken.GetAuthority()+"/connect/token";
                Log.Information($"CallTokenIdentity authority {authority}");
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
