using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Models.HomeModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Bitte geben Sie Ihren Namen ein.")]
        [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } =string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse ein.")]
        [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige E-Mail-Adresse ein.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie einen Betreff ein.")]
        [StringLength(150, ErrorMessage = "Der Betreff darf maximal 150 Zeichen lang sein.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie Ihre Nachricht ein.")]
        [StringLength(2000, ErrorMessage = "Die Nachricht darf maximal 2000 Zeichen lang sein.")]
        public string Message { get; set; } = string.Empty;
    }
}
