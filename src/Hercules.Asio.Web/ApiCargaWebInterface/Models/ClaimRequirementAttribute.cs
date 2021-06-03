using ApiCargaWebInterface.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;
        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string urlLogin = context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetUrlSAMLLogin();

            if (!string.IsNullOrEmpty(urlLogin))
            {
                Claim claim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == _claim.Type);
                if (claim == null)
                {
                    string url = context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetUrlFront() + context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetProxy() +  context.HttpContext.Request.Path;
                    context.Result = new RedirectResult(urlLogin + "?returnUrl=" + url);
                }
                else if (claim.Value != _claim.Value)
                {
                    string url = context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetUrlFront() + context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetProxy() + "/access-denied";
                    context.Result = new RedirectResult(url);
                }
            }
        }
    }
}