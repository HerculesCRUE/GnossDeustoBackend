using ApiCargaWebInterface.Extra.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private string _timeStamp;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;

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
            context.Request.Path = $"/error/{code.GetHashCode()}";
        }

        private void CreateLoggin(string pTimestamp)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File($"logs/log_{pTimestamp}.txt").CreateLogger();
        }

        private string CreateTimeStamp()
        {
            DateTime time = DateTime.Now;
            string timeStamp = $"{time.Year.ToString()}{time.Month.ToString()}{time.Day.ToString()}";
            return timeStamp;
        }
    }
}
