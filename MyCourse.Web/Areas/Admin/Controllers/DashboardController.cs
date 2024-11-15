using Azure.Core;
using FluentValidation;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using MyCourse.Domain.DTOs.ContactRequestDtos;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Domain.Exceptions.MediaEx;
using MyCourse.Web.Areas.Admin.Models.Dashboard;
using MyCourse.Web.Areas.Admin.Models.Dashboard.BlogPost;
using MyCourse.Web.Areas.Admin.Models.Dashboard.Contact;
using System.Linq;
using System.Net;
using System.Net.Mail;
using static MyCourse.Domain.Exceptions.BlogPostEx.BlogPostExceptions;
using static MyCourse.Domain.Exceptions.CourseEx.CourseExceptions;

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
        private readonly IContactService _contactService;
        private readonly IBlogPostService _blogPostService;
        private readonly IEmailService _emailService;
        public DashboardController(
            ILogger<DashboardController> logger,
            IWebHostEnvironment hostingEnvironment,
            ICourseService courseService,
            IApplicationService applicationService,
            IMediaService mediaService,
            IContactService contactService,
            IBlogPostService blogPostService,
            IEmailService emailService)
        {

            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _courseService = courseService;
            _applicationService = applicationService;
            _mediaService = mediaService;
            _contactService = contactService;
            _blogPostService = blogPostService;
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
                        catch (MediaException ex)
                        {
                            _logger.LogError(ex, "Fehler beim Verarbeiten der Datei {FileName}.", formFile.FileName);
                            ModelState.AddModelError("Images", ex.Message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unerwarteter Fehler beim Verarbeiten der Datei {FileName}.", formFile.FileName);
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptApplication(int applicationId)
        {
            try
            {
                // Schritt 1: Abrufen der Anwendung
                var application = await _applicationService.GetApplicationByIdAsync(applicationId);
                if (application == null)
                {
                    throw new Exception($"Anmeldung mit ID {applicationId} nicht gefunden.");
                }

                // Abrufen des zugehörigen Kurses
                var course = await _courseService.GetCourseByIdAsync(application.CourseId);
                if (course == null)
                {
                    throw new Exception($"Kurs mit ID {application.CourseId} nicht gefunden.");
                }

                // Schritt 2: Akzeptieren der Anmeldung
                await _applicationService.AcceptApplicationAsync(applicationId);

                // Schritt 3: Senden der Bestätigungs-E-Mail
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

                // Schritt 4: Erfolgsmeldung
                TempData["SuccessMessage"] = "Die Anmeldung wurde erfolgreich akzeptiert.";
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
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
                // Schritt 1: Abrufen der Anwendung
                var application = await _applicationService.GetApplicationByIdAsync(applicationId);
                if (application == null)
                {
                    throw new Exception($"Anmeldung mit ID {applicationId} nicht gefunden.");
                }

                // Abrufen des zugehörigen Kurses
                var course = await _courseService.GetCourseByIdAsync(application.CourseId);
                if (course == null)
                {
                    throw new Exception($"Kurs mit ID {application.CourseId} nicht gefunden.");
                }

                // Schritt 2: Setzen der Anmeldung auf die Warteliste
                await _applicationService.SetApplicationToWaitingListAsync(applicationId);

                // Schritt 3: Senden der Benachrichtigungs-E-Mail
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

                // Schritt 4: Erfolgsmeldung
                TempData["SuccessMessage"] = "Die Anmeldung wurde erfolgreich auf die Warteliste gesetzt.";
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
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
            catch (CourseNotFoundException ex)
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
            catch (CourseNotFoundException ex)
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

        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Ungültige Kurs-ID {CourseId} für Bearbeitungsanfrage.", id);
                TempData["ErrorMessage"] = "Ungültige Kurs-ID.";
                return RedirectToAction("ManageCourses");
            }

            try
            {

                var courseDto = await _courseService.GetCourseEditDetailsWithImagesAsync(id);
                if (courseDto == null)
                {
                    TempData["ErrorMessage"] = "Der angeforderte Kurs wurde nicht gefunden.";
                    return RedirectToAction("ManageCourses");
                }

                var viewModel = new CourseEditViewModel
                {
                    Id = courseDto.CourseId,
                    Title = courseDto.Title,
                    Description = courseDto.Description,
                    CourseDate = courseDto.CourseDate,
                    CourseDurationHours = courseDto.CourseDuration.Hours,
                    CourseDurationMinutes = courseDto.CourseDuration.Minutes,
                    MaxParticipants = courseDto.MaxParticipants,
                    Location = courseDto.Location,
                    Price = courseDto.Price,
                    IsActive = courseDto.IsActive,
                    ExistingImages = courseDto.ExistingImages.Select(ei => new ExistingImageViewModel
                    {
                        MediaId = ei.MediaId,
                        Url = ei.Url,
                        ToDelete = false
                    }).ToList(),
                    NewImages = new List<IFormFile>()
                };

                return View(viewModel);
            }
            catch (CourseNotFoundException ex)
            {
                _logger.LogError(ex, "Kurs mit ID {CourseId} wurde nicht gefunden.", id);
                return RedirectToAction("ManageCourses");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading course details for EditCourse.");
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
                return RedirectToAction("ManageCourses");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(CourseEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var courseEditDto = new CourseEditWithImagesDto
            {
                CourseId = viewModel.Id,
                Title = viewModel.Title,
                Description = viewModel.Description,
                CourseDate = viewModel.CourseDate,
                CourseDuration = new TimeSpan(viewModel.CourseDurationHours, viewModel.CourseDurationMinutes, 0),
                MaxParticipants = viewModel.MaxParticipants,
                Location = viewModel.Location,
                Price = viewModel.Price,
                IsActive = viewModel.IsActive,
                ExistingImages = viewModel.ExistingImages.Select(ei => new CourseImageDto
                {
                    MediaId = ei.MediaId,
                    Url = ei.Url,
                    ToDelete = ei.ToDelete
                }).ToList(),
                NewImages = viewModel.NewImages!
            };

            try
            {
                await _courseService.UpdateCourseWithImagesAsync(courseEditDto);

                TempData["SuccessMessage"] = "Der Kurs wurde erfolgreich aktualisiert.";
                _logger.LogInformation("Course with ID {CourseId} updated successfully.", viewModel.Id);
            }
            catch (CourseNotFoundException)
            {
                _logger.LogWarning("Course with ID {CourseId} not found during update.", viewModel.Id);
                TempData["ErrorMessage"] = "Der angeforderte Kurs wurde nicht gefunden.";
                return RedirectToAction("ManageCourses");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating course with ID {CourseId}.", viewModel.Id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
                return RedirectToAction("ManageCourses");
            }

            return RedirectToAction("ManageCourses");
        }

        // GET: Admin/Dashboard/ContactRequests
        [HttpGet]
        public async Task<IActionResult> ContactRequests(int? selectedRequestId)
        {
            try
            {
                var activeRequests = await _contactService.GetUnansweredContactRequestsAsync();
                var inactiveRequests = await _contactService.GetAnsweredContactRequestsAsync();

                var activeListItems = activeRequests.Select(r => new ContactRequestListItemViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Email = r.Email,
                    Subject = r.Subject,
                    RequestDate = r.DateCreated,
                    IsAnswered = r.IsAnswered,
                }).ToList();

                var inactiveListItems = inactiveRequests.Select(r => new ContactRequestListItemViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Email = r.Email,
                    Subject = r.Subject,
                    RequestDate = r.DateCreated,
                    IsAnswered = r.IsAnswered,
                }).ToList();

                ContactRequestDetailViewModel? selectedRequestViewModel = null;

                if (selectedRequestId.HasValue)
                {
                    var selectedRequest = await _contactService.GetContactRequestByIdAsync(selectedRequestId.Value);
                    if (selectedRequest != null)
                    {
                        selectedRequestViewModel = new ContactRequestDetailViewModel
                        {
                            Id = selectedRequest.Id,
                            Name = selectedRequest.Name,
                            Email = selectedRequest.Email,
                            Subject = selectedRequest.Subject,
                            Message = selectedRequest.Message,
                            RequestDate = selectedRequest.DateCreated,
                            AnswerMessage = selectedRequest.AnswerMessage ?? string.Empty,
                        };
                    }
                }

                var viewModel = new ContactRequestViewModel
                {
                    ActiveContactRequests = activeListItems,
                    InactiveContactRequests = inactiveListItems,
                    SelectedRequest = selectedRequestViewModel,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Kontaktanfragen.");
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Abrufen der Kontaktanfragen aufgetreten.";
                return View(new ContactRequestViewModel());
            }
        }

        // GET: Admin/Dashboard/GetContactRequestDetail
        [HttpGet]
        public async Task<IActionResult> GetContactRequestDetail(int id)
        {
            try
            {
                var selectedRequest = await _contactService.GetContactRequestByIdAsync(id);
                if (selectedRequest == null)
                {
                    return NotFound();
                }

                var selectedRequestViewModel = new ContactRequestDetailViewModel
                {
                    Id = selectedRequest.Id,
                    Name = selectedRequest.Name,
                    Email = selectedRequest.Email,
                    Subject = selectedRequest.Subject,
                    Message = selectedRequest.Message,
                    RequestDate = selectedRequest.DateCreated,
                    AnswerMessage = selectedRequest.AnswerMessage ?? string.Empty,
                    IsAnswered = selectedRequest.IsAnswered
                };

                return PartialView("_ContactRequestDetailPartial", selectedRequestViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Kontaktanfrage mit ID {Id}.", id);
                return StatusCode(500, "Interner Serverfehler");
            }
        }


        // POST: Admin/Dashboard/RespondToContactRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondToContactRequest(ContactRequestDetailViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    // Aktive (unbeantwortete) Kontaktanfragen abrufen
                    var activeRequests = await _contactService.GetUnansweredContactRequestsAsync();

                    // Inaktive (beantwortete) Kontaktanfragen abrufen
                    var inactiveRequests = await _contactService.GetAnsweredContactRequestsAsync();

                    var activeListItems = activeRequests.Select(r => new ContactRequestListItemViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Email = r.Email,
                        Subject = r.Subject,
                        RequestDate = r.DateCreated,
                        IsAnswered = r.IsAnswered,
                    }).ToList();

                    var inactiveListItems = inactiveRequests.Select(r => new ContactRequestListItemViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Email = r.Email,
                        Subject = r.Subject,
                        RequestDate = r.DateCreated,
                        IsAnswered = r.IsAnswered,
                    }).ToList();

                    var combinedViewModel = new ContactRequestViewModel
                    {
                        ActiveContactRequests = activeListItems,
                        InactiveContactRequests = inactiveListItems,
                        SelectedRequest = viewModel,
                    };

                    return View("ContactRequests", combinedViewModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler beim Laden der Kontaktanfragen nach ungültigem ModelState.");
                    TempData["ErrorMessage"] = "Es ist ein Fehler beim Laden der Kontaktanfragen aufgetreten.";

                    return View("ContactRequests", new ContactRequestViewModel());
                }
            }

            try
            {
                var respondDto = new ContactRequestRespondDto
                {
                    Id = viewModel.Id,
                    AnswerMessage = viewModel.AnswerMessage,
                };

                await _contactService.RespondToContactRequestAsync(respondDto);

                TempData["SuccessMessage"] = "Die Antwort wurde erfolgreich gesendet.";
                return RedirectToAction(nameof(ContactRequests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Beantworten der Kontaktanfrage mit ID {Id}.", viewModel.Id);
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Beantworten der Kontaktanfrage aufgetreten.";

                try
                {
                    // Aktive (unbeantwortete) Kontaktanfragen abrufen
                    var activeRequests = await _contactService.GetUnansweredContactRequestsAsync();

                    // Inaktive (beantwortete) Kontaktanfragen abrufen
                    var inactiveRequests = await _contactService.GetAnsweredContactRequestsAsync();

                    var activeListItems = activeRequests.Select(r => new ContactRequestListItemViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Email = r.Email,
                        Subject = r.Subject,
                        RequestDate = r.DateCreated,
                        IsAnswered = r.IsAnswered,
                    }).ToList();

                    var inactiveListItems = inactiveRequests.Select(r => new ContactRequestListItemViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Email = r.Email,
                        Subject = r.Subject,
                        RequestDate = r.DateCreated,
                        IsAnswered = r.IsAnswered,
                    }).ToList();

                    var combinedViewModel = new ContactRequestViewModel
                    {
                        ActiveContactRequests = activeListItems,
                        InactiveContactRequests = inactiveListItems,
                        SelectedRequest = viewModel
                    };

                    return View("ContactRequests", combinedViewModel);
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Fehler beim Laden der Kontaktanfragen nach einem Antwortfehler.");
                    TempData["ErrorMessage"] = "Es ist ein schwerwiegender Fehler beim Beantworten der Kontaktanfrage aufgetreten.";

                    return View("ContactRequests", new ContactRequestViewModel());
                }
            }
        }


        #region - BlogPost -

        // GET: Admin/Dashboard/CreateBlogPost
        [HttpGet]
        public IActionResult CreateBlogPost()
        {
            return View();
        }

        // POST: Admin/Dashboard/CreateBlogPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlogPost(BlogPostCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var createDto = new BlogPostCreateDto
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                IsPublished = viewModel.IsPublished,
                Tags = string.IsNullOrWhiteSpace(viewModel.TagsInput) ? new List<string>() :
                    viewModel.TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(tag => tag.Trim()).ToList(),
                Medias = new List<BlogPostMediaCreateDto>()
            };

            if (viewModel.Images != null && viewModel.Images.Count > 0)
            {
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var maxFileSize = 10 * 1024 * 1024;

                foreach (var formFile in viewModel.Images)
                {
                    try
                    {
                        if (formFile.Length > 0 && formFile.Length <= maxFileSize)
                        {
                            var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();

                            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("Images", $"Die Datei {formFile.FileName} hat ein ungültiges Datei-Format.");
                                continue;
                            }

                            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                            fileName = SanitizeFileName(fileName);

                            var newFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "images");

                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            var filePath = Path.Combine(uploadsFolder, newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }

                            var mediaDto = new BlogPostMediaCreateDto
                            {
                                Url = $"/uploads/images/{newFileName}",
                                FileName = newFileName,
                                MediaType = Domain.Enums.MediaType.Image,
                                ContentType = formFile.ContentType,
                                Description = string.Empty,
                                FileSize = formFile.Length,
                            };

                            createDto.Medias.Add(mediaDto);
                        }
                        else
                        {
                            ModelState.AddModelError("Images", $"Die Datei {formFile.FileName} ist zu groß oder leer.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fehler beim Verarbeiten der Datei {FileName}.", formFile.FileName);
                        ModelState.AddModelError("Images", ex.Message);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var blogPost = await _blogPostService.CreateBlogPostAsync(createDto);
                TempData["SuccessMessage"] = "Der BlogPost wurde erfolgreich erstellt.";
                return RedirectToAction(nameof(ManageBlogPosts));
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
                _logger.LogError(ex, "Fehler beim Erstellen des BlogPosts.");
                ModelState.AddModelError(string.Empty, "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.");
                return View(viewModel);
            }

        }

        // GET: Admin/Dashboard/EditBlogPost/{id}
        [HttpGet]
        public async Task<IActionResult> EditBlogPost(int id)
        {
            try
            {
                var blogPost = await _blogPostService.GetBlogPostEditDetailsWithImagesAsync(id);
                if (blogPost == null)
                {
                    TempData["ErrorMessage"] = "Der BlogPost wurd nicht gefunden.";
                    return RedirectToAction(nameof(ManageBlogPosts));
                }

                var viewModel = new BlogPostEditViewModel
                {
                    Id = blogPost.BlogPostId,
                    Title = blogPost.Title,
                    Description = blogPost.Description,
                    IsPublished = blogPost.IsPublished,
                    ExistingImages = blogPost.ExistingImages.Select(m => new ExistingImageViewModel
                    {
                        MediaId = m.MediaId,
                        Url = m.Url,
                        ToDelete = false
                    }).ToList(),
                    NewImages = new List<IFormFile>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des BlogPosts mit ID {id}.", id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
                return RedirectToAction(nameof(ManageBlogPosts));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlogPost(BlogPostEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var blogPostEditDto = new BlogPostEditWithImagesDto
            {
                BlogPostId = viewModel.Id,
                Title = viewModel.Title,
                Description = viewModel.Description,
                IsPublished = viewModel.IsPublished,
                Tags = string.IsNullOrWhiteSpace(viewModel.TagsInput)
                    ? new List<string>()
                    : viewModel.TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(tag => tag.Trim()).ToList(),
                ExistingImages = viewModel.ExistingImages.Select(ei => new BlogPostImageDto
                {
                    MediaId = ei.MediaId,
                    Url = ei.Url,
                    ToDelete = ei.ToDelete
                }).ToList(),
                NewImages = viewModel.NewImages!
            };

            try
            {
                await _blogPostService.UpdateBlogPostWithImagesAsync(blogPostEditDto);

                TempData["SuccessMessage"] = "Der BlogPost wurde erfolgreich aktualisiert.";
                _logger.LogInformation("BlogPost mit ID {BlogPostId} wurde erfolgreich aktualisiert.", viewModel.Id);
            }
            catch (BlogPostNotFoundException)
            {
                _logger.LogWarning("BlogPost mit ID {BlogPostId} wurde nicht gefunden.", viewModel.Id);
                TempData["ErrorMessage"] = "Der angeforderte BlogPost wurde nicht gefunden.";
                return RedirectToAction(nameof(ManageBlogPosts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des BlogPosts mit ID {BlogPostId}.", viewModel.Id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
                return RedirectToAction(nameof(ManageBlogPosts));
            }

            return RedirectToAction(nameof(ManageBlogPosts));
        }


        // GET: Admin/Dashboard/ManageBlogPosts
        [HttpGet]
        public async Task<IActionResult> ManageBlogPosts()
        {
            try
            {
                var blogPosts = await _blogPostService.GetAllBlogPostsAsync();
                var viewModel = new BlogPostManageViewModel
                {
                    BlogPosts = blogPosts
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der BlogPosts.");
                TempData["ErrorMessage"] = "Es ist ein Fehler beim Laden der BlogPosts aufgetreten.";
                return View(new BlogPostManageViewModel());
            }
        }

        // POST: Admin/Dashboard/DeleteBlogPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            try
            {
                await _blogPostService.DeleteBlogPostAsync(id);
                TempData["SuccessMessage"] = "Der BlogPost wurde erfolgreich gelöscht.";
            }
            catch (BlogPostNotFoundException ex)
            {
                _logger.LogError(ex, "BlogPost mit ID {BlogPostId} wurde nicht gefunden.", id);
                TempData["ErrorMessage"] = "Der BlogPost wurde nicht gefunden.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des BlogPosts mit ID {BlogPostId}.", id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
            }

            return RedirectToAction(nameof(ManageBlogPosts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlogPostStatus(int id)
        {
            try
            {
                await _blogPostService.ToggleBlogPostStatusAsync(id);
                TempData["SuccessMessage"] = "Der Blog wurde erfolgreich aktualisiert.";
            }
            catch (BlogPostNotFoundException ex)
            {
                _logger.LogError(ex, "Blog mit ID {BlogPostId} wurde nicht gefunden.", id);
                TempData["ErrorMessage"] = "Der Blog wurde nicht gefunden.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Blogstatus für Blog mit ID {BlogPostId}.", id);
                TempData["ErrorMessage"] = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.";
            }

            return RedirectToAction("ManageBlogPosts");
        }


        #endregion

        // Helper functions
        private string SanitizeFileName(string fileName)
        {
            return string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        }

        private string GetContentTypeByExtension(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                default:
                    return "application/octet-stream";
            }
        }

    }
}
