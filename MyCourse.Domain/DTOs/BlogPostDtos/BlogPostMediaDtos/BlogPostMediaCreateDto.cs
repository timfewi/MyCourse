using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos
{
    public class BlogPostMediaCreateDto
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Caption { get; set; }
    }
}
