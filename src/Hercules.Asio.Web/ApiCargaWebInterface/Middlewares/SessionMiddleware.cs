using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hercules.Asio.Web.Middlewares
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Obtener el estado de SAML.
            string smlstatus = httpContext.Session.GetString("session_saml");

            // Si no existe, se mira en la cookie.
            if (string.IsNullOrEmpty(smlstatus))
            {
                if (httpContext.Request.Cookies.ContainsKey("cookie_saml"))
                {
                    // Si existe en la cookie, se guarda en la sesión.
                    httpContext.Session.SetString("session_saml", httpContext.Request.Cookies["cookie_saml"]);
                    smlstatus = httpContext.Session.GetString("session_saml");
                }
            }

            if (!string.IsNullOrEmpty(smlstatus))
            {
                string guid = smlstatus.Substring(0, smlstatus.IndexOf("_"));
                string status = smlstatus.Substring(smlstatus.IndexOf("_") + 1);
                //cargar pagina con normalidad o mostrar pagina sin acceso
                httpContext.User.Identities.FirstOrDefault().AddClaim(new Claim("Administrator", status));
            }

            await _next(httpContext);
        }
    }
}
