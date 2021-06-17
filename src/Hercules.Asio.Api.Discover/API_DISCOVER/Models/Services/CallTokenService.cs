// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para la obtención de los tokens de acceso
using API_DISCOVER.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// clase para la obtención de los tokens de acceso
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CallTokenService
    {
        readonly private ConfigTokenService _configToken;
        readonly IHostEnvironment _env;

        /// <summary>
        /// CallTokenService
        /// </summary>
        /// <param name="configToken"></param>
        /// <param name="env"></param>
        public CallTokenService(ConfigTokenService configToken, IHostEnvironment env)
        {
            _configToken = configToken;
            _env = env;
        }

        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api carga
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenCarga()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeCarga", "AccessTokenCarga");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiCarga&client_id=Discover&client_secret=secretDiscover";
                return CallTokenIdentity(stringData);
            }
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api cron
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenCron()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeCron", "AccessTokenCron");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiCron&client_id=Discover&client_secret=secretDiscover";
                return CallTokenIdentity(stringData);
            }
        }

        /// <summary>
        /// Obtiene el token de acceso al api de OAIPMH
        /// </summary>
        public TokenBearer CallTokenUrisFactory()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeUrisFactory", "AccessTokenUrisFactory");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiUrisFactory&client_id=Discover&client_secret=secretDiscover";
                return CallTokenIdentity(stringData);
            }
        }


        /// <summary>
        /// Llama al api de gestión de tokens
        /// </summary>
        /// <param name="stringData">cadena con la información de configuración de los tokens de un api;"grant_type={grantType}&scope={scope del api}&client_id={ClienteId del Api}&client_secret={contraseña del api}"</param>
        /// <returns>token bearer</returns>
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
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                IConfigurationRoot Configuration = builder.Build();
                token.access_token = Configuration[pAccessToken];
                token.token_type = Configuration[pTypeToken];
            }

            return token;
        }
    }
}
