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

namespace MyCourse.Domain.Data.Repositories.CourseRepositories
{
    [Injectable(ServiceLifetime.Scoped)]
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _dbSet.Include(c => c.Applications)
                .Include(c => c.CourseMedias)
                .ThenInclude(cm => cm.Media)
            .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetAllActiveCoursesAsync()
        {
            var activeCourses = await _dbSet.Where(c => c.IsActive)
            .Include(c => c.CourseMedias)
                .ThenInclude(cm => cm.Media)
            .ToListAsync();
            return activeCourses;
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _dbSet
                .Include(c => c.Applications)
                .Include(c => c.CourseMedias)
                    .ThenInclude(cm => cm.Media)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task AddCourseAsync(Course course)
        {
            await _dbSet.AddAsync(course);
        }

        public void UpdateCourse(Course course)
        {
            _dbSet.Update(course);
        }

        public void DeleteCourse(Course course)
        {
            _dbSet.Remove(course);
        }
        public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
        {
            return await _dbSet.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<Course> GetCourseWithDetailsAsync(int courseId)
        {
            var course = await _dbSet
                .Include(c => c.CourseMedias)
                    .ThenInclude(cm => cm.Media)
                 .Include(c => c.Applications)
                 .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                throw new InvalidOperationException($"Course with id {courseId} not found.");
            }

            return course;
        }

        public async Task LoadCourseMediasAsync(Course course)
        {
            await _dbContext.Entry(course)
                .Collection(c => c.CourseMedias)
                .Query()
                .Include(cm => cm.Media)
                .LoadAsync();
        }

    }
}
