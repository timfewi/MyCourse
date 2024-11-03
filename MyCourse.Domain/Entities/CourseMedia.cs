using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class CourseMedia : BaseEntity
    {
        public int CourseId { get; set; }
        public int MediaId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = default!;
        [ForeignKey(nameof(MediaId))]
        public Media Media { get; set; } = default!;
    }
}
