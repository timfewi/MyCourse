using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Web.Models.CourseModels;

namespace MyCourse.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseService _courseService;
        private readonly IApplicationService _applicationService;

        public CourseController(
            ILogger<CourseController> logger,
            ICourseService courseService,
            IApplicationService applicationService
            )
        {
            _logger = logger;
            _courseService = courseService;
            _applicationService = applicationService;
        }

        // GET CourseDetails
        [HttpGet]
        public async Task<IActionResult> CourseDetailsPartial(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var model = new CourseDetailsViewModel
            {
                Id = id,
                ImageUrls = course.ImageUrls,
                Title = course.Title,
                Description = course.Description,
                CourseDate = course.CourseDate,
                CourseDuration = course.CourseDuration,
                Location = course.Location,
                Price = course.Price,
                MaxParticipants = course.MaxParticipants,
                ApplicationCount = course.ApplicationCount,
            };


            return PartialView("_CourseDetailsPartial", model);
        }

        // GET: Course/Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var model = new CourseDetailsViewModel
            {
                Id = id,
                ImageUrls = course.ImageUrls,
                Title = course.Title,
                Description = course.Description,
                CourseDate = course.CourseDate,
                CourseDuration = course.CourseDuration,
                Location = course.Location,
                Price = course.Price,
                MaxParticipants = course.MaxParticipants,
                ApplicationCount = course.ApplicationCount,
            };


            return View(model);
        }

        // GET: Course/Register/5
        [HttpGet]
        public IActionResult Register(int id)
        {
            var course = _courseService.GetCourseByIdAsync(id).Result;
            if (course == null)
            {
                return NotFound();
            }

            var model = new CourseRegistrationViewModel
            {
                CourseId = id,
                // Initialisiere weitere Eigenschaften nach Bedarf
            };

            return View(model);
        }

        // POST: Course/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CourseRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _applicationService.RegisterUserForCourseAsync(model.CourseId, new ApplicationRegistrationDto
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email.Trim().ToLower(),
                        PhoneNumber = model.PhoneNumber,
                        ExperienceLevel = model.ExperienceLevel,
                        Comments = model.Comments,
                        PreferredStyle = model.PreferredStyle,

                    });
                    return RedirectToAction("Confirmation");
                }
                catch (ValidationException ex)
                {
                    // Validierungsfehler aus dem Service
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                catch (MyCourse.Domain.Exceptions.ApplicationEx.ApplicationException ex)
                {
                    // Spezifische Fehlermeldung für doppelte Anmeldung
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    // Allgemeine Fehlerbehandlung
                    _logger.LogError(ex, "Fehler bei der Registrierung für Kurs {CourseId}", model.CourseId);
                    ModelState.AddModelError(string.Empty, "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.");
                }

            }
            return View(model);
        }

        // GET: Course/Confirmation
        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterPartial(int id)
        {
            var viewModel = new CourseRegistrationViewModel
            {
                CourseId = id
            };
            return PartialView("_CourseRegisterPartial", viewModel);
        }

        // POST-Aktion für die Kursregistrierung
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPartial(CourseRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _applicationService.RegisterUserForCourseAsync(model.CourseId, new ApplicationRegistrationDto
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        ExperienceLevel = model.ExperienceLevel,
                        Comments = model.Comments,
                        PreferredStyle = model.PreferredStyle,

                    });
                    return Json(new { success = true, message = "Erfolg. Ihre Anmeldung wird in kürze überprüft und Sie erhalten eine Email." });
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Fehler bei der Kursregistrierung.");
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return PartialView("_CourseRegisterPartial", model);
        }
    }
}
