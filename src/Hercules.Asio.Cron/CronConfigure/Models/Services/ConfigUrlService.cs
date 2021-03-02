// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Obtiene las configuraciones de las url base
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;
namespace CronConfigure.Models.Services
{
    /// <summary>
    /// Obtiene las configuraciones de las url base
    /// </summary>
    public class ConfigUrlService
    {
        private IConfiguration _configuration { get; set; }
        public string Url { get; set; }

        public ConfigUrlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        ///<summary>
        ///Método que obtiene la ConfigUrl configurada
        ///</summary>
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    connectionString = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrl"];
                }
                Url = connectionString;
            }
            return Url;
        }
    }
}
