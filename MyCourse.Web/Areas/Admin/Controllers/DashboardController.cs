using FluentValidation;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Web.Areas.Admin.Models.Dashboard;
using System.Net;
using System.Net.Mail;

namespace MyCourse.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICourseService _courseService;
        private readonly IApplicationService _applicationService;
        private readonly IMediaService _mediaService;
        private readonly IEmailService _emailService;
        public DashboardController(
            ILogger<DashboardController> logger,
            IWebHostEnvironment hostingEnvironment,
            ICourseService courseService,
            IApplicationService applicationService,
            IMediaService mediaService,
            IEmailService emailService)
        {

            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _courseService = courseService;
            _applicationService = applicationService;
            _mediaService = mediaService;
            _emailService = emailService;
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
                var courseId = await _courseService.CreateCourseAsync(dto);

                if (viewModel.Images != null && viewModel.Images.Count > 0)
                {
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var maxFileSize = 10 * 1024 * 1024; // 10 MB

                    foreach (var formFile in viewModel.Images)
                    {
                        try
                        {
                            if (formFile.Length > 0 && formFile.Length <= maxFileSize)
                            {
                                var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();

                                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                                {
                                    ModelState.AddModelError("Images", $"Die Datei {formFile.FileName} hat ein ungültiges Format.");
                                    continue;
                                }

                                // Sicherstellen, dass der Dateiname sicher ist
                                var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                                fileName = SanitizeFileName(fileName);

                                var newFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "images");

                                // Überprüfe, ob der Ordner existiert, und erstelle ihn bei Bedarf
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                var filePath = Path.Combine(uploadsFolder, newFileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await formFile.CopyToAsync(stream);
                                }

                                var mediaDto = new MediaCreateDto()
                                {
                                    Url = $"/uploads/images/{newFileName}",
                                    FileName = newFileName,
                                    MediaType = Domain.Enums.MediaType.Image,
                                    ContentType = formFile.ContentType,
                                    Description = string.Empty,
                                    FileSize = formFile.Length,
                                };

                                await _mediaService.AddMediaToCourseAsync(courseId, mediaDto);
                            }
                            else
                            {
                                ModelState.AddModelError("Images", $"Die Datei {formFile.FileName} ist zu groß oder leer.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Fehler beim Verarbeiten der Datei {FileName}.", formFile.FileName);
                            ModelState.AddModelError("Images", $"Die Datei {formFile.FileName} konnte nicht verarbeitet werden.");
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Der Kurs wurde erfolgreich erstellt.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Kurses.");
                ModelState.AddModelError(string.Empty, "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.");
                return View(viewModel);
            }
        }

        private string SanitizeFileName(string fileName)
        {
            return string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptApplication(int applicationId)
        {
            try
            {

                var application = await _applicationService.GetApplicationByIdAsync(applicationId);
                var course = await _courseService.GetCourseByIdAsync(application.CourseId);
                if (application == null)
                {
                    throw new Exception($"Anmeldung mit ID {applicationId} nicht gefunden.");
                }

                var subject = "Ihre Kursanmeldung wurde akzeptiert";
                var message = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                            <div style='background-color: #ffffff; padding: 20px; border-radius: 5px;'>
                                <h1 style='color: #4CAF50;'>Hallo {application.FirstName} {application.LastName},</h1>
                                <p>Ihre Anmeldung für den Kurs <strong>{course.Title}</strong> wurde <strong>akzeptiert</strong>.</p>
                                <p>Wir freuen uns, Sie im Kurs begrüßen zu dürfen!</p>
                                <p>Mit freundlichen Grüßen,<br/>Ihr Team</p>
                                <hr style='border: none; border-top: 1px solid #ddd;' />
                                <p style='font-size: 12px; color: #888888;'>&copy; {DateTime.Now.Year} Alexandra Hearts. Alle Rechte vorbehalten.</p>
                            </div>
                        </body>
                    </html>";

                await _emailService.SendEmailAsync(application.Email, subject, message);
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

                // Holen der Anwendungsdetails
                var application = await _applicationService.GetApplicationByIdAsync(applicationId);
                var course = await _courseService.GetCourseByIdAsync(application.CourseId);
                if (application == null)
                {
                    throw new Exception($"Anmeldung mit ID {applicationId} nicht gefunden.");
                }

                var subject = "Ihre Kursanmeldung wurde auf die Warteliste gesetzt";
                var message = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                            <div style='background-color: #ffffff; padding: 20px; border-radius: 5px;'>
                                <h1 style='color: #FFA500;'>Hallo {application.FirstName} {application.LastName},</h1>
                                <p>Ihre Anmeldung für den Kurs <strong>{course.Title}</strong> wurde auf die <strong>Warteliste</strong> gesetzt, da der Kurs derzeit ausgebucht ist.</p>
                                <p>Sobald ein Platz frei wird, werden wir Sie umgehend benachrichtigen.</p>
                                <p>Vielen Dank für Ihr Interesse!</p>
                                <p>Mit freundlichen Grüßen,<br/>Ihr Team</p>
                                <hr style='border: none; border-top: 1px solid #ddd;' />
                                <p style='font-size: 12px; color: #888888;'>&copy; {DateTime.Now.Year} Dein Kursname. Alle Rechte vorbehalten.</p>
                            </div>
                        </body>
                    </html>";

                await _emailService.SendEmailAsync(application.Email, subject, message);

                TempData["SuccessMessage"] = "Die Anmeldung wurde erfolgreich auf die Warteliste gesetzt.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Setzen der Anmeldung {ApplicationId} auf die Warteliste.", applicationId);
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Setzen der Anmeldung auf die Warteliste aufgetreten.";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> ManageCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();

            var viewModel = new ManageCoursesViewModel
            {
                Courses = courses
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCourseStatus(int id)
        {
            try
            {
                await _courseService.ToggleCourseStatusAsync(id);
                TempData["SuccessMessage"] = "Der Kursstatus wurde erfolgreich aktualisiert.";
            }
            catch (CourseExceptions.NotFoundException ex)
            {
                _logger.LogError(ex, "Kurs mit ID {CourseId} wurde nicht gefunden.", id);
                TempData["ErrorMessage"] = "Der Kurs wurde nicht gefunden.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Kursstatus für Kurs mit ID {CourseId}.", id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
            }

            return RedirectToAction("ManageCourses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Ungültige Kurs-ID {CourseId} für Löschanfrage.", id);
                TempData["ErrorMessage"] = "Ungültige Kurs-ID.";
                return RedirectToAction("ManageCourses");
            }

            try
            {
                await _courseService.DeleteCourseAsync(id);
                TempData["SuccessMessage"] = "Der Kurs wurde erfolgreich gelöscht.";
                _logger.LogInformation("Kurs mit ID {CourseId} wurde erfolgreich gelöscht.", id);
            }
            catch (CourseExceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, "Kurs mit ID {CourseId} wurde nicht gefunden.", id);
                TempData["ErrorMessage"] = "Der angeforderte Kurs wurde nicht gefunden.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ein Fehler ist beim Löschen des Kurses mit ID {CourseId} aufgetreten.", id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
            }

            return RedirectToAction("ManageCourses");
        }
    }
}
