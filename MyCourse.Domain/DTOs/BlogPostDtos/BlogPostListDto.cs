using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.BlogPostDtos
{
    public class BlogPostListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string ThumbnailUrl { get; set; } = string.Empty;

    }
}
