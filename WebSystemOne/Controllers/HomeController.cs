using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebSystemOne.Models;
using WebSystemOne.ViewModel;
using WebSystemOne.Data;
using Microsoft.EntityFrameworkCore;
using WebSystemOne.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WebSystemOne.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ApplicationDb _context;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly FeedbackService _feedbackService;
        private readonly GettingRecordsService _gettingRecordsService;
        public HomeController(ApplicationDb context, UserManager<ApplicationUserModel> userManager, FeedbackService feedbackService, GettingRecordsService gettingRecordsService, IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _userManager = userManager;
            _feedbackService = feedbackService;
            _gettingRecordsService = gettingRecordsService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View(new List<StatementViewModel>());
            }

            var applications = await _context.Aplication
                .Where(a => a.UserId == user.Id)
                .Include(a => a.Status)
                .OrderByDescending(a => a.Created)
                .Select(a => new StatementViewModel
                {
                    Id = a.Id,
                    ServiceNumber = a.ServiceNumber.ToString(),
                    Created = a.Created,
                    Name = a.Status.Name
                })
                .ToListAsync();

            return View(applications);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Privacy()
        {
            var model = new FeedbackViewModel();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.LastName = user.LastName;
                model.FirstName = user.FirstName;
                model.MiddleName = user.MiddleName;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Privacy(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var serviceNumber = GenerateServiceNumber();

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    user.LastName = model.LastName;
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    await _userManager.UpdateAsync(user);
                }

                // Получаем или создаем сервис
                var service = await _context.Service.FindAsync(1) ?? await CreateServiceAsync();

                // Создаем заявку с начальным статусом (id=1 - "Запрос подан")
                var application = new AplicationModel
                {
                    ServiceNumber = serviceNumber,
                    Created = DateTime.UtcNow,
                    Body = model.Body,
                    ServiceId = service.Id,
                    StatusId = 1, // Начальный статус
                    UserId = user?.Id
                };

                _context.Aplication.Add(application);
                await _context.SaveChangesAsync();

                // Запускаем фоновую задачу для обновления статуса
                // без блокировки текущего потока
                _ = UpdateApplicationStatusWithScopeAsync(
                    serviceNumber,
                    model.LastName,
                    model.FirstName,
                    model.MiddleName,
                    model.Body
                );
                TempData["FeedbackStatus"] = "Отзыв успешно отправлен.";
                TempData["ServiceNumber"] = serviceNumber; // Сохраняем номер заявки для использования в представлении

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ошибка сохранения отзыва: " + ex.Message);
                return View(model);
            }
        }

        // 2. Новый асинхронный метод для обновления статуса в фоновом режиме
        private async Task UpdateApplicationStatusWithScopeAsync(
    int serviceNumber,
    string lastName,
    string firstName,
    string middleName,
    string body)
        {
            // Создаем новый скоуп для Dependency Injection,
            // чтобы получить новый экземпляр DbContext
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    // Получаем сервисы из нового скоупа
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDb>();
                    var feedbackService = scope.ServiceProvider.GetRequiredService<FeedbackService>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<HomeController>>();

                    // Отправляем запрос в API и получаем реальный статус
                    var status = await feedbackService.SendFeedback(
                        lastName,
                        firstName,
                        middleName,
                        body,
                        serviceNumber
                    );

                    // Делаем паузу перед обновлением статуса
                    await Task.Delay(5000);

                    // Находим заявку и обновляем статус через новый контекст
                    var application = await dbContext.Aplication
                        .FirstOrDefaultAsync(a => a.ServiceNumber == serviceNumber);

                    if (application != null)
                    {
                        application.StatusId = status;
                        await dbContext.SaveChangesAsync();

                        // Логируем успешное обновление
                        logger.LogInformation($"Статус заявки {serviceNumber} обновлен на {status}");
                    }
                    else
                    {
                        logger.LogWarning($"Заявка с номером {serviceNumber} не найдена для обновления статуса");
                    }
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<HomeController>>();
                    logger.LogError(ex, $"Ошибка при обновлении статуса заявки {serviceNumber}");
                }
            }
        }
        private async Task<ServiceModel> CreateServiceAsync()
        {
            var service = new ServiceModel
            {
                Id = 1,
                ServiceNumber = "Отзыв о работе ресурса"
            };
            _context.Service.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }


        private int GenerateServiceNumber()
        {
            return Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            await _gettingRecordsService.GettingRecords();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new List<StatementViewModel>());
            }

            var applications = await _context.Aplication
                .Where(a => a.UserId == user.Id)
                .Include(a => a.Status)
                .OrderByDescending(a => a.Created)
                .Select(a => new StatementViewModel
                {
                    Id = a.Id,
                    ServiceNumber = a.ServiceNumber.ToString(),
                    Created = a.Created,
                    Name = a.Status.Name
                })
                .ToListAsync();

            return Json(applications);
        }
    }
}