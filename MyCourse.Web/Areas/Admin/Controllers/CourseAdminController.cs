using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.Exceptions.CourseExceptions.CourseEx;

namespace MyCourse.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/courses")]
    public class CourseAdminController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseAdminController> _logger;
        private readonly IMapper _mapper;

        public CourseAdminController(ICourseService courseService, ILogger<CourseAdminController> logger, IMapper mapper)
        {
            _courseService = courseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CourseCreateDto courseDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.CreateCourseAsync(courseDto);
                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                catch (CourseException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ein unerwarteter Fehler ist beim Erstellen des Kurses aufgetreten.");
                    ModelState.AddModelError("", "Ein unerwarteter Fehler ist aufgetreten.");
                }
            }
            return View(courseDto);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                var courseUpdateDto = _mapper.Map<CourseUpdateDto>(course);
                return View(courseUpdateDto);
            }
            catch (CourseException ex)
            {
                _logger.LogWarning(ex, "Kurs mit ID {CourseId} nicht gefunden.", id);
                return NotFound(ex.Message);
            }
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, CourseUpdateDto courseDto)
        {
            if (id != courseDto.Id)
            {
                ModelState.AddModelError("", "Die Kurs-ID stimmt nicht überein.");
                return View(courseDto);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.UpdateCourseAsync(courseDto);
                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                catch (CourseException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ein unerwarteter Fehler ist beim Aktualisieren des Kurses aufgetreten.");
                    ModelState.AddModelError("", "Ein unerwarteter Fehler ist aufgetreten.");
                }
            }

            return View(courseDto);
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                return RedirectToAction("Index");
            }
            catch (CourseException ex)
            {
                _logger.LogWarning(ex, "Kurs mit ID {CourseId} nicht gefunden.", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ein unerwarteter Fehler ist beim Löschen des Kurses aufgetreten.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ein unerwarteter Fehler ist aufgetreten.");
            }
        }
    }
}
