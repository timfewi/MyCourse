using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard.BlogPost
{
    public class BlogPostCreateViewModel
    {
        [Required(ErrorMessage = "Der Titel ist erforderlich.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Die Beschreibung ist erforderlich.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Veröffentlichen")]
        public bool IsPublished { get; set; }

        [Display(Name = "Tags (durch Komma getrennt)")]
        public string TagsInput { get; set; } = string.Empty;

        [Display(Name = "Bilder hochladen")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
