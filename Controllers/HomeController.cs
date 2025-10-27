using Library_Manager.Filters;
using Library_Manager.Models;
using Library_Manager   .Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library_Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //[Authentication]
        [Authorization("QTV,QLB,QLT,QLM")]
        public IActionResult Index()
        {
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
