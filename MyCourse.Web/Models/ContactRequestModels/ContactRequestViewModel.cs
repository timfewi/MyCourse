using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Models.ContactRequestModels
{
    public class ContactRequestViewModel
    {
        [Required(ErrorMessage = "Bitte geben Sie Ihren Namen ein.")]
        [Display(Name = "Ihr Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse ein.")]
        [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige E-Mail-Adresse ein.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie einen Betreff ein.")]
        [Display(Name = "Betreff")]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Ihre Nachricht")]
        [Required(ErrorMessage = "Bitte geben Sie eine Nachricht ein.")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; } = string.Empty;
    }
}
