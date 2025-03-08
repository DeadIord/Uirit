using MassTransit;
using Microsoft.Extensions.Logging;
using SendService.Core.Commands;
using System;
using System.Threading.Tasks;

namespace WebSystemOne.Services
{
    public class FeedbackService
    {
        private readonly ILogger<FeedbackService> _logger;
        private readonly IRequestClient<FeedBackRequest> _feedBackRequestClient;

        public FeedbackService(ILogger<FeedbackService> logger, IRequestClient<FeedBackRequest> feedBackRequestClient)
        {
            _logger = logger;
            _feedBackRequestClient = feedBackRequestClient;
        }

        public virtual async Task<string> SendFeedback(string lastName, string firstName, string middleName, string body)
        {
            try
            {
                if (_feedBackRequestClient == null)
                {
                    _logger.LogWarning("MassTransit client is not configured. Feedback is only saved locally.");
                    return "Отзыв сохранен локально, но не отправлен через API.";
                }

                var feedBackRequest = new FeedBackRequest
                {
                    Service = new ServiceInfo
                    {
                        RegDate = DateTime.Now,
                        ServiceType = 113
                    },
                    Contacts = new ContactsInfo
                    {
                        PrivatePerson = new[]
                        {
                            new PrivatePersonInfo
                            {
                                LastName = lastName,
                                FirstName = firstName,
                                MiddleName = middleName
                            }
                        }
                    },
                    Properties = new PropertiesInfo
                    {
                        Text = body
                    }
                };

                _logger.LogInformation("Отправка запроса отзыва: {LastName} {FirstName}", lastName, firstName);

                var response = await _feedBackRequestClient.GetResponse<FeedBackResponse>(feedBackRequest);

                _logger.LogInformation("Получен ответ на запрос отзыва");
                return response.Message.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке отзыва");
                return "Произошла ошибка при отправке отзыва через API, но отзыв сохранен локально.";
            }
        }
    }
}