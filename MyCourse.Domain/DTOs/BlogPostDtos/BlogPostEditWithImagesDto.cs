using Microsoft.AspNetCore.Http;
using MyCourse.Domain.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.BlogPostDtos
{
    public class BlogPostEditWithImagesDto
    {
        public int BlogPostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<BlogPostImageDto> ExistingImages { get; set; } = new List<BlogPostImageDto>();

        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
    }
}
