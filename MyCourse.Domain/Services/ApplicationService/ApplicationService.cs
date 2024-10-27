using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ApplicationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Services.ApplicationService
{
    [Injectable(ServiceLifetime.Scoped)]
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMapper _mapper;

        public ApplicationService(IApplicationRepository applicationRepository, IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationDetailDto>> GetApplicationsByCourseIdAsync(int courseId)
        {
            var applications = await _applicationRepository.GetApplicationsByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<ApplicationDetailDto>>(applications);
        }

        public async Task<ApplicationDetailDto> GetApplicationByIdAsync(int applicationId)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null)
            {
                throw new ApplicationNotFoundException(applicationId);
            }
            return _mapper.Map<ApplicationDetailDto>(application);
        }

        public async Task UpdateApplicationStatusAsync(ApplicationUpdateDto applicationDto)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationDto.Id);
            if (application == null)
            {
                throw new ApplicationNotFoundException(applicationDto.Id);
            }
            application.Status = applicationDto.Status;
            _applicationRepository.UpdateApplication(application);
            await _applicationRepository.SaveChangesAsync();
        }

        public async Task DeleteApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null)
            {
                throw new ApplicationNotFoundException(applicationId);
            }
            _applicationRepository.DeleteApplication(application);
            await _applicationRepository.SaveChangesAsync();
        }
    }

}
