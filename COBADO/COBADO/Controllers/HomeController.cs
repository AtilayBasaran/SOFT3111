using COBADO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace COBADO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult index()
        {
            return View();
        }
        public IActionResult about_us()
        {
            return View();
        }
        public IActionResult adminPanel()
        {
            return View();
        }
        public IActionResult all_event()
        {
            return View();
        }
        public IActionResult buy_page()
        {
            return View();
        }
        public IActionResult categories()
        {
            return View();
        }
        public IActionResult contact_us()
        {
            return View();
        }
        public IActionResult events()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult login()
        {
            return View();
        }
        public IActionResult music()
        {
            return View();
        }
        public IActionResult ownerEvent()
        {
            return View();
        }
        public IActionResult ownerPanel()
        {
            return View();
        }
        public IActionResult payment()
        {
            return View();
        }
        public IActionResult profile()
        {
            return View();
        }
        public IActionResult register()
        {
            return View();
        }
        public IActionResult security()
        {
            return View();
        }
        public IActionResult sport()
        {
            return View();
        }
        public IActionResult successPage()
        {
            return View();
        }
        public IActionResult talk()
        {
            return View();
        }
        public IActionResult theatre()
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
