using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard
{
    public class CourseEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Titel ist erforderlich.")]
        [StringLength(100, ErrorMessage = "Der Titel darf maximal 100 Zeichen lang sein.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Die Beschreibung ist erforderlich.")]
        [StringLength(1000, ErrorMessage = "Die Beschreibung darf maximal 1000 Zeichen lang sein.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Das Kursdatum ist erforderlich.")]
        [DataType(DataType.Date)]
        public DateTime CourseDate { get; set; }

        [Required(ErrorMessage = "Die Kursdauer in Stunden ist erforderlich.")]
        [Range(0, 23, ErrorMessage = "Stunden müssen zwischen 0 und 23 liegen.")]
        public int CourseDurationHours { get; set; }

        [Required(ErrorMessage = "Die Kursdauer in Minuten ist erforderlich.")]
        [Range(0, 59, ErrorMessage = "Minuten müssen zwischen 0 und 59 liegen.")]
        public int CourseDurationMinutes { get; set; }

        [Range(1, 1000, ErrorMessage = "Die maximale Teilnehmerzahl muss zwischen 1 und 1000 liegen.")]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Der Standort ist erforderlich.")]
        [StringLength(200, ErrorMessage = "Der Standort darf maximal 200 Zeichen lang sein.")]
        public string Location { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Der Preis muss eine positive Zahl sein.")]
        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public List<ExistingImageViewModel> ExistingImages { get; set; } = new List<ExistingImageViewModel>();

        // Neue Bilder zum Hinzufügen
        [Display(Name = "Neue Bilder hinzufügen")]
        [DataType(DataType.Upload)]
        public List<IFormFile>? NewImages { get; set; } = new List<IFormFile>();
    }
}
