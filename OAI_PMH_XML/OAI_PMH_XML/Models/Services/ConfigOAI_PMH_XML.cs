// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Obtiene los párametros configurados
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace OAI_PMH_XML.Models.Services
{
    public class ConfigOAI_PMH_XML
    {
        public IConfigurationRoot Configuration { get; set; }
        private string ConfigUrl { get; set; }        

        public string GetConfigUrl()
        {
            if (string.IsNullOrEmpty(ConfigUrl))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    ConfigUrl = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    ConfigUrl = Configuration["ConfigUrl"];
                }
            }
            return ConfigUrl;
        }
    }
}
