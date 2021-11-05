using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.MvcCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System;

namespace Hercules_SAML
{
    [AllowAnonymous]
    [Route("Auth")]
    public class AuthController : Controller
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private Saml2Configuration config;

        public AuthController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        [Route("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/") } });

            return binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();
        }

        [Route("AssertionConsumerService")]
        public async Task<IActionResult> AssertionConsumerService()
        {
            try
            {
                config.AuthnResponseSignType = Saml2AuthnResponseSignTypes.SignAssertion;
                //config.SignAuthnRequest = false;

                var binding = new Saml2PostBinding();
                var saml2AuthnResponse = new Saml2AuthnResponse(config);

                binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);
                if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
                {
                    throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
                }
                binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
                await saml2AuthnResponse.CreateSession(HttpContext, lifetime: new TimeSpan(0, 0, 5), claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));

                var relayStateQuery = binding.GetRelayStateQuery();
                var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");
                return Redirect(returnUrl);
            }
            catch (Exception ex) {
                return Ok(ex.Message+" "+ex.StackTrace);
            
            }
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            //return Redirect(config.SingleLogoutDestination.Scheme + "://" + config.SingleLogoutDestination.Host + "/cas/logout?service="+ config.Issuer);
            var binding = new Saml2PostBinding();            
            var saml2LogoutRequest = await new Saml2LogoutRequest(config, User).DeleteSession(HttpContext);
            return binding.Bind(saml2LogoutRequest).ToActionResult();            
        }
    }
}
