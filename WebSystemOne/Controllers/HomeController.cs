using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebSystemOne.Models;
using WebSystemOne.ViewModel;
using WebSystemOne.Data;
using Microsoft.EntityFrameworkCore;
using WebSystemOne.Services;

namespace WebSystemOne.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDb _context;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly FeedbackService _feedbackService;

        public HomeController(ApplicationDb context, UserManager<ApplicationUserModel> userManager, FeedbackService feedbackService)
        {
            _context = context;
            _userManager = userManager;
            _feedbackService = feedbackService;
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
            if (ModelState.IsValid)
            {
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

                    var service = await _context.Service.FirstOrDefaultAsync(s => s.Id == 1);
                    if (service == null)
                    {
                        service = new ServiceModel
                        {
                            Id = 1,
                            ServiceNumber = "Отзыв о работе ресурса"
                        };
                        _context.Service.Add(service);
                        await _context.SaveChangesAsync();
                    }

                    var status = await _context.Status.FirstOrDefaultAsync(s => s.Id == 1);
                    if (status == null)
                    {
                        status = new StatusModel
                        {
                            Id = 1,
                            Code = 1010,
                            Name = "Запрос подан",
                            Text = "Указанные сведения будут проверены на корректность заполнения обязательных полей."
                        };
                        _context.Status.Add(status);
                        await _context.SaveChangesAsync();
                    }

                    var application = new AplicationModel
                    {
                        ServiceNumber = GenerateServiceNumber(),
                        Created = DateTime.UtcNow,
                        Body = model.Body,
                        ServiceId = 1,
                        StatusId = 1
                    };

                    if (user != null)
                    {
                        application.UserId = user.Id;
                    }

                    if (_feedbackService != null)
                    {
                        string apiResponse = await _feedbackService.SendFeedback(
                            model.LastName,
                            model.FirstName,
                            model.MiddleName,
                            model.Body
                        );
                    }

                    _context.Aplication.Add(application);
                    await _context.SaveChangesAsync();

                    TempData["FeedbackStatus"] = "Отзыв успешно отправлен.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Ошибка сохранения отзыва: " + ex.Message);
                }
            }
            return View(model);
        }


        private int GenerateServiceNumber()
        {
            return Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
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
                    ServiceNumber = a.ServiceNumber.ToString(),
                    Created = a.Created,
                    Name = a.Status.Name
                })
                .ToListAsync();

            return Json(applications);
        }
    }
}