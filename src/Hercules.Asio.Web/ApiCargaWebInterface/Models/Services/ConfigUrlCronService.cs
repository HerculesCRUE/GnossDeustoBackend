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
        public string Url { get; set; }
        public string UrlSwagger { get; set; }

        private IConfiguration _configuration { get; set; }

        public ConfigUrlCronService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Obtiene la url del api de cron que ha sido configurada
        /// </summary>
        /// <returns>uri del api cron</returns>
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrlCron"))
                {
                    connectionString = environmentVariables["ConfigUrlCron"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrlCron"];
                }
                Url = connectionString;
            }
            return Url;
        }
        /// <summary>
        /// Obtiene la url de swagger del api de cron que ha sido configurada
        /// </summary>
        /// <returns>uri del api cron</returns>
        public string GetUrlSwagger()
        {
            if (string.IsNullOrEmpty(UrlSwagger))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrlCronSwagger"))
                {
                    connectionString = environmentVariables["ConfigUrlCronSwagger"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrlCronSwagger"];
                }
                UrlSwagger = connectionString;
            }
            return UrlSwagger;
        }

    }
}
