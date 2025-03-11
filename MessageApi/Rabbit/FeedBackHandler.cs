// MessageApi.Rabbit/FeedBackHandler.cs
using MassTransit;
using MessageApi.Data;
using MessageApi.Models;
using Microsoft.EntityFrameworkCore;
using SendService.Core.Commands;
using System;
using System.Threading.Tasks;

namespace MessageApi.Rabbit
{
    public class FeedBackHandler : IConsumer<FeedBackRequest>
    {
        private readonly ILogger<FeedBackHandler> _logger;
        private readonly ApplicationDbContext _context;

        public FeedBackHandler(ILogger<FeedBackHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<FeedBackRequest> context)
        {
            var feedbackRequest = context.Message;

            _logger.LogInformation("Получен запрос отзыва от: {LastName} {FirstName}",
                feedbackRequest.Contacts.PrivatePerson[0].LastName,
                feedbackRequest.Contacts.PrivatePerson[0].FirstName);

            var responseData = await ProcessFeedback(feedbackRequest);

            var feedbackResponse = new FeedBackResponse { Data = responseData };
            await context.RespondAsync(feedbackResponse);
        }

        private async Task<int> ProcessFeedback(FeedBackRequest request)
        {
            try
            {
                var fio = request.Contacts.PrivatePerson.FirstOrDefault();
                var newApplication = new ApplicationModel
                {
                    ServiceNumber = request.Service.ServiceNumber,
                    Created = request.Service.RegDate.ToUniversalTime(),
                    Body = request.Properties.Text,
                    Check = false,
                    StatusId = 1,
                    FIO = $"{fio.LastName} {fio.FirstName} {fio.MiddleName}"
                };

                _context.Aplication.Add(newApplication);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Заявка сохранена в БД с ID {Id}", newApplication.Id);
                return 2;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении отзыва в БД");
                return 3;
            }
        }
    }
}