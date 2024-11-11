using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        // Navigation Properties
        public ICollection<BlogPostMedia> BlogPostMedias { get; set; } = new List<BlogPostMedia>();


    }
}
