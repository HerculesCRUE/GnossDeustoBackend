using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace API_DISCOVER.Models.Log
{
    public static class Log
    {
        private static IConfigurationRoot Configuration { get; set; }
        private static string _timeStamp;
        private static string _LogPath;

        public static void Error(Exception pException,string pExtraInfo=null)
        {
            if (string.IsNullOrEmpty(_timeStamp) || !_timeStamp.Equals(CreateTimeStamp()))
            {
                _timeStamp = CreateTimeStamp();
                CreateLoggin(_timeStamp);
            }
            if (pExtraInfo == null)
            {
                Serilog.Log.Error($"{pException.Message}\n{pException.StackTrace}\n");
            }else
            {
                Serilog.Log.Error($"{pException.Message}\n{pException.StackTrace}\n{pExtraInfo}\n");
            }
        }

        private static void CreateLoggin(string pTimestamp)
        {
            string pathDirectory = GetLogPath();
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            Serilog.Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File($"{pathDirectory}/log_{pTimestamp}.txt").CreateLogger();
        }

        private static string CreateTimeStamp()
        {
            DateTime time = DateTime.Now;
            string month = time.Month.ToString();
            if (month.Length == 1)
            {
                month = $"0{month}";
            }
            string day = time.Day.ToString();
            if (day.Length == 1)
            {
                day = $"0{day}";
            }
            string timeStamp = $"{time.Year.ToString()}{month}{day}";
            return timeStamp;
        }

        public static string GetLogPath()
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
    }
}
