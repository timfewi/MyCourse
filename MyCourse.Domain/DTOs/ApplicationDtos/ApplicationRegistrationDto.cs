using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.ApplicationDtos
{
    public class ApplicationRegistrationDto
    {
        [Required]
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [PersonalData]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [PersonalData]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [PersonalData]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
