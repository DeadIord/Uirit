using MassTransit;
using MessageApi.Data;
using Microsoft.EntityFrameworkCore;
using SendService.Core.Commands;

namespace MessageApi.Rabbit
{
    public class UpdateStatusHandler : IConsumer<UpdateStatusRequset>
    {
        private readonly ILogger<UpdateStatusHandler> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UpdateStatusHandler(ILogger<UpdateStatusHandler> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<UpdateStatusRequset> context)
        {
            _logger.LogInformation("Получен запрос на получение записей");
            var aplicationId = context.Message;


            var records = await _dbContext.Aplication
                .Where(a => a.ServiceNumber == aplicationId.ServiceNumber).FirstAsync();
            records.StatusId = 4;

            await _dbContext.SaveChangesAsync();
        }
    }
}
