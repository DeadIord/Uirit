using MassTransit;
using SendService.Core.Commands;

namespace WebSystemOne.Services
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MassTransit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WebSystemOne.Models;
    using WebSystemOne.Data;

    public class GettingRecordsService
    {
        private readonly ILogger<GettingRecordsService> _logger;
        private readonly IRequestClient<GettingRecordsRequest> _feedBackRequestClient;
        private readonly ApplicationDb _dbContext;

        public GettingRecordsService(
            ILogger<GettingRecordsService> logger,
            IRequestClient<GettingRecordsRequest> feedBackRequestClient,
            ApplicationDb dbContext)
        {
            _logger = logger;
            _feedBackRequestClient = feedBackRequestClient;
            _dbContext = dbContext;
        }

        public async Task GettingRecords()
        {
            try
            {
                _logger.LogInformation("Отправка запроса в микросервис");

                var response = await _feedBackRequestClient.GetResponse<GettingRecordsResponse>(new GettingRecordsRequest());

                _logger.LogInformation($"Получен ответ: {response.Message.Data.Count} записей");

                var serviceNumbers = response.Message.Data.Select(r => r.ServiceNumber).ToList();

                var applicationsToUpdate = await _dbContext.Aplication
                    .Where(a => serviceNumbers.Contains(a.ServiceNumber))
                    .ToListAsync();

                if (!applicationsToUpdate.Any())
                {
                    _logger.LogInformation("Не найдено соответствующих записей в локальной БД.");  }

                // Обновляем StatusId на 4
                foreach (var app in applicationsToUpdate)
                {
                    app.StatusId = 4;
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Обновлено {applicationsToUpdate.Count} записей");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении и обработке записей");
                throw;
            }
        }
    }

}
