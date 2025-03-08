using MassTransit;
using SendService.Core.Commands;

namespace MessageApi.Rabbit
{
    public class FeedBackHandler(ILogger<FeedBackHandler> logger) : IConsumer<FeedBackRequest>
    {
        private readonly ILogger<FeedBackHandler> _logger = logger;

        public async Task Consume(ConsumeContext<FeedBackRequest> context)
        {
            var searchRequest = context.Message;

            _logger.LogInformation("Получен запрос на поиск: {Text}", searchRequest.Text);

            var searchResult = await PerformSearch(searchRequest.Text);


            var searchResponse = new FeedBackResponse { Data = searchResult };

            await context.RespondAsync(searchResponse);
        }

        private async Task<string> PerformSearch(string searchText)
        {


            var results = "Отправлено";
            return results;
        }


    }
}
