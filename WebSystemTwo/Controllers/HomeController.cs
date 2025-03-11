using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSystemTwo.Data;
using WebSystemTwo.Models;
using WebSystemTwo.Services;

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
            await _db.SaveChangesAsync();

            await _testService.UpdateStatusAsync(application.ServiceNumber);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            await _testService.ProcessDocumentsAsync();

            var applications = await _db.Aplication
         .Include(a => a.Status)
         .Select(a => new
         {
             a.Id,
             a.FIO,
             a.Created,
             a.ServiceNumber,
             a.Status.StatusName,
             a.Body
         })
         .ToListAsync();

            return Json(applications);
        }
    }
}
