using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.CourseDtos
{
    public class CourseImageDto
    {
        public int MediaId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool ToDelete { get; set; }
    }
}
