using ApiCargaWebInterface.Models;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        public async Task InvokeAsync(HttpContext httpContext, TokenSAMLBDService tokenSAMLBDService)
        {
            // Obtener el estado de SAML.
            string smlstatus = httpContext.Session.GetString("session_saml");

            // Si no existe, se mira en la cookie.
            if (string.IsNullOrEmpty(smlstatus))
            {
                if (httpContext.Request.Cookies.ContainsKey("cookie_saml"))
                {
                    string cookieValue = httpContext.Request.Cookies["cookie_saml"];
                    if (cookieValue.Contains("_"))
                    {
                        string guidString = cookieValue.Substring(0, cookieValue.IndexOf("_"));
                        Guid guidToken;
                        if (Guid.TryParse(guidString, out guidToken))
                        {
                            TokenSAML tokenSAMLBBDD = tokenSAMLBDService.GetTokenSAML(guidToken);
                            if (tokenSAMLBBDD != null)
                            {
                                // Si existe en la cookie Y el token esta en la BBDD, se guarda en la sesión.
                                httpContext.Session.SetString("session_saml", httpContext.Request.Cookies["cookie_saml"]);
                                smlstatus = httpContext.Session.GetString("session_saml");
                                tokenSAMLBDService.RemoveTokenSAML(tokenSAMLBBDD);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(smlstatus))
            {
                string status = smlstatus.Substring(smlstatus.IndexOf("_") + 1);
                httpContext.User.Identities.FirstOrDefault().AddClaim(new Claim("Administrator", status));
            }

            await _next(httpContext);
        }
    }
}
