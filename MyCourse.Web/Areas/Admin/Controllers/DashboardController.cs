using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Web.Areas.Admin.Models.Dashboard;

namespace MyCourse.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ICourseService _courseService;
        private readonly IApplicationService _applicationService;

        public DashboardController(
            ILogger<DashboardController> logger,
            ICourseService courseService,
            IApplicationService applicationService)
        {

            _logger = logger;
            _courseService = courseService;
            _applicationService = applicationService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var activeCourses = await _courseService.GetAllActiveCoursesAsync();
                var totalRegistrations = 2; // TODO ApplicationSerivce
                var statistics = "Beispiel Statistik"; // TODO Muss ich mir noch überlegen

                var model = new DashboardViewModel
                {
                    ActiveCoursesCount = activeCourses.Count(),
                    TotalRegistrations = totalRegistrations,
                    Statistics = statistics
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Admin Dashboards.");
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Laden des Dashboards aufgetreten";
                return View(new DashboardViewModel());
            }
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CourseCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = new CourseCreateDto
            {
                Title = viewModel.Title!,
                Description = viewModel.Description!,
                CourseDate = viewModel.CourseDate,
                CourseDuration = new TimeSpan(viewModel.CourseDurationHours, viewModel.CourseDurationMinutes, 0),
                MaxParticipants = viewModel.MaxParticipants,
                Location = viewModel.Location!,
                Price = viewModel.Price,
                IsActive = viewModel.IsActive
            };

            try
            {
                await _courseService.CreateCourseAsync(dto);
                TempData["SuccessMessage"] = "Der Kurs wurde erfolgreich erstellt.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Kurses.");
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten.";
                return View(viewModel);
            }
        }
    }
}
