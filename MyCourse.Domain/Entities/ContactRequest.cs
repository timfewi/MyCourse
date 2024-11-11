using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class ContactRequest : BaseEntity
    {
        [Required(ErrorMessage = "Bitte geben Sie Ihren Namen ein.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse ein.")]
        [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige E-Mail-Adresse ein.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie einen Betreff ein.")]
        [StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bitte geben Sie eine Nachricht ein.")]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public bool IsAnswered { get; set; } = false;

        public DateTime? AnswerDate { get; set; }

        public string AnswerMessage { get; set; } = string.Empty;
    }
}
