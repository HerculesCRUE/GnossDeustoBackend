// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para obtener las variables de configuración de urls
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// Servicio para obtener las variables de configuración de urls
    /// </summary>
    public class ConfigUrlService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string UrlCarga { get; set; }
        public string UrlCron { get; set; }
        /// <summary>
        /// Obtiene la url del api de carga que ha sido configurada
        /// </summary>
        /// <returns>uri del api carga</returns>
        public string GetUrlCarga()
        {
            if (string.IsNullOrEmpty(UrlCarga))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    connectionString = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrl"];
                }

                UrlCarga = connectionString;
            }
            return UrlCarga;
        }

        /// <summary>
        /// Obtiene la url del api de carga que ha sido configurada
        /// </summary>
        /// <returns>uri del api cron</returns>
        public string GetUrlCron()
        {
            if (string.IsNullOrEmpty(UrlCron))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                string connectionString = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrlCron"))
                {
                    connectionString = environmentVariables["ConfigUrlCron"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrlCron"];
                }

                UrlCron = connectionString;
            }
            return UrlCron;
        }
    }
}
