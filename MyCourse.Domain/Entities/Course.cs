using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CourseDate { get; set; }
        public TimeSpan CourseDuration { get; set; }
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Navigation Properties
        public ICollection<Application> Applications { get; set; } = [];
        public ICollection<CourseMedia> CourseMedias { get; set; } = [];
    }
}
