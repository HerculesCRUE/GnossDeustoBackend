using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Services
{
    /// <summary>
    /// Clase para la obtención de los datos necesarios para obtener el token de acceso al conversor.
    /// </summary> 
    public class ConfigTokenService
    {
        public IConfiguration _configuration { get; set; }
        private string Authority { get; set; }
        private string GrantType { get; set; }
        private string ScopeConversor { get; set; }
        private string ClientIdConversor { get; set; }
        private string ClientSecretConversor { get; set; }

        public ConfigTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Obtiene el endpoint para la llamada de obtención del token.
        /// </summary> 
        public string GetAuthorityGetToken()
        {
            if (string.IsNullOrEmpty(Authority))
            {
                string authority = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("AuthorityGetToken"))
                {
                    authority = environmentVariables["AuthorityGetToken"] as string;
                }
                else
                {
                    authority = _configuration["AuthorityGetToken"];
                }

                Authority = authority;
            }
            return Authority;
        }

        /// <summary>
        /// Obtiene el tipo de concesión de Oauth.
        /// </summary> 
        public string GetGrantType()
        {
            if (string.IsNullOrEmpty(GrantType))
            {
                string grantType = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("GrantType"))
                {
                    grantType = environmentVariables["GrantType"] as string;
                }
                else
                {
                    grantType = _configuration["GrantType"];
                }

                GrantType = grantType;
            }
            return GrantType;
        }

        /// <summary>
        /// Obtiene la limitación de acceso al conversor.
        /// </summary> 
        public string GetScopeConversor()
        {
            if (string.IsNullOrEmpty(ScopeConversor))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Scope"))
                {
                    scope = environmentVariables["Scope"] as string;
                }
                else
                {
                    scope = _configuration["Scope"];
                }

                ScopeConversor = scope;
            }
            return ScopeConversor;
        }

        /// <summary>
        /// Obtiene la "clave" de acceso del conversor.
        /// </summary>
        public string GetClientSecretConversor()
        {
            if (string.IsNullOrEmpty(ClientSecretConversor))
            {
                string clientSecret = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientSecretConversor"))
                {
                    clientSecret = environmentVariables["ClientSecretConversor"] as string;
                }
                else
                {
                    clientSecret = _configuration["ClientSecretConversor"];
                }

                ClientSecretConversor = clientSecret;
            }
            return ClientSecretConversor;
        }

        /// <summary>
        /// Obtiene la ID del cliente Conversor.
        /// </summary>
        public string GetClientIdConversor()
        {
            if (string.IsNullOrEmpty(ClientIdConversor))
            {
                string clientId = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientIdConversor"))
                {
                    clientId = environmentVariables["ClientIdConversor"] as string;
                }
                else
                {
                    clientId = _configuration["ClientIdConversor"];
                }

                ClientIdConversor = clientId;
            }
            return ClientIdConversor;
        }
    }
}
