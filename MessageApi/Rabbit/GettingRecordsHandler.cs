using MassTransit;
using MessageApi.Data;
using Microsoft.EntityFrameworkCore;
using SendService.Core.Commands;

namespace MessageApi.Rabbit
{

    public class GettingRecordsHandler : IConsumer<GettingRecordsRequest>
    {
        private readonly ILogger<GettingRecordsHandler> _logger;
        private readonly ApplicationDbContext _dbContext;

        public GettingRecordsHandler(ILogger<GettingRecordsHandler> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<GettingRecordsRequest> context)
        {
            _logger.LogInformation("Получен запрос на получение записей");

            var records = await _dbContext.Aplication
                .Where(a => a.StatusId == 4 && !a.Check)
                .ToListAsync();

            _logger.LogInformation($"Найдено {records.Count} записей");

            foreach (var record in records)
            {
                record.Check = true;
            }

            await _dbContext.SaveChangesAsync();

            var response = new GettingRecordsResponse
            {
                Data = records.Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    ServiceNumber = a.ServiceNumber,
                    Created = a.Created,
                    Body = a.Body,
                    StatusId = a.StatusId,
                    Check = a.Check
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }

}
