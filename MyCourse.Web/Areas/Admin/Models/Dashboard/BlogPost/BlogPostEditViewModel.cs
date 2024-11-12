using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard.BlogPost
{
    public class BlogPostEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Titel ist erforderlich.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Die Beschreibung ist erforderlich.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Veröffentlichen")]
        public bool IsPublished { get; set; }

        [Display(Name = "Tags (durch Komma getrennt)")]
        public string TagsInput { get; set; } = string.Empty;

        public List<ExistingImageViewModel> ExistingImages { get; set; } = new List<ExistingImageViewModel>();

        // Neue Bilder zum Hinzufügen
        [Display(Name = "Neue Bilder hinzufügen")]
        [DataType(DataType.Upload)]
        public List<IFormFile>? NewImages { get; set; } = new List<IFormFile>();
    }
}
