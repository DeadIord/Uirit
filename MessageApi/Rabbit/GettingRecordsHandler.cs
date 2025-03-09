using MassTransit;
using SendService.Core.Commands;

namespace MessageApi.Rabbit
{

    public class GettingRecordsHandler(ILogger<GettingRecordsHandler> logger) : IConsumer<GettingRecordsRequest>
    {
        private readonly ILogger<GettingRecordsHandler> _logger = logger;

        public async Task Consume(ConsumeContext<GettingRecordsRequest> context)
        {
            var searchRequest = context.Message;

            _logger.LogInformation("Получен запрос на поиск: ", searchRequest);

            var searchResult = await PerformSearch();


            var searchResponse = new GettingDocumentResponse { Data = searchResult };

            await context.RespondAsync(searchResponse);
        }

        private async Task<List<object>> PerformSearch()
        {


            var results = new List<object>();
            return results;
        }


    }
}
