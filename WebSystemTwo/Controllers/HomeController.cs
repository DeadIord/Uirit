using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SendService.Core.Commands;
using System.Diagnostics;
using WebSystemTwo.Models;

namespace WebSystemTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
