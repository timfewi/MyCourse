using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.ApplicationDtos
{
    public class ApplicationDetailDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
        public ApplicationStatusType Status { get; set; }
        public string StatusDisplayName { get; set; } = default!;
        public string? ExperienceLevel { get; set; }
        public string? PreferredStyle { get; set; }
        public string? Comments { get; set; }
    }

}
