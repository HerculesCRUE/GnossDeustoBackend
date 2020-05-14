using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace ApiCargaWebInterface.Models.Services
{
    public class ConfigUrlCronService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Url { get; set; }
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
