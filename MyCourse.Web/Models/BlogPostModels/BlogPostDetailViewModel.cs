namespace MyCourse.Web.Models.BlogPostModels
{
    public class BlogPostDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // HTML-Inhalt
        public bool IsPublished { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? PublishedDate { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string ThumbnailUrl { get; set; } = string.Empty;
        public List<BlogPostMediaDetailViewModel> Medias { get; set; } = new List<BlogPostMediaDetailViewModel>();
    }

    public class BlogPostMediaDetailViewModel
    {
        public string Url { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }
}
