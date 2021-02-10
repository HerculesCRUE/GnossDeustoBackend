// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada para obtener las urlsConfiguradas
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase usada para obtener las urlsConfiguradas
    ///</summary>
    public class ConfigUrlService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Url { get; set; }
        private string UrlUnidata { get; set; }
        private string ConfigUrlXmlConverter { get; set; }

        ///<summary>
        ///Obtiene la url configurada en ConfigUrl del fichero appsettings.json
        ///</summary>
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
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    connectionString = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrl"];
                }
                Url = connectionString;
            }
            return Url;
        }
        ///<summary>
        ///Obtiene la url configurada en ConfigUrlUnidata del fichero appsettings.json
        ///</summary>
        public string GetUrlUnidata()
        {
            if (string.IsNullOrEmpty(UrlUnidata))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrlUnidata"))
                {
                    connectionString = environmentVariables["ConfigUrlUnidata"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrlUnidata"];
                }
                UrlUnidata = connectionString;
            }
            return UrlUnidata;
        }
        ///<summary>
        ///Obtiene la url configurada en ConfigUrlXmlConverter del fichero appsettings.json
        ///</summary>
        public string GetUrlXmlConverter()
        {
            if (string.IsNullOrEmpty(ConfigUrlXmlConverter))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string connectionString = "";
                if (environmentVariables.Contains("ConfigUrlXmlConverter"))
                {
                    connectionString = environmentVariables["ConfigUrlXmlConverter"] as string;
                }
                else
                {
                    connectionString = Configuration["ConfigUrlXmlConverter"];
                }
                ConfigUrlXmlConverter = connectionString;
            }
            return ConfigUrlXmlConverter;
        }
    }
}
