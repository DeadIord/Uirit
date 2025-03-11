using MassTransit;
using MessageApi.Data;
using MessageApi.Models;
using Microsoft.EntityFrameworkCore;
using SendService.Core.Commands;
using System.Xml.Serialization;

public class GettingDocumentHandler : IConsumer<GettingDocumentRequset>
{
    private readonly ILogger<GettingDocumentHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public GettingDocumentHandler(ILogger<GettingDocumentHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<GettingDocumentRequset> context)
    {
        var searchRequest = context.Message;

        _logger.LogInformation("Получен запрос на поиск документов");

        try
        {
            var applications = await PerformSearch();

            // Преобразуем список приложений в XML через DTO
            string xmlData = ConvertToXml(applications);

            var searchResponse = new GettingDocumentResponse { Data = xmlData };

            await context.RespondAsync(searchResponse);
            _logger.LogInformation("Отправлен ответ с {Count} записями", applications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке запроса");
            throw;
        }
    }

    private async Task<List<ApplicationModel>> PerformSearch()
    {
        var applications = await _dbContext.Aplication
            .Include(a => a.Status)
            .Where(a => a.StatusId == 1 && !a.Check)
            .ToListAsync();

        return applications;
    }
   
    // Использование в обработчике XML
    private string ConvertToXml(List<ApplicationModel> applications)
    {
        // Преобразуем Entity-модели в XML DTO
        var applicationDtos = applications.Select(ApplicationXmlDto.FromEntity).ToList();

        // Создаем класс-обертку для сериализации списка
        var wrapper = new ApplicationXmlDtoList
        {
            Applications = applicationDtos
        };

        // Сериализуем в XML
        XmlSerializer serializer = new XmlSerializer(typeof(ApplicationXmlDtoList));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, wrapper);
            return writer.ToString();
        }
    }
    [XmlRoot("Applications")]
    public class ApplicationXmlDtoList
    {
        [XmlElement("Application")]
        public List<ApplicationXmlDto> Applications { get; set; }
    }
    [XmlRoot("Application")]
    public class ApplicationXmlDto
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("ServiceNumber")]
        public int ServiceNumber { get; set; }

        [XmlElement("Created")]
        public DateTime Created { get; set; }

        [XmlElement("Body")]
        public string Body { get; set; }

        [XmlElement("StatusId")]
        public int StatusId { get; set; }

        [XmlElement("Check")]
        public bool Check { get; set; }
        [XmlElement("FIO")]
        public string FIO { get; set; }

        // Метод для преобразования из обычного DTO или из Entity    
        public static ApplicationXmlDto FromEntity(ApplicationModel entity)
        {
            return new ApplicationXmlDto
            {
                Id = entity.Id,
                ServiceNumber = entity.ServiceNumber,
                Created = entity.Created,
                Body = entity.Body,
                StatusId = entity.StatusId,
                Check = entity.Check,
                FIO = entity.FIO
            };
        }
    }

}
