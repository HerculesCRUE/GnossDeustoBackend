using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OAI_PMH_CVN.Models.Services
{
    public class ConfigOAI_PMH_CVN
    {
        public IConfigurationRoot Configuration { get; set; }
        private string XML_CVN_Repository { get; set; }
        private string CVN_ROH_converter { get; set; }
        private string ConfigUrl { get; set; }
        public string GetXML_CVN_Repository()
        {
            if (string.IsNullOrEmpty(XML_CVN_Repository))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("XML_CVN_Repository"))
                {
                    XML_CVN_Repository = environmentVariables["XML_CVN_Repository"] as string;
                }
                else
                {
                    XML_CVN_Repository = Configuration["XML_CVN_Repository"];
                }
            }
            return XML_CVN_Repository;
        }

        public string GetCVN_ROH_converter()
        {
            if (string.IsNullOrEmpty(CVN_ROH_converter))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("CVN_ROH_converter"))
                {
                    CVN_ROH_converter = environmentVariables["CVN_ROH_converter"] as string;
                }
                else
                {
                    CVN_ROH_converter = Configuration["CVN_ROH_converter"];
                }
            }
            return CVN_ROH_converter;
        }

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
