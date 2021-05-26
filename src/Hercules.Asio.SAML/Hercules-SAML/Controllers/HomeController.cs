using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hercules_SAML.Models;
using Microsoft.AspNetCore.Http;

namespace Hercules_SAML.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User != null && User.Claims.Count() > 0)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddMinutes(1);
                cookieOptions.Secure = false;
                string guid = new Guid().ToString();

                if (User.Claims.FirstOrDefault(x => x.Type == "grupos" && x.Value == "gnoss-test") != null) // TODO: Obtenerlo del appsettings.
                {
                    guid += "_true";
                }
                else
                {
                    guid += "_false";
                }

                Response.Cookies.Append("cookie_saml", guid, cookieOptions);
                Response.Redirect("https://herc-as-front-desa.atica.um.es/carga-web/public/home/");
            }
            else
            {
                Response.Redirect("http://herc-as-front-desa.atica.um.es/login/Auth/Login/");
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
