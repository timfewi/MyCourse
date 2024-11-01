using MyCourse.Domain.DTOs.CourseDtos;

namespace MyCourse.Web.Models.HomeModels
{
    public class HomeViewModel
    {
        public IEnumerable<CourseListDto>? ActiveCourses { get; set; } 
        public IEnumerable<Feature>? Features { get; set; }
    }

    public class Feature
    {
        public string? IconClass { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
