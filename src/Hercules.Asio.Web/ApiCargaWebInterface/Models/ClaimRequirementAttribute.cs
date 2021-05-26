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

            Claim claim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == _claim.Type);
            if (claim==null)
            {                
                context.Result = new RedirectResult(context.HttpContext.RequestServices.GetRequiredService<ConfigUrlService>().GetUrlSAMLLogin());
            }
            else if(claim.Value!= _claim.Value)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}