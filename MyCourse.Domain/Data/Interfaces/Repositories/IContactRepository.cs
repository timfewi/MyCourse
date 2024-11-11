using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Repositories
{
    public interface IContactRepository : IRepository<ContactRequest>
    {
        Task<IEnumerable<ContactRequest>> GetUnansweredAsync();
        Task<IEnumerable<ContactRequest>> GetAnsweredAsync();
        Task<IEnumerable<ContactRequest>> SearchAsync(string searchTerm);
        Task MarkAsAnsweredAsync(int id, string answerMessage);
        Task<IEnumerable<ContactRequest>> GetByEmailAsync(string email);
    }
}
