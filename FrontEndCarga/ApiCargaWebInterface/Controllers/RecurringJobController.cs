using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    public class RecurringJobController : Controller
    {
        public IActionResult Index()
        {
            List<CreateRecurringJobViewModel> lista = new List<CreateRecurringJobViewModel>();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}