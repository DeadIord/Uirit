using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSystemTwo.Data;
using WebSystemTwo.Models;

namespace WebSystemTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var applications = await _db.Applications.Include(a => a.Status).ToListAsync();
            return View(applications);
        }

        public async Task<IActionResult> Register(long id)
        {
            var application = await _db.Applications.Include(a => a.Status)
                                                    .FirstOrDefaultAsync(a => a.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            application.StatusId = 4;
            application.ServiceNumber = string.Format("Присвоен регистрационный № {0} от {1}",
                                                      application.Id,
                                                      DateTime.Now.ToString("dd.MM.yyyy"));

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(IndexAsync));
        }
    }
}
