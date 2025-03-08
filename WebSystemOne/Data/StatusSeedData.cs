using Microsoft.EntityFrameworkCore;
using WebSystemOne.Models;
using WebSystemOne.Data;

namespace WebSystemOne.Data
{
    public static class StatusSeedData
    {
        public static void Initialize(ApplicationDb context)
        {
            context.Database.EnsureCreated();

            if (context.Status.Any())
            {
                return;
            }

            var statuses = new StatusModel[]
            {
                new StatusModel
                {
                    Id = 1,
                    Code = 1010,
                    Name = "Запрос подан",
                    Text = "Указанные сведения будут проверены на корректность заполнения обязательных полей. В случае положительного результата проверки запрос будет зарегистрирован в течение одного рабочего дня."
                },
                new StatusModel
                {
                    Id = 2,
                    Code = 1040,
                    Name = "Запрос доставлен в ведомство",
                    Text = ""
                },
                new StatusModel
                {
                    Id = 3,
                    Code = 103099,
                    Name = "Запрос не доставлен",
                    Text = "Запрос не доставлен. Пожалуйста, подайте запрос повторно."
                },
                new StatusModel
                {
                    Id = 4,
                    Code = 1050,
                    Name = "Запрос зарегистрирован",
                    Text = "Присвоен регистрационный № {0} от {1}. Запрос принят к рассмотрению."
                }
            };

            foreach (var status in statuses)
            {
                context.Status.Add(status);
            }

            context.SaveChanges();

            if (context.Service.Any())
            {
                return;  
            }

            var services = new ServiceModel[]
            {
                new ServiceModel
                {
                    Id = 1,
                    ServiceNumber = "Отзыв о работе ресурса"
                }
            };

            foreach (var service in services)
            {
                context.Service.Add(service);
            }

            context.SaveChanges();
        }
    }
}