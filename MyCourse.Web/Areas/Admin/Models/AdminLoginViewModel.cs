using System.ComponentModel.DataAnnotations;

namespace MyCourse.Web.Areas.Admin.Models
{
    public class AdminLoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Angemeldet bleiben?")]
        public bool RememberMe { get; set; } 
    }
}
