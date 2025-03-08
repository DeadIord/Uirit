// MessageApi.Rabbit/FeedBackHandler.cs
using MassTransit;
using SendService.Core.Commands;
using System;
using System.Threading.Tasks;

namespace MessageApi.Rabbit
{
    public class FeedBackHandler : IConsumer<FeedBackRequest>
    {
        private readonly ILogger<FeedBackHandler> _logger;

        public FeedBackHandler(ILogger<FeedBackHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FeedBackRequest> context)
        {
            var feedbackRequest = context.Message;

            _logger.LogInformation("Получен запрос отзыва от: {LastName} {FirstName}",
                feedbackRequest.Contacts.PrivatePerson[0].LastName,
                feedbackRequest.Contacts.PrivatePerson[0].FirstName);

            // Process the feedback request
            var responseData = await ProcessFeedback(feedbackRequest);

            // Return the response
            var feedbackResponse = new FeedBackResponse { Data = responseData };
            await context.RespondAsync(feedbackResponse);
        }

        private async Task<string> ProcessFeedback(FeedBackRequest request)
        {
            try
            {
                return "Отзыв успешно обработан";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки отзыва");
                return "Ошибка обработки отзыва: " + ex.Message;
            }
        }
    }
}