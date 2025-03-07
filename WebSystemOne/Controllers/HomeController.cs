using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SendService.Core.Commands;
using WebSystemOne.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebSystemOne.Controllers
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
