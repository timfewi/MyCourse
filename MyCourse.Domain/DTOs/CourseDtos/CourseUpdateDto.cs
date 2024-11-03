using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

