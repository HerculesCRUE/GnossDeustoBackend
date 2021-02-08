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
        public IConfigurationRoot Configuration { get; set; }
        private string UnidataDomain { get; set; }
        
        /// <summary>
        /// Obtiene la url del dominio de Unidata
        /// </summary>
        /// <returns>uri del dominio de Unidata</returns>
        public string GetUnidataDomain()
        {
            if (string.IsNullOrEmpty(UnidataDomain))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("UnidataDomain"))
                {
                    connectionString = environmentVariables["UnidataDomain"] as string;
                }
                else
                {
                    connectionString = Configuration["UnidataDomain"];
                }
                UnidataDomain = connectionString;
            }
            return UnidataDomain;
        }
    }
}
