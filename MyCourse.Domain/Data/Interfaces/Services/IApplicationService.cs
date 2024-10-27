using MyCourse.Domain.DTOs.ApplicationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDetailDto>> GetApplicationsByCourseIdAsync(int courseId);
        Task<ApplicationDetailDto> GetApplicationByIdAsync(int applicationId);
        Task RegisterUserForCourseAsync(int courseId, ApplicationRegistrationDto dto);
        Task UpdateApplicationStatusAsync(ApplicationUpdateDto applicationDto);
        Task DeleteApplicationAsync(int applicationId);
    }

}
