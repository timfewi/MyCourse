using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.ContactRequestDtos
{
    public class ContactRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public bool IsAnswered { get; set; }
        public DateTime? AnswerDate { get; set; }
        public string? AnswerMessage { get; set; }
    }
}
