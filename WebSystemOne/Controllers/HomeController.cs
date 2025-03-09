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
        private readonly GettingRecordsService _gettingRecordsService;
        public HomeController(ApplicationDb context, UserManager<ApplicationUserModel> userManager, FeedbackService feedbackService, GettingRecordsService gettingRecordsService)
        {
            _context = context;
            _userManager = userManager;
            _feedbackService = feedbackService;
            _gettingRecordsService = gettingRecordsService;
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

               var apiResponse =  await _feedbackService.SendFeedback(model.LastName, model.FirstName, model.MiddleName, model.Body, serviceNumber);
                var status = apiResponse;

                // �������� ��� ������� ������ � ������
                var service = await _context.Service.FindAsync(1) ?? await CreateServiceAsync();

                var application = new AplicationModel
                {
                    ServiceNumber = serviceNumber,
                    Created = DateTime.UtcNow,
                    Body = model.Body,
                    ServiceId = service.Id,
                    StatusId = status,
                    UserId = user?.Id
                };

                _context.Aplication.Add(application);
                await _context.SaveChangesAsync();

                TempData["FeedbackStatus"] = "����� ������� ���������.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "������ ���������� ������: " + ex.Message);
                return View(model);
            }
        }

        private async Task<ServiceModel> CreateServiceAsync()
        {
            var service = new ServiceModel
            {
                Id = 1,
                ServiceNumber = "����� � ������ �������"
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
                    ServiceNumber = a.ServiceNumber.ToString(),
                    Created = a.Created,
                    Name = a.Status.Name
                })
                .ToListAsync();

            return Json(applications);
        }
    }
}