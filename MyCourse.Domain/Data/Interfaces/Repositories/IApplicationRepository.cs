using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Repositories
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Task<Application?> GetByCourseAndEmailAsync(int courseId, string email);
        Task<IEnumerable<Application>> GetApplicationsByCourseIdAsync(int courseId);
        Task<Application?> GetApplicationByIdAsync(int applicationId);
        void UpdateApplication(Application application);
        void DeleteApplication(Application application);
    }

}
