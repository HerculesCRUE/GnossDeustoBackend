// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada para obtener las urlsConfiguradas
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace API_CARGA.Models.Services
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase usada para obtener las urlsConfiguradas
    ///</summary>
    public class ConfigUrlService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Url { get; set; }
        private string ConfigUrlXmlConverter { get; set; }
        private IConfiguration _configuration { get; }

        public ConfigUrlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        ///<summary>
        ///Obtiene la url configurada en ConfigUrl del fichero appsettings.json
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
        ///<summary>
        ///Obtiene la url configurada en ConfigUrlXmlConverter del fichero appsettings.json
        ///</summary>
        public string GetUrlXmlConverter()
        {
            if (string.IsNullOrEmpty(ConfigUrlXmlConverter))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrlXmlConverter"))
                {
                    connectionString = environmentVariables["ConfigUrlXmlConverter"] as string;
                }
                else
                {
                    connectionString = _configuration["ConfigUrlXmlConverter"];
                }
                ConfigUrlXmlConverter = connectionString;
            }
            return ConfigUrlXmlConverter;
        }
    }
}
