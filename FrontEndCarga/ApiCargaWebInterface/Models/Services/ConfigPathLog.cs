using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class ConfigPathLog
    {
        private IConfigurationRoot Configuration { get; set; }
        private string _LogPath;
        private string _LogPathCarga;
        private string _LogPathCron;
        public string GetLogPath()
        {
            if (string.IsNullOrEmpty(_LogPath))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPath"))
                {
                    logPath = environmentVariables["LogPath"] as string;
                }
                else
                {
                    logPath = Configuration["LogPath"];
                }
                _LogPath = logPath;
            }
            return _LogPath;
        }

        public string GetLogPathCarga()
        {
            if (string.IsNullOrEmpty(_LogPathCarga))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPathCarga"))
                {
                    logPath = environmentVariables["LogPathCarga"] as string;
                }
                else
                {
                    logPath = Configuration["LogPathCarga"];
                }
                _LogPathCarga = logPath;
            }
            return _LogPathCarga;
        }

        public string GetLogPathCron()
        {
            if (string.IsNullOrEmpty(_LogPathCron))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPathCron"))
                {
                    logPath = environmentVariables["LogPathCron"] as string;
                }
                else
                {
                    logPath = Configuration["LogPathCron"];
                }
                _LogPathCron = logPath;
            }
            return _LogPathCron;
        }
    }
}
