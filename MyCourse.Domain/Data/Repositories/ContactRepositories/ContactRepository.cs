using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Repositories.ContactRepositories
{
    [Injectable(ServiceLifetime.Scoped)]
    public class ContactRepository(AppDbContext dbContext) : Repository<ContactRequest>(dbContext), IContactRepository
    {
        public async Task<IEnumerable<ContactRequest>> GetUnansweredAsync()
        {
            return await _dbSet
                .Where(cr => !cr.IsAnswered)
                .OrderBy(cr => cr.AnswerDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactRequest>> GetAnsweredAsync()
        {
            return await _dbSet
                .Where(cr => cr.IsAnswered)
                .OrderBy(cr => cr.AnswerDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactRequest>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }
            searchTerm = searchTerm.ToLower();

            searchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(cr => cr.Name.ToLower().Contains(searchTerm) ||
                                cr.Email.ToLower().Contains(searchTerm) ||
                                cr.Subject.ToLower().Contains(searchTerm) ||
                                cr.Message.ToLower().Contains(searchTerm))
                .OrderByDescending(cr => cr.DateCreated)
                .ToListAsync();
        }

        public async Task MarkAsAnsweredAsync(int id, string answerMessage)
        {
            var contactRequest = await GetByIdAsync(id);
            if (contactRequest != null && !contactRequest.IsAnswered)
            {
                contactRequest.IsAnswered = true;
                contactRequest.AnswerMessage = answerMessage;
                contactRequest.AnswerDate = DateTime.Now;

                _dbSet.Update(contactRequest);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ContactRequest>> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(cr => cr.Email.ToLower().Contains(email))
                .OrderByDescending(cr => cr.DateCreated)
                .ToListAsync();
        }

    }
}
