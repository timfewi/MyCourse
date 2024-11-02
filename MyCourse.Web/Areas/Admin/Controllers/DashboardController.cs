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
                var coursesWithApplications = new List<CourseWithApplicationsDto>();

                foreach (var course in activeCourses)
                {
                    var applications = await _applicationService.GetApplicationsByCourseIdAsync(course.Id);
                    var courseDto = new CourseWithApplicationsDto
                    {
                        CourseId = course.Id,
                        Title = course.Title,
                        CourseDate = course.CourseDate.ToString("dd.MM.yyyy"),
                        Applications = applications.Select(a => new Domain.DTOs.ApplicationDtos.ApplicationDetailDto
                        {
                            Id = a.Id,
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            Email = a.Email,
                            PhoneNumber = a.PhoneNumber,
                            ExperienceLevel = a.ExperienceLevel,
                            PreferredStyle = a.PreferredStyle,
                            Comments = a.Comments,
                            ApplicationDate = a.ApplicationDate,
                            Status = a.Status,
                            StatusDisplayName = a.StatusDisplayName,
                            
                        }).ToList()
                    };
                    coursesWithApplications.Add(courseDto);
                }
                var totalRegistrations = coursesWithApplications.Sum(c => c.Applications.Count);
                var statistics = "Beispiel Statistik"; // TODO Muss ich mir noch überlegen

                var model = new DashboardViewModel
                {
                    ActiveCoursesCount = activeCourses.Count(),
                    TotalRegistrations = totalRegistrations,
                    Statistics = statistics,
                    CoursesWithApplications = coursesWithApplications
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptApplication(int applicationId)
        {
            try
            {
                await _applicationService.AcceptApplicationAsync(applicationId);
                TempData["SuccessMessage"] = "Die Anmeldung wurde erfolgreich akzeptiert.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Akzeptieren der Anmeldung {ApplicationId}.", applicationId);
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Akzeptieren der Anmeldung aufgetreten.";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectApplication(int applicationId)
        {
            try
            {
                await _applicationService.RejectApplicationAsync(applicationId);
                TempData["SuccessMessage"] = "Die Anmeldung wurde erfolgreich abgelehnt.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Ablehnen der Anmeldung {ApplicationId}.", applicationId);
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Ablehnen der Anmeldung aufgetreten.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetToWaitingList(int applicationId)
        {
            try
            {
                await _applicationService.SetApplicationToWaitingListAsync(applicationId);
                TempData["SuccessMessage"] = "Die Anmeldung wurde erfolgreich auf die Warteliste gesetzt.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Setzen der Anmeldung {ApplicationId} auf die Warteliste.", applicationId);
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Setzen der Anmeldung auf die Warteliste aufgetreten.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
