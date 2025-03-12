using MassTransit;
using Microsoft.EntityFrameworkCore;
using SendService.Core.Commands;
using System.Xml.Serialization;
using WebSystemTwo.Data;
using WebSystemTwo.Models;

namespace WebSystemTwo.Services
{

    public class testService
    {
        private readonly ILogger<testService> _logger;
        private readonly IRequestClient<GettingDocumentRequset> _gettingDocumentRequestClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly IBus _bus;

        public testService(
            ILogger<testService> logger,
            IRequestClient<GettingDocumentRequset> gettingDocumentRequest,
            ApplicationDbContext dbContext,
            IBus bus)
        {
            _logger = logger;
            _gettingDocumentRequestClient = gettingDocumentRequest;
            _dbContext = dbContext;
            _bus = bus;
        }

        public async Task<bool> UpdateStatusAsync(int ServiceNumber)
        {
            try
            {
                _logger.LogInformation("Отправка запроса на обновление статуса для ServiceNumber: {ServiceNumber}", ServiceNumber);

                var updateStatus = new UpdateStatusRequset { ServiceNumber = ServiceNumber };

                // Создаем CancellationToken с таймаутом 5 секунд
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                // Запускаем задачу отправки сообщения в RabbitMQ с таймаутом
                var publishTask = _bus.Publish(updateStatus, cts.Token);

                await publishTask;

                _logger.LogInformation("Запрос на обновление статуса отправлен для ServiceNumber: {ServiceNumber}", ServiceNumber);
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Таймаут при отправке запроса на обновление статуса для ServiceNumber: {ServiceNumber}. RabbitMQ недоступен.", ServiceNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке запроса на обновление статуса для ServiceNumber: {ServiceNumber}", ServiceNumber);
                return false;
            }
        }
        public async Task ProcessDocumentsAsync()
        {
            try
            {
                _logger.LogInformation("Отправка запроса на получение документов");
                var response = await _gettingDocumentRequestClient.GetResponse<GettingDocumentResponse>(new GettingDocumentRequset());
                _logger.LogInformation("Получен ответ с документами");

                var xmlData = response.Message.Data;
                if (string.IsNullOrEmpty(xmlData))
                {
                    _logger.LogWarning("Получены пустые данные");
                    return;
                }

                // Десериализация XML в объекты
                var applications = DeserializeXml(xmlData);
                if (applications == null || !applications.Any())
                {
                    _logger.LogWarning("Не удалось десериализовать данные или список пуст");
                    return;
                }

                _logger.LogInformation($"Десериализовано {applications.Count} записей");

                // Сохранение в БД
                await SaveApplicationsToDbAsync(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке документов");
                throw;
            }
        }

        private List<ApplicationXmlDto> DeserializeXml(string xmlData)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ApplicationXmlDtoList));
                using (StringReader reader = new StringReader(xmlData))
                {
                    var result = (ApplicationXmlDtoList)serializer.Deserialize(reader);
                    return result?.Applications ?? new List<ApplicationXmlDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при десериализации XML: {Message}", ex.Message);
                // Добавляем подробную информацию о XML для отладки
                _logger.LogDebug("Содержимое XML: {XmlContent}", xmlData);
                return new List<ApplicationXmlDto>();
            }
        }

        private async Task SaveApplicationsToDbAsync(List<ApplicationXmlDto> applications)
        {
            try
            {
                // Проверяем, что контекст не уничтожен
                if (_dbContext == null)
                {
                    _logger.LogError("DbContext равен null");
                    throw new InvalidOperationException("DbContext не инициализирован");
                }

                // Преобразуем XML DTO в модели для БД
                var applicationModels = applications.Select(a => a.ToApplicationModel()).ToList();

                _logger.LogInformation("Начинаем добавление {Count} записей в БД", applicationModels.Count);

                // Проверка на существующие записи и их обновление вместо добавления
                foreach (var model in applicationModels)
                {
                    var existingEntity = await _dbContext.Aplication
                        .FirstOrDefaultAsync(a => a.Id == model.Id);

                    if (existingEntity != null)
                    {
                        // Обновляем существующую запись
                        _dbContext.Entry(existingEntity).CurrentValues.SetValues(model);
                        _logger.LogDebug("Обновлена существующая запись с Id {Id}", model.Id);
                    }
                    else
                    {
                        // Добавляем новую запись
                        await _dbContext.Aplication.AddAsync(model);
                        _logger.LogDebug("Добавлена новая запись с Id {Id}", model.Id);
                    }
                }

                // Сохраняем изменения
                int savedCount = await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Сохранено {Count} записей в БД", savedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении в БД: {Message}", ex.Message);
                // Дополнительная диагностика
                if (ex is DbUpdateException dbEx)
                {
                    _logger.LogError("Внутреннее исключение: {InnerException}", dbEx.InnerException?.Message);
                }
                throw;
            }
        }

        [XmlRoot("Application")]
        public class ApplicationXmlDto
        {
            [XmlElement("Id")]
            public long Id { get; set; }

            [XmlElement("ServiceNumber")]
            public int ServiceNumber { get; set; }

            [XmlElement("Created")]
            public DateTime Created { get; set; }

            [XmlElement("Body")]
            public string Body { get; set; }

            [XmlElement("StatusId")]
            public long StatusId { get; set; }

            [XmlElement("Check")]
            public bool Check { get; set; }

            [XmlElement("FIO")]
            public string FIO { get; set; }

            [XmlElement("User")]
            public string User { get; set; }

            public ApplicationModel ToApplicationModel()
            {
                return new ApplicationModel
                {
                    Id = Id,
                    ServiceNumber = ServiceNumber,
                    Created = Created,
                    Body = Body,
                    StatusId = StatusId,
                    FIO = FIO,
                    User = User ?? "System" // Значение по умолчанию, если поле не заполнено
                };
            }
        }

        [XmlRoot("Applications")]
        public class ApplicationXmlDtoList
        {
            [XmlElement("Application")]
            public List<ApplicationXmlDto> Applications { get; set; }
        }
    }
}