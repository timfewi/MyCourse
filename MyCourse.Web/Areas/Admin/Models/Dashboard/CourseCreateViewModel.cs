using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard
{
    public class CourseCreateViewModel
    {
        [Required(ErrorMessage = "Der Titel ist erforderlich.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Die Beschreibung ist erforderlich.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Das Kursdatum ist erforderlich.")]
        [DataType(DataType.Date)]
        public DateTime CourseDate { get; set; }

        [Required(ErrorMessage = "Die Kursdauer in Stunden ist erforderlich.")]
        [Range(0, 23, ErrorMessage = "Stunden müssen zwischen 0 und 23 liegen.")]
        public int CourseDurationHours { get; set; }

        [Required(ErrorMessage = "Die Kursdauer in Minuten ist erforderlich.")]
        [Range(0, 59, ErrorMessage = "Minuten müssen zwischen 0 und 59 liegen.")]
        public int CourseDurationMinutes { get; set; }

        [Required(ErrorMessage = "Die maximale Teilnehmerzahl ist erforderlich.")]
        [Range(1, 1000, ErrorMessage = "Maximale Teilnehmerzahl muss zwischen 1 und 1000 liegen.")]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Der Ort ist erforderlich.")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Der Preis ist erforderlich.")]
        [Range(0, double.MaxValue, ErrorMessage = "Preis muss positiv sein.")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}
