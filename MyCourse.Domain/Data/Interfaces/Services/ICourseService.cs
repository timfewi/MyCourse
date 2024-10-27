using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseListDto>> GetAllCoursesAsync();
        Task<CourseDetailDto> GetCourseByIdAsync(int courseId);
        Task CreateCourseAsync(CourseCreateDto courseDto);
        Task UpdateCourseAsync(CourseUpdateDto courseDto);
        Task DeleteCourseAsync(int courseId);
        Task RegisterUserForCourseAsync(int courseId, ApplicationRegistrationDto dto);
    }
}
