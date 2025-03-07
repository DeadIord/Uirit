using MassTransit;
using SendService.Core.Commands;

namespace WebSystemOne.Services
{
    public class testService
    {

        private readonly ILogger<testService> _logger;
        private readonly IRequestClient<FeedBackRequest> _FeedBackRequestClient;

        public testService(ILogger<testService> logger, IRequestClient<FeedBackRequest> feedBackRequest)
        {
            _logger = logger;
            _FeedBackRequestClient = feedBackRequest;
        }
        public async Task test()
        {
            string text = "";
            var feedBackRequest = new FeedBackRequest { Text = text };
            _logger.LogInformation("Отправка запроса отзыва: {Text}", feedBackRequest.Text);

            var response = await _FeedBackRequestClient.GetResponse<FeedBackResponse>(feedBackRequest);

            _logger.LogInformation("Получен ответ на запроса отзыва");

            var test = response.Message.Data;
        }
    }
}
