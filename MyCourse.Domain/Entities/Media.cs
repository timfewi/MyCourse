using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class Media : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long FileSize { get; set; }

        // Navigation Properties
        public ICollection<CourseMedia> CourseMedias { get; set; } = [];
        public ICollection<ApplicationMedia> ApplicationMedias { get; set; } = [];
    }
}