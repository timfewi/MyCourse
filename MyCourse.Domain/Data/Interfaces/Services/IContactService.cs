using MyCourse.Domain.DTOs.ContactRequestDtos;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Services
{
    public interface IContactService
    {
        Task<IEnumerable<ContactRequestDto>> GetAllContactRequestsAsync();
        Task<IEnumerable<ContactRequestDto>> GetUnansweredContactRequestsAsync();
        Task<IEnumerable<ContactRequestDto>> GetAnsweredContactRequestsAsync();
        Task<IEnumerable<ContactRequestDto>> SearchContactRequestsAsync(string searchTerm);
        Task CreateContactRequestAsync(ContactRequestCreateDto createDto);
        Task RespondToContactRequestAsync(ContactRequestRespondDto respondDto);
        Task<ContactRequestDto> GetContactRequestByIdAsync(int id);
        
    }
}
