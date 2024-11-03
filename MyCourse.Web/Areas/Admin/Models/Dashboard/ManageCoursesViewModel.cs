using MyCourse.Domain.DTOs.CourseDtos;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard
{
    public class ManageCoursesViewModel
    {
        public IEnumerable<CourseListDto> Courses { get; set; } = new List<CourseListDto>();
    }
}
