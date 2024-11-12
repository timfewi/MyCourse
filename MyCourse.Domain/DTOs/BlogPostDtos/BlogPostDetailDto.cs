using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.BlogPostDtos
{
    public class BlogPostDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? PublishedDate { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<BlogPostMediaDetailDto> Medias { get; set; } = new List<BlogPostMediaDetailDto>();
    }
}
