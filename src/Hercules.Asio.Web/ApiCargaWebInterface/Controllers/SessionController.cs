using ApiCargaWebInterface.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.Web.Controllers
{
    public class SessionController : Controller
    {
        readonly ConfigUrlService _ConfigUrlService;

        public SessionController(ConfigUrlService configUrlService)
        {
            _ConfigUrlService = configUrlService;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Remove("session_saml");
            return Redirect( _ConfigUrlService.GetUrlSAMLLogin() + "/Auth/Logout");
            //return Redirect(_ConfigUrlService.GetUrlFront() + _ConfigUrlService.GetProxy());
        }
    }
}
