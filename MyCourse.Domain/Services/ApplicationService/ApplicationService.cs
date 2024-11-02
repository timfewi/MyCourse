using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using MyCourse.Domain.Exceptions.ApplicationEx;
using MyCourse.Domain.Exceptions.CourseEx;
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
        private readonly ICourseRepository _courseRepository;
        private readonly IValidator<ApplicationRegistrationDto> _registrationDtoValidator;
        private readonly IMapper _mapper;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ICourseRepository courseRepository,
            IValidator<ApplicationRegistrationDto> registrationDtoValidator,
            IMapper mapper)
        {
            _applicationRepository = applicationRepository;
            _courseRepository = courseRepository;
            _registrationDtoValidator = registrationDtoValidator;
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
                throw ApplicationExceptions.NotFound(applicationId);
            }
            return _mapper.Map<ApplicationDetailDto>(application);
        }

        public async Task RegisterUserForCourseAsync(int courseId, ApplicationRegistrationDto dto)
        {
            if (courseId <= 0)
            {
                throw new ArgumentException("CourseId is invalid. Course not available.");
            }
            // Validierung der Eingabedaten mit FluentValidation
            var validationResult = await _registrationDtoValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Überprüfe, ob der Kurs existiert
            var course = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            if (course == null)
            {
                throw CourseExceptions.NotFound(courseId);
            }

            // Überprüfe, ob der Kurs aktiv ist
            if (!course.IsActive)
            {
                throw CourseExceptions.InvalidOperation("Course is not active.", courseId);
            }

            // Überprüfe, ob der Kurs voll ist
            if (course.Applications.Count >= course.MaxParticipants)
            {
                throw CourseExceptions.CourseFull(courseId);
            }

            // Überprüfe, ob der Benutzer bereits angemeldet ist
            var existingApplication = await _applicationRepository.GetByCourseAndEmailAsync(courseId, dto.Email);
            if (existingApplication != null)
            {
                throw ApplicationExceptions.DuplicateApplication(dto.Email, courseId);
            }

            var application = _mapper.Map<Application>(dto);
            application.CourseId = courseId;
            application.Status = ApplicationStatusType.Pending;

            await _applicationRepository.AddAsync(application);
            await _applicationRepository.SaveChangesAsync();
        }

        public async Task UpdateApplicationStatusAsync(ApplicationUpdateDto applicationDto)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationDto.Id);
            if (application == null)
            {
                throw ApplicationExceptions.NotFound(applicationDto.Id);
            }

            // Beispiel für Statusübergang-Validierung
            if (!IsValidStatusTransition(application.Status, applicationDto.Status))
            {
                throw ApplicationExceptions.InvalidStatusTransition(applicationDto.Id, application.Status.ToString(), applicationDto.Status.ToString());
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
                throw ApplicationExceptions.NotFound(applicationId);
            }
            _applicationRepository.DeleteApplication(application);
            await _applicationRepository.SaveChangesAsync();
        }

        private bool IsValidStatusTransition(ApplicationStatusType currentStatus, ApplicationStatusType newStatus)
        {
            return (currentStatus == ApplicationStatusType.Pending &&
                    (newStatus == ApplicationStatusType.Approved || newStatus == ApplicationStatusType.Rejected));
        }

        public async Task AcceptApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                throw ApplicationExceptions.NotFound(applicationId);
            }

            application.Status = ApplicationStatusType.Approved;
            application.DateUpdated = DateTime.Now;

            _applicationRepository.UpdateApplication(application);
            await _applicationRepository.SaveChangesAsync();
        }

        public async Task RejectApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                throw ApplicationExceptions.NotFound(applicationId);
            }

            application.Status = ApplicationStatusType.Rejected;
            application.DateUpdated = DateTime.Now;

            _applicationRepository.UpdateApplication(application);
            await _applicationRepository.SaveChangesAsync();
        }
        public async Task SetApplicationToWaitingListAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                throw ApplicationExceptions.NotFound(applicationId);
            }

            application.Status = ApplicationStatusType.Waiting;
            application.DateUpdated = DateTime.Now;

            _applicationRepository.UpdateApplication(application);
            await _applicationRepository.SaveChangesAsync();
        }
    }

}
