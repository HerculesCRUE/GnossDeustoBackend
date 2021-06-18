using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Middlewares
{
    /// <summary>
    /// ErrorHandlingMiddleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;        
        private string _timeStamp;
        private string _LogPath;

        private IConfiguration _configuration { get; set; }

        /// <summary>
        /// ErrorHandlingMiddleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (string.IsNullOrEmpty(_timeStamp) || !_timeStamp.Equals(CreateTimeStamp()))
            {
                _timeStamp = CreateTimeStamp();
                CreateLoggin(_timeStamp);
            }

            var code = HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(new { error = "Internal server error" });
            if (code != HttpStatusCode.InternalServerError)
            {
                result = JsonConvert.SerializeObject(new { error = ex.Message });
            }
            else
            {
                Log.Error($"{ex.Message}\n{ex.StackTrace}\n");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }

        private void CreateLoggin(string pTimestamp)
        {
            string pathDirectory = GetLogPath();
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File($"{pathDirectory}/log_{pTimestamp}.txt").CreateLogger();
        }

        private string CreateTimeStamp()
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

        /// <summary>
        /// GetLogPath
        /// </summary>
        /// <returns></returns>
        public string GetLogPath()
        {
            if (string.IsNullOrEmpty(_LogPath))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string logPath = "";
                if (environmentVariables.Contains("LogPath"))
                {
                    logPath = environmentVariables["LogPath"] as string;
                }
                else
                {
                    logPath = _configuration["LogPath"];
                }
                _LogPath = logPath;
            }
            return _LogPath;
        }
    }
}
