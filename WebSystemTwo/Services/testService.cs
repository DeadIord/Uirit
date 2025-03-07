using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SendService.Core.Commands;
using WebSystemTwo.Controllers;

namespace WebSystemOne.Controllers
{
    public class testService
    {

        private readonly ILogger<testService> _logger;
        private readonly IRequestClient<GettingDocumentRequset> _GettingDocumentRequestClient;

        public testService(ILogger<testService> logger, IRequestClient<GettingDocumentRequset> gettingDocumentRequest)
        {
            _logger = logger;
            _GettingDocumentRequestClient = gettingDocumentRequest;
        }

        public async Task test()
        {
            string text = "";
            var gettingDocumentRequest = new GettingDocumentRequset { Text = text };
            _logger.LogInformation("Отправка запроса отзыва: {Text}", gettingDocumentRequest.Text);

            var response = await _GettingDocumentRequestClient.GetResponse<GettingDocumentResponse>(gettingDocumentRequest);

            _logger.LogInformation("Получен ответ на запроса отзыва");

            var test = response.Message.Data;
        }
    }
}
