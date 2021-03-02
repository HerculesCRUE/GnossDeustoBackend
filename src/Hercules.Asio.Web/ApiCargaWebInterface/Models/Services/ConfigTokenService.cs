﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
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

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Clase para la obtención de los datos necesarios para obtener los tokens de acceso 
    /// </summary>
    public class ConfigTokenService
    {
        public string Authority { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string ScopeCron { get; set; }
        public string ScopeUrisFactory { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ScopeOAIPMH { get; set; }
        public string ScopeDocumentacion { get; set; }
        public string ClientIdOAIPMH { get; set; }
        public string ClientSecretOAIPMH { get; set; }
        private string ScopeConversor { get; set; }
        private string ClientIdConversor { get; set; }
        private string ClientSecretConversor { get; set; }
        private IConfiguration _configuration { get; set; }
        public ConfigTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
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
                    authority = _configuration["Authority"];
                }

                Authority = authority;
            }
            return Authority;
        }
        /// <summary>
        /// Obtiene el client secret configurado
        /// </summary>
        /// <returns>client secretreturns>
        internal object GetClientSecretOAIPMH()
        {
            if (string.IsNullOrEmpty(ClientSecretOAIPMH))
            {
                string clientSecretOAIPMH = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientSecretOAIPMH"))
                {
                    clientSecretOAIPMH = environmentVariables["ClientSecretOAIPMH"] as string;
                }
                else
                {
                    clientSecretOAIPMH = _configuration["ClientSecretOAIPMH"];
                }

                ClientSecretOAIPMH = clientSecretOAIPMH;
            }
            return ClientSecretOAIPMH;
        }
        /// <summary>
        /// Obtiene el Scope del api de documentacion configurado
        /// </summary>
        /// <returns>Scope del OAIPMH</returns>
        internal object GetScopeDocumentacion()
        {
            if (string.IsNullOrEmpty(ScopeDocumentacion))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeDocumentacion"))
                {
                    ScopeDocumentacion = environmentVariables["ScopeDocumentacion"] as string;
                }
                else
                {
                    ScopeDocumentacion = _configuration["ScopeDocumentacion"];
                }
            }
            return ScopeDocumentacion;
        }

        /// <summary>
        /// Obtiene el Scope del OAIPMH configurado
        /// </summary>
        /// <returns>Scope del OAIPMH</returns>
        internal string GetScopeOAIPMH()
        {
            if (string.IsNullOrEmpty(ScopeOAIPMH))
            {
                string scopeOAIPMH = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeOAIPMH"))
                {
                    scopeOAIPMH = environmentVariables["ScopeOAIPMH"] as string;
                }
                else
                {
                    scopeOAIPMH = _configuration["ScopeOAIPMH"];
                }

                ScopeOAIPMH = scopeOAIPMH;
            }
            return ScopeOAIPMH;
        }
        /// <summary>
        /// Obtien el cliente id del OAIPMH configurado
        /// </summary>
        /// <returns>cliente id del OAIPMH</returns>
        public string GetClientIdOAIPMH()
        {
            if (string.IsNullOrEmpty(ClientIdOAIPMH))
            {
                string ScopeOAIPMH = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientIdOAIPMH"))
                {
                    ScopeOAIPMH = environmentVariables["ClientIdOAIPMH"] as string;
                }
                else
                {
                    ScopeOAIPMH = _configuration["ClientIdOAIPMH"];
                }

                ClientIdOAIPMH = ScopeOAIPMH;
            }
            return ClientIdOAIPMH;
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
                    grantType = _configuration["GrantType"];
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
                    scope = _configuration["Scope"];
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
                    scope = _configuration["ScopeCron"];
                }

                ScopeCron = scope;
            }
            return ScopeCron;
        }
        /// <summary>
        /// Obtiene el scope del urisFactory configurado
        /// </summary>
        /// <returns>scope de urisFactory</returns>
        public string GetScopeUrisFactory()
        {
            if (string.IsNullOrEmpty(ScopeUrisFactory))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeUrisFactory"))
                {
                    scope = environmentVariables["ScopeUrisFactory"] as string;
                }
                else
                {
                    scope = _configuration["ScopeUrisFactory"];
                }

                ScopeUrisFactory = scope;
            }
            return ScopeUrisFactory;
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
                    clientId = _configuration["ClientId"];
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
                    clientSecret = _configuration["ClientSecret"];
                }

                ClientSecret = clientSecret;
            }
            return ClientSecret;
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
                if (environmentVariables.Contains("ScopeConversor"))
                {
                    scope = environmentVariables["ScopeConversor"] as string;
                }
                else
                {
                    scope = _configuration["ScopeConversor"];
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
