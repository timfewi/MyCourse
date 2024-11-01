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
        public bool IsActive { get; set; }
        //public string ImageUrl { get; set; } = string.Empty; 
    }

}
