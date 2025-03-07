using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebSystemOne.Models;
using WebSystemOne.ViewModel;
using WebSystemOne.Data;

namespace WebSystemOne.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDb _context;
        private readonly UserManager<ApplicationUserModel> _userManager;

        public HomeController(ApplicationDb context, UserManager<ApplicationUserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
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
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    user.LastName = model.LastName;
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    await _userManager.UpdateAsync(user);
                }

                var application = new AplicationModel
                {
                    ServiceNumber = GenerateServiceNumber(),
                    Created = DateTime.UtcNow,
                    Body = model.Body,
                    
                    ServiceId = 1,
                    StatusId = 1,
                    UserId = user?.Id
                };

                _context.Aplication.Add(application);
                await _context.SaveChangesAsync();

                TempData["FeedbackStatus"] = "Отзыв успешно отправлен.";
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        private int GenerateServiceNumber()
        {
            return Math.Abs(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
        }
    }
}
