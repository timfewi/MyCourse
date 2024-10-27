using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.CourseDtos
{
    public class CourseUpdateDto : CourseCreateDto
    {
        public int Id { get; set; }
    }

}
