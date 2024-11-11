using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.ContactRequestDtos
{
    public class ContactRequestRespondDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Antwortnachricht ein.")]
        [StringLength(2000)]
        public string AnswerMessage { get; set; } = string.Empty;
    }
}
