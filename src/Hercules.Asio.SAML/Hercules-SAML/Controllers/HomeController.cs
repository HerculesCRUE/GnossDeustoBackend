using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hercules_SAML.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections;
using Hercules_SAML.Services;

namespace Hercules_SAML.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly ConfigClaimService _ConfigUrlService;

        public HomeController(ILogger<HomeController> logger, ConfigClaimService configUrlService)
        {
            _logger = logger;
            _ConfigUrlService = configUrlService;
        }

        public IActionResult Index(string returnUrl = null)
        {
            if (User != null && User.Claims.Count() > 0)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddMinutes(1); // Tiempo de la cookie.
                cookieOptions.Secure = false;
                string guid = Guid.NewGuid().ToString();

                if (User.Claims.FirstOrDefault(x => x.Type == _ConfigUrlService.GetClaim() && x.Value == _ConfigUrlService.GetValue()) != null)
                {
                    guid += "_true";
                }
                else
                {
                    guid += "_false";
                }

                Response.Cookies.Append("cookie_saml", guid, cookieOptions);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    Response.Redirect(returnUrl);
                }
            }
            else
            {
                Response.Redirect(Url.Content("~/Auth/Login") + "?returnUrl=" + Url.Content("~/") + "?returnUrl=" + returnUrl);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
