// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para verificar el estado del sistema
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para verificar el estado del sistema
    /// </summary>
    public class CheckSystemController : Controller
    {
        CheckSystemService _checkSystemService;
        public CheckSystemController(CheckSystemService checkSystemService)
        {
            _checkSystemService = checkSystemService;
        }
        public IActionResult Index()
        {
            return View(null);
        }
        /// <summary>
        /// Obtiene un report indicando si los servicios funcionan o no
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/getBasicReport")]
        public IActionResult getReportBasic()
        {
            return View("Index", _checkSystemService.CheckSystem());
        }

        /// <summary>
        /// Obtiene una lista de logs del api seleccionado
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/getLogs")]
        public IActionResult getLogs(string api)
        {
            var logs = _checkSystemService.GetLogs(api);
            List<LogInfoViewModel> model = new List<LogInfoViewModel>();
            if (logs != null)
            {
                foreach (var log in logs)
                {
                    LogInfoViewModel modelLog = new LogInfoViewModel();
                    modelLog.Api = api;
                    modelLog.LastModified = log.Value;
                    modelLog.NameLog = log.Key;
                    model.Add(modelLog);
                }
                return View(model);
            }
            return NotFound();
        }
        /// <summary>
        /// Obtiene una lista de logs del api seleccionado
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/getLog")]
        public IActionResult getLog(string log_name, string api)
        {
            var result = _checkSystemService.GetLog(log_name, api);
            if (!string.IsNullOrEmpty(result))
            {
                var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(result));
                var contentType = "APPLICATION/octet-stream";
                var fileName = $"{log_name}";
                return File(content, contentType, fileName);
            }
            return NotFound();
        }
    }
}