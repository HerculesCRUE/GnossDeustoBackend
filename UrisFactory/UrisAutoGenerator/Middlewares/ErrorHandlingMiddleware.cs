using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;
using UrisFactory.Extra.Exceptions;

namespace UrisFactory.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private string _timeStamp;
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

            if (ex is ParametersNotConfiguredException) 
            {
                code = HttpStatusCode.BadRequest;
                Log.Information($"{ex.Message}\n");
            }
            else if (ex is FailedLoadConfigJsonException)
            {
                code = HttpStatusCode.InternalServerError;
                Log.Information($"{ex.Message}\n");
            }

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
