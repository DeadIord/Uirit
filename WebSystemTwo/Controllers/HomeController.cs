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
        private readonly IRequestClient<GettingDocumentRequset> _GettingDocumentRequestClient;

        public HomeController(ILogger<HomeController> logger, IRequestClient<GettingDocumentRequset> gettingDocumentRequest)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string text = "";
            var gettingDocumentRequest = new GettingDocumentRequset { Text = text };
            _logger.LogInformation("Отправка запроса отзыва: {Text}", gettingDocumentRequest.Text);

            var response = await _GettingDocumentRequestClient.GetResponse<GettingDocumentResponse>(gettingDocumentRequest);

            _logger.LogInformation("Получен ответ на запроса отзыва");

            var test = response.Message.Data;
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
