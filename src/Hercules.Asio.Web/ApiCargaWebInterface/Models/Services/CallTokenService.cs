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
        public CallTokenService(ConfigTokenService configToken)
        {
            _configToken = configToken;
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api carga
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenCarga()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScope()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
            return CallTokenIdentity(stringData);
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api cron
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenCron()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeCron()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
            return CallTokenIdentity(stringData);
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api de uris
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenUrisFactory()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeUrisFactory()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
            return CallTokenIdentity(stringData);
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api de uris
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenApiDocumentacion()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeDocumentacion()}&client_id={_configToken.GetClientId()}&client_secret={_configToken.GetClientSecret()}";
            return CallTokenIdentity(stringData);
        }
        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api OAIPMH
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenOAIPMH()
        {
            string stringData = $"grant_type={_configToken.GetGrantType()}&scope={_configToken.GetScopeOAIPMH()}&client_id={_configToken.GetClientIdOAIPMH()}&client_secret={_configToken.GetClientSecretOAIPMH()}";
            return CallTokenIdentity(stringData);
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
