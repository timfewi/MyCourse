using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Models.CourseModels
{
    public class CourseRegistrationViewModel
    {
        public int CourseId { get; set; }


        [Required(ErrorMessage = "Der Vorname ist erforderlich.")]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Der Nachname ist erforderlich.")]
        [Display(Name = "Vorname")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Die E-Mail-Adresse ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Ungültiges E-Mail-Format.")]
        [Display(Name = "E-Mail-Adresse")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Die Telefonnummer ist erforderlich.")]
        [Phone(ErrorMessage = "Ungültiges Telefonnummer-Format.")]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte wählen Sie Ihr Erfahrungsniveau aus")]
        [Display(Name = "Erfahrungstufe")]
        public string? ExperienceLevel { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie Ihren bevorzugten Malstil an")]
        [Display(Name = "Lieblingsstil")]
        public string? PreferredStyle { get; set; }


        [Display(Name = "zusätzliche Anmerkungen")]
        public string? Comments { get; set; }
    }
}
