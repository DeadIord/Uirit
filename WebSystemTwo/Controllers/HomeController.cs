using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            var applications = await _db.Aplication
                .Include(a => a.Status)
                .OrderByDescending(a => a.Created)
                .ToListAsync();
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

            try
            {
                var result = await _testService.UpdateStatusAsync(application.ServiceNumber);


                if (result)
                {
                    application.StatusId = 4;
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "��������� ������� ����������������";
                }
                else
                {
                    TempData["ErrorMessage"] = "�� ������� �������� ������. ������ ����������. ���������� �����.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ��� ��������� ��������� {Id}", id);
                TempData["ErrorMessage"] = "��������� ������ ��� ��������� ���������. ���������� �����.";
            }

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
                .OrderByDescending(a => a.Created)
                .ToListAsync();

            return Json(applications);
        }
    }
}