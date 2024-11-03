using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.CourseDtos
{
    public class CourseListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CourseDate { get; set; }
        public TimeSpan CourseDuration { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string DefaultImageUrl { get; set; } = "/images/placeholder.png";
        public string HoverImageUrl { get; set; } = "/images/placeholder.png";
    }

}
