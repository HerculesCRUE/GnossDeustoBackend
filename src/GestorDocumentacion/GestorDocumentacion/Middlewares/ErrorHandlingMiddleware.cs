// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que actua de Middleware para la gestión de las excepciones
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GestorDocumentacion.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfigurationRoot Configuration { get; set; }
        private string _timeStamp;
        private string _LogPath;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;

        }

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

            //if (ex is ParametersNotConfiguredException)
            //{
            //    code = HttpStatusCode.BadRequest;
            //    Log.Information($"{ex.Message}\n");
            //}
            //else if (ex is FailedLoadConfigJsonException)
            //{
            //    code = HttpStatusCode.InternalServerError;
            //    Log.Information($"{ex.Message}\n");
            //}

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
    }
}
