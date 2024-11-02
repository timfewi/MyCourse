using System.ComponentModel.DataAnnotations;

namespace MyCourse.Domain.Enums
{
    public enum ApplicationStatusType
    {
        [Display(Name = "Ausstehend")]
        Pending,       // Bewerbung eingegangen, aber noch nicht bearbeitet

        [Display(Name = "Warteliste")]
        Waiting,       // Benutzer steht auf der Warteliste

        [Display(Name = "Bestätigt")]
        Approved,      // Benutzer wurde fest als Teilnehmer bestätigt

        [Display(Name = "Abgelehnt")]
        Rejected,      // Bewerbung abgelehnt

    }
}
