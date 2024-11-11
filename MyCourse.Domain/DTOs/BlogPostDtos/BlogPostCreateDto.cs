using Microsoft.AspNetCore.Http;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.BlogPostDtos
{
    public class BlogPostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = [];

        public List<BlogPostMediaCreateDto> Medias { get; set; } = [];
    }
}
