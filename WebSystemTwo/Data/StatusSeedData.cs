using Microsoft.EntityFrameworkCore;
using WebSystemTwo.Models;

namespace WebSystemTwo.Data
{
    public static class StatusSeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Statuses.Any())
            {
                return;
            }

            var statuses = new StatusModel[]
            {
                new() {
                    Id = 1,
                    StatusCode = 1010,
                    StatusName = "Запрос подан",
                    Text = "Указанные сведения будут проверены на корректность заполнения обязательных полей. В случае положительного результата проверки запрос будет зарегистрирован в течение одного рабочего дня."
                },
                new() {
                    Id = 2,
                    StatusCode = 1040,
                    StatusName = "Запрос доставлен в ведомство",
                    Text = ""
                },
                new() {
                    Id = 3,
                    StatusCode = 103099,
                    StatusName = "Запрос не доставлен",
                    Text = "Запрос не доставлен. Пожалуйста, подайте запрос повторно."
                },
                new() {
                    Id = 4,
                    StatusCode = 1050,
                    StatusName = "Запрос зарегистрирован",
                    Text = "Присвоен регистрационный № {0} от {1}. Запрос принят к рассмотрению."
                }
            };

            foreach (var status in statuses)
            {
                context.Statuses.Add(status);
            }

            context.SaveChanges();


        }
    }
}