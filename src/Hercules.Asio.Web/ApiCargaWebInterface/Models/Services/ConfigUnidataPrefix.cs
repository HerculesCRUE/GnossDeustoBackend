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
    /// Servicio para obtener el prefijo de Unidata
    /// </summary>
    public class ConfigUnidataPrefix
    {
        private string UnidataDomain { get; set; }
        private IConfiguration _configuration { get; set; }

        public ConfigUnidataPrefix(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene la url del dominio de Unidata
        /// </summary>
        /// <returns>uri del dominio de Unidata</returns>
        public string GetUnidataDomain()
        {
            if (string.IsNullOrEmpty(UnidataDomain))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("UnidataDomain"))
                {
                    connectionString = environmentVariables["UnidataDomain"] as string;
                }
                else
                {
                    connectionString = _configuration["UnidataDomain"];
                }
                UnidataDomain = connectionString;
            }
            return UnidataDomain;
        }
    }
}
