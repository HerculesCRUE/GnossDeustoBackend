using ApiCargaWebInterface.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.Web.ActionFilter
{
    public class ViewBagActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string urlLogin = context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetUrlSAMLLogin();
            if (!string.IsNullOrEmpty(urlLogin))
            {
                // for Razor Views
                if (context.Controller is Controller)
                {
                    var controller = context.Controller as Controller;
                    controller.ViewBag.SAMLActive = true;
                }
            }
            base.OnResultExecuting(context);
        }
    }
}
