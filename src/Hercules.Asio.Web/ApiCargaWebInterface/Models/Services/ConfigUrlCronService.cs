// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Servicio para obtener las variables de configuración respecto al cron
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Servicio para obtener las variables de configuración respecto al cron
    /// </summary>
    public class ConfigUrlCronService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// Obtiene la url del api de cron que ha sido configurada
        /// </summary>
        /// <returns>uri del api cron</returns>
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrlCron"))
                {
                    connectionString = environmentVariables["ConfigUrlCron"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrlCron"];
                }
                Url = connectionString;
            }
            return Url;
        }
    }
}
