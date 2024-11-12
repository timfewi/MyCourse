using MyCourse.Domain.DTOs.CourseDtos;

namespace MyCourse.Web.Models.HomeModels
{
    public class HomeViewModel
    {
        public List<CourseHomeViewModel> ActiveCourses { get; set; } = new List<CourseHomeViewModel>();
        public IEnumerable<Feature>? Features { get; set; }
        public List<BlogPostHomeViewModel> Blogs { get; set; } = new List<BlogPostHomeViewModel>();
        public int TotalPublishedBlogPosts { get; set; }
    }

    public class Feature
    {
        public string? IconClass { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
