// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la obtención de los datos necesarios para obtener los tokens de acceso 
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// Clase para la obtención de los datos necesarios para obtener los tokens de acceso 
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
        /// obtiene el authority configurado
        /// </summary>
        /// <returns>authority</returns>
        public string GetAuthority()
        {
            if (string.IsNullOrEmpty(Authority))
            {
                string authority = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Authority"))
                {
                    authority = environmentVariables["Authority"] as string;
                }
                else
                {
                    authority = Configuration["Authority"];
                }

                Authority = authority;
            }
            return Authority;
        }

        /// <summary>
        /// Obtiene el grant type configurado
        /// </summary>
        /// <returns>grant type</returns>
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
        /// Obtiene el scope de api carga configurado
        /// </summary>
        /// <returns>scope api carga</returns>
        public string GetScope()
        {
            if (string.IsNullOrEmpty(Scope))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Scope"))
                {
                    scope = environmentVariables["Scope"] as string;
                }
                else
                {
                    scope = Configuration["Scope"];
                }

                Scope = scope;
            }
            return Scope;
        }
        /// <summary>
        /// Obtiene el scope del cron configurado
        /// </summary>
        /// <returns>scope del cron</returns>
        public string GetScopeCron()
        {
            if (string.IsNullOrEmpty(ScopeCron))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeCron"))
                {
                    scope = environmentVariables["ScopeCron"] as string;
                }
                else
                {
                    scope = Configuration["ScopeCron"];
                }

                ScopeCron = scope;
            }
            return ScopeCron;
        }

        /// <summary>
        /// Obtiene el cliente id configurado
        /// </summary>
        /// <returns>cliente id</returns>
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
        /// Obtiene el client secret configurado
        /// </summary>
        /// <returns>client secret</returns>
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
