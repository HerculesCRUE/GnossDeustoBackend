// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la obtención de los datos necesarios para obtener el token de acceso al apiCarga
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    /// <summary>
    /// Clase para la obtención de los datos necesarios para obtener el token de acceso al apiCarga
    /// </summary> 
    public class ConfigTokenService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Authority { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string ScopeCron { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ConfigTokenService()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }
        /// <summary>
        /// obtiene el endpoint para la llamada de obtención del token
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
                    authority = Configuration["AuthorityGetToken"];
                }

                Authority = authority;
            }
            return Authority;
        }
        /// <summary>
        /// obtiene el tipo de concesión de Oauth
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
                    grantType = Configuration["GrantType"];
                }

                GrantType = grantType;
            }
            return GrantType;
        }

        /// <summary>
        /// obtiene la limitación de acceso al api de carga
        /// </summary> 
        public string GetScopeCarga()
        {
            if (string.IsNullOrEmpty(Scope))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeCarga"))
                {
                    scope = environmentVariables["ScopeCarga"] as string;
                }
                else
                {
                    scope = Configuration["ScopeCarga"];
                }

                Scope = scope;
            }
            return Scope;
        }

        /// <summary>
        /// obtiene el id de cliente del api de carga
        /// </summary> 
        public string GetClientId()
        {
            if (string.IsNullOrEmpty(ClientId))
            {
                string clientId = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientId"))
                {
                    clientId = environmentVariables["ClientId"] as string;
                }
                else
                {
                    clientId = Configuration["ClientId"];
                }

                ClientId = clientId;
            }
            return ClientId;
        }

        /// <summary>
        /// obtiene la "clave" de acceso del api de carga
        /// </summary>
        public string GetClientSecret()
        {
            if (string.IsNullOrEmpty(ClientSecret))
            {
                string clientSecret = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientSecret"))
                {
                    clientSecret = environmentVariables["ClientSecret"] as string;
                }
                else
                {
                    clientSecret = Configuration["ClientSecret"];
                }

                ClientSecret = clientSecret;
            }
            return ClientSecret;
        }
    }
}
