using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSystemOne.Controllers;
using WebSystemTwo.Data;
using WebSystemTwo.Models;

namespace WebSystemTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly testService _testService;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, testService testService)
        {
            _logger = logger;
            _db = db;
            _testService = testService;
        }

        public async Task<IActionResult> IndexAsync()
        {
      await    _testService.ProcessDocumentsAsync();

            var applications = await _db.Aplication.Include(a => a.Status).ToListAsync();
            return View(applications);
        }

        public async Task<IActionResult> Register(long id)
        {
            var application = await _db.Aplication.Include(a => a.Status)
            .FirstOrDefaultAsync(a => a.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            application.StatusId = 4;
            application.ServiceNumber = string.Format("�������� ��������������� � {0} �� {1}",
            application.Id,
            DateTime.Now.ToString("dd.MM.yyyy"));

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(IndexAsync));
        }
    }
}
