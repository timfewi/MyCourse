using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.CourseDtos
{
    public class CourseEditWithImagesDto
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CourseDate { get; set; }

        [Required]
        public TimeSpan CourseDuration { get; set; }

        [Range(1, 1000)]
        public int MaxParticipants { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public List<CourseImageDto> ExistingImages { get; set; } = new List<CourseImageDto>();

        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
    }
}
