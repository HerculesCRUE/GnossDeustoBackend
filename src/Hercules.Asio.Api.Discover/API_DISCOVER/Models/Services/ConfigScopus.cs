// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener la configuración necesaria para el uso de Sparql
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    ///<summary>
    ///Clase para obtener la configuración necesaria para el uso del API de SCOPUS
    ///</summary>
    public class ConfigScopus
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        private string ScopusApiKey { get; set; }
        private string ScopusUrl { get; set; }
        
        ///<summary>
        ///Obtiene el API key de Scopus
        ///</summary>
        public string GetScopusApiKey()
        {
            if (string.IsNullOrEmpty(ScopusApiKey))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopusApiKey"))
                {
                    ScopusApiKey = environmentVariables["ScopusApiKey"] as string;
                }
                else
                {
                    ScopusApiKey = Configuration["ScopusApiKey"];
                }                                                
            }
            return ScopusApiKey;
        }

        ///<summary>
        ///Obtiene el url del API de Scopus
        ///</summary>
        public string GetScopusUrl()
        {
            if (string.IsNullOrEmpty(ScopusUrl))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopusUrl"))
                {
                    ScopusUrl = environmentVariables["ScopusUrl"] as string;
                }
                else
                {
                    ScopusUrl = Configuration["ScopusUrl"];
                }
            }
            return ScopusUrl;
        }
    }
}
