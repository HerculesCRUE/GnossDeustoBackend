// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que actua de Middleware para la gestión de las excepciones
using ApiCargaWebInterface.Extra.Exceptions;
using ApiCargaWebInterface.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Middlewares
{
    /// <summary>
    /// Clase que actua de Middleware para la gestión de las excepciones
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConfigPathLog _configPathLog;
        private IConfigurationRoot Configuration { get; set; }
        private string _timeStamp;
        private string _LogPath;
        public ErrorHandlingMiddleware(RequestDelegate next, ConfigPathLog configPathLog)
        {
            _next = next;
            _configPathLog = configPathLog;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex);
                await _next(context);
            }
        }

        private void HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (string.IsNullOrEmpty(_timeStamp) || !_timeStamp.Equals(CreateTimeStamp()))
            {
                _timeStamp = CreateTimeStamp();
                CreateLoggin(_timeStamp);
            }

            var code = HttpStatusCode.InternalServerError;
            if (ex is BadRequestException)
            {
                code = HttpStatusCode.BadRequest;
                Log.Information($"{ex.Message}\n");
            }

            if (code != HttpStatusCode.InternalServerError)
            {
                Log.Information($"{ex.Message}\n");
            }
            else
            {
                Log.Error($"{ex.Message}\n{ex.StackTrace}\n");
            }

            context.Response.StatusCode = (int)code;
            if ((int)code == 500)
            {
                context.Request.Path = $"/error/exception";
            }
            else
            {
                context.Request.Path = $"/error/{code.GetHashCode()}";
            }
            
        }
        /// <summary>
        /// Creación del log para la escritura del error
        /// </summary>
        /// <param name="pTimestamp"></param>
        private void CreateLoggin(string pTimestamp)
        {
            string pathDirectory = GetLogPath();
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File($"{pathDirectory}/log_{pTimestamp}.txt").CreateLogger();
        }
        /// <summary>
        /// Devuleve una cadena con el día/mes/año
        /// </summary>
        /// <returns>cadena con el día/mes/año</returns>
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
        /// Obtiene el path configurado donde guardar el error
        /// </summary>
        /// <returns></returns>
        public string GetLogPath()
        {
            return $"{_configPathLog.GetLogPathBase()}{_configPathLog.GetLogPath()}";
        }
    }
}
