// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Obtiene los párametros configurados
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace OAI_PMH_CVN.Models.Services
{
    public class ConfigOAI_PMH_CVN
    {
        public IConfiguration _configuration { get; set; }
        private string XML_CVN_Repository { get; set; }
        private string CVN_ROH_converter { get; set; }
        private string ConfigUrl { get; set; }

        public ConfigOAI_PMH_CVN(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// GetBuildConfiguration
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetBuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json");

            return builder.Build();
        }
        public string GetXML_CVN_Repository()
        {
            if (string.IsNullOrEmpty(XML_CVN_Repository))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("XML_CVN_Repository"))
                {
                    XML_CVN_Repository = environmentVariables["XML_CVN_Repository"] as string;
                }
                else
                {
                    XML_CVN_Repository = _configuration["XML_CVN_Repository"];
                }
            }
            return XML_CVN_Repository;
        }

        public string GetCVN_ROH_converter()
        {
            if (string.IsNullOrEmpty(CVN_ROH_converter))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("CVN_ROH_converter"))
                {
                    CVN_ROH_converter = environmentVariables["CVN_ROH_converter"] as string;
                }
                else
                {
                    CVN_ROH_converter = _configuration["CVN_ROH_converter"];
                }
            }
            return CVN_ROH_converter;
        }

        public string GetConfigUrl()
        {
            if (string.IsNullOrEmpty(ConfigUrl))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ConfigUrl"))
                {
                    ConfigUrl = environmentVariables["ConfigUrl"] as string;
                }
                else
                {
                    ConfigUrl = _configuration["ConfigUrl"];
                }
            }
            return ConfigUrl;
        }
    }
}
