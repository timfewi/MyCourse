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

namespace MyCourse.Domain.Data.Repositories.ApplicationRepositories
{
    [Injectable(ServiceLifetime.Scoped)]
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        public ApplicationRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<Application>> GetApplicationsByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Application?> GetApplicationByIdAsync(int applicationId)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public void UpdateApplication(Application application)
        {
            _dbSet.Update(application);
        }

        public void DeleteApplication(Application application)
        {
            _dbSet.Remove(application);
        }
        public async Task<Application?> GetByCourseAndEmailAsync(int courseId, string email)
        {
            string normalizedEmail = email.Trim().ToLower();
            return await _dbSet.FirstOrDefaultAsync(a => a.CourseId == courseId && a.Email == normalizedEmail);
        }

    }

}
