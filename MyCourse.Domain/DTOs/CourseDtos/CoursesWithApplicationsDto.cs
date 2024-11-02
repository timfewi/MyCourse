using MyCourse.Domain.DTOs.ApplicationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.CourseDtos
{
    public class CourseWithApplicationsDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseDate { get; set; } = default!;
        public List<ApplicationDetailDto> Applications { get; set; } = new();
    }
}
