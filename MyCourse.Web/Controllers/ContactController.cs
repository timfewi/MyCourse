using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ContactRequestDtos;
using MyCourse.Domain.Exceptions.ContactRequestEx;
using MyCourse.Web.Models.ContactRequestModels;
namespace MyCourse.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(
            IContactService contactService,
            ILogger<ContactController> logger
            )
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ContactRequestViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contactRequestCreateDto = new ContactRequestCreateDto
            {
                Name = model.Name,
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message
            };

            try
            {
                await _contactService.CreateContactRequestAsync(contactRequestCreateDto);
                return View("Success");
            }
            catch (ContactRequestValidationException ex)
            {
                _logger.LogError(ex, "Validierungsfehler beim Erstellen der Kontaktanfrage.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen der Kontaktanfrage.");
                ModelState.AddModelError(string.Empty, "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.");
                return View(model);
            }
        }
    }
}
