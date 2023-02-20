using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppCmvc.Models;

namespace WebAppCmvc.Controllers
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
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}