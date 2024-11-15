using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class BlogPostMedia : BaseEntity
    {

        public int Order { get; set; }

        public int BlogPostId { get; set; }
        [ForeignKey(nameof(BlogPostId))]
        public BlogPost BlogPost { get; set; } = null!;

        public int MediaId { get; set; }
        [ForeignKey(nameof(MediaId))]
        public Media Media { get; set; } = null!;
    }
}
