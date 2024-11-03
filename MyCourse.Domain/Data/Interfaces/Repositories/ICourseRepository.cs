using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> GetAllActiveCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task AddCourseAsync(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        Task<IEnumerable<Course>> GetActiveCoursesAsync();
        Task<Course> GetCourseWithDetailsAsync(int courseId);
        Task LoadCourseMediasAsync(Course course);
    }
}
