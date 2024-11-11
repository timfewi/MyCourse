using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Models.HomeModels
{
    public class CourseHomeViewModel
    {
        [Display(Name = "Kurs-ID")]
        public int Id { get; set; }

        [Display(Name = "Titel")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Beschreibung")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Kursdatum und Uhrzeit")]
        public DateTime CourseDate { get; set; }

        [Display(Name = "Kursdauer")]
        public TimeSpan CourseDuration { get; set; }

        [Display(Name = "Zeitspanne")]
        public string CourseTimeSpan
        {
            get
            {
                var endTime = CourseDate.Add(CourseDuration);
                return $"{CourseDate:HH:mm} - {endTime:HH:mm}";
            }
        }

        [Display(Name = "Standort")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Preis (€)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Aktiv")]
        public bool IsActive { get; set; }

        [Display(Name = "Standardbild")]
        public string DefaultImageUrl { get; set; } = "/images/placeholder.png";

        [Display(Name = "Hover-Bild")]
        public string HoverImageUrl { get; set; } = "/images/placeholder.png";

        // Zusätzliche Eigenschaften für die Startseite
        [Display(Name = "Anzahl Teilnehmer")]
        public int MaxParticipants { get; set; }

        [Display(Name = "Lehrer")]
        public string Instructor { get; set; } = string.Empty;
    }
}

