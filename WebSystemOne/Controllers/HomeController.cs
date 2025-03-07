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
        private readonly IRequestClient<FeedBackRequest> _FeedBackRequestClient ;

        public HomeController(ILogger<HomeController> logger, IRequestClient<FeedBackRequest> feedBackRequest)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string text = "";
            var feedBackRequest = new FeedBackRequest { Text = text };
            _logger.LogInformation("Отправка запроса отзыва: {Text}", feedBackRequest.Text);

            var response = await _FeedBackRequestClient.GetResponse<FeedBackResponse>(feedBackRequest);

            _logger.LogInformation("Получен ответ на запроса отзыва");

            var test = response.Message.Data;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
