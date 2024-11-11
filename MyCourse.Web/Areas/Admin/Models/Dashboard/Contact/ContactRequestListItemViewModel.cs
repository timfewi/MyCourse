using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard.Contact
{
    public class ContactRequestListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Betreff")]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Datum der Anfrage")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Beantwortet?")]
        public bool IsAnswered { get; set; }
    }

}
