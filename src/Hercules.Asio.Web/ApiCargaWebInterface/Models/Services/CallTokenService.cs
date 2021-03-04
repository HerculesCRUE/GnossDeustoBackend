// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// clase para la obtención de los tokens de acceso
using ApiCargaWebInterface.Models.Entities;
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
using System.Text;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// clase para la obtención de los tokens de acceso
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
                string stringData = $"grant_type=client_credentials&scope=apiCarga&client_id=Web&client_secret=master";
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
                string stringData = $"grant_type=client_credentials&scope=apiCron&client_id=Web&client_secret=master";
                return CallTokenIdentity(stringData);
            }
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api de uris
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenUrisFactory()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeUrisFactory", "AccessTokenUrisFactory");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiUrisFactory&client_id=Web&client_secret=master";
                return CallTokenIdentity(stringData);
            }
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api de uris
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenApiDocumentacion()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeDocumentacion", "AccessTokenDocumentacion");
            }
            else
            {
                {
                    string stringData = $"grant_type=client_credentials&scope=apiGestorDocumentacion&client_id=Web&client_secret=master";
                    return CallTokenIdentity(stringData);
                }
            }
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api OAIPMH
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenOAIPMH()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeOAIPMH", "AccessTokenOAIPMH");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiOAIPMH&client_id=Web&client_secret=master";
                return CallTokenIdentity(stringData);
            }
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Conversor.
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenConversor()
        {
            if (_env.IsDevelopment())
            {
                return TokenAppsettings("TokenTypeConversor", "AccessTokenConversor");
            }
            else
            {
                string stringData = $"grant_type=client_credentials&scope=apiConversor&client_id=Web&client_secret=master";
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
                //var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:Root12345678"));
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);
                client.Timeout = TimeSpan.FromDays(1);
                string authority = _configToken.GetAuthority()+ "/connect/token";
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
