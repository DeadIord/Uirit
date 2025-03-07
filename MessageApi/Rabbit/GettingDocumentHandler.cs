using MassTransit;
using SendService.Core.Commands;

namespace MessageApi.Rabbit
{
    public class GettingDocumentHandler(ILogger<GettingDocumentHandler> logger) : IConsumer<GettingDocumentRequset>
    {
        private readonly ILogger<GettingDocumentHandler> _logger = logger;

        public async Task Consume(ConsumeContext<GettingDocumentRequset> context)
        {
            var searchRequest = context.Message;

            _logger.LogInformation("Получен запрос на поиск: {Text}", searchRequest.Text);

            var searchResult = await PerformSearch(searchRequest.Text);


            var searchResponse = new FeedBackResponse { Data = searchResult };

            await context.RespondAsync(searchResponse);
        }

        private async Task<List<object>> PerformSearch(string searchText)
        {


            var results = new List<object>();
            return results;
        }


    }
}
