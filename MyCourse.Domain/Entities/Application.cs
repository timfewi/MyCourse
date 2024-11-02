using Microsoft.AspNetCore.Identity;
using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class Application : BaseEntity
    {
        public int CourseId { get; set; }

        [PersonalData]
        public string FirstName { get; set; } = string.Empty;
        [PersonalData]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        [PersonalData]
        public string Email { get; set; } = string.Empty;
        [Phone]
        [PersonalData]
        public string PhoneNumber { get; set; } = string.Empty.PadLeft(10, '0');
        public DateTime ApplicationDate { get; set; } = DateTime.Now;
        public ApplicationStatusType Status { get; set; }

        [PersonalData]
        public string? ExperienceLevel { get; set; }
        [PersonalData]
        public string? PreferredStyle { get; set; }
        [PersonalData]
        public string? Comments { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = default!;
        public ICollection<ApplicationMedia> ApplicationMedias { get; set; } = [];
    }
}


