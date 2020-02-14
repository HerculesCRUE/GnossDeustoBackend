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
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File("logs/log.txt").CreateLogger();
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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;

            if (ex is ParametersNotConfiguredException) 
            {
                code = HttpStatusCode.BadRequest;
            }
            else if (ex is FailedLoadConfigJsonException)
            {
                code = HttpStatusCode.InternalServerError;
            }

            var result = JsonConvert.SerializeObject(new { error = "Internal server error" });
            if (code != HttpStatusCode.InternalServerError)
            {
                result = JsonConvert.SerializeObject(new { error = ex.Message });
            }
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            Log.Error(ex.Message);
            return context.Response.WriteAsync(result);
        }
    }
}
