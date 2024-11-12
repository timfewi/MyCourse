namespace MyCourse.Web.Models.HomeModels
{
    public class BlogPostHomeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
