using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.CourseDtos
{
    public class CourseDetailDto : CourseUpdateDto
    {
        public int ApplicationCount { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();


    }

}
