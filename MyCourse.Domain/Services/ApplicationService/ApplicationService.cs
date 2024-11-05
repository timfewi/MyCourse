using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using System.Threading.Tasks;
using static MyCourse.Domain.Exceptions.ApplicationEx.ApplicationExceptions;
using static MyCourse.Domain.Exceptions.CourseEx.CourseExceptions;

namespace MyCourse.Domain.Services.ApplicationService
{
    [Injectable(ServiceLifetime.Scoped)]
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IValidator<ApplicationRegistrationDto> _registrationDtoValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ICourseRepository courseRepository,
            IValidator<ApplicationRegistrationDto> registrationDtoValidator,
            IMapper mapper,
            ILogger<ApplicationService> logger)
        {
            _applicationRepository = applicationRepository;
            _courseRepository = courseRepository;
            _registrationDtoValidator = registrationDtoValidator;
            _mapper = mapper;
            _logger = logger;
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
                _logger.LogWarning("Application with ID {ApplicationId} not found.", applicationId);
                throw new ApplicationNotFoundException(applicationId);
            }
            return _mapper.Map<ApplicationDetailDto>(application);
        }

        public async Task RegisterUserForCourseAsync(int courseId, ApplicationRegistrationDto dto)
        {
            if (courseId <= 0)
            {
                _logger.LogWarning("Invalid course ID {CourseId} for registration.", courseId);
                throw new CourseNotFoundException(courseId);
            }

            // Validierung der Eingabedaten mit FluentValidation
            var validationResult = await _registrationDtoValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for registration: {Errors}",
                    string.Join("; ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            // Überprüfe, ob der Kurs existiert
            var course = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found.", courseId);
                throw new CourseNotFoundException(courseId);
            }

            // Überprüfe, ob der Kurs aktiv ist
            if (!course.IsActive)
            {
                _logger.LogWarning("Course with ID {CourseId} is not active.", courseId);
                throw new NotActiveCourseException(courseId);
            }

            // Überprüfe, ob der Kurs voll ist
            if (course.Applications.Count >= course.MaxParticipants)
            {
                _logger.LogWarning("Course with ID {CourseId} has reached max participants.", courseId);
                throw new MaxParticipantsExceededException(courseId);
            }

            // Überprüfe, ob der Benutzer bereits angemeldet ist
            var existingApplication = await _applicationRepository.GetByCourseAndEmailAsync(courseId, dto.Email);
            if (existingApplication != null)
            {
                _logger.LogWarning("Duplicate application detected for email {Email} and course ID {CourseId}.", dto.Email, courseId);
                throw new DuplicateApplicationException(dto.Email, courseId);
            }

            var application = _mapper.Map<Application>(dto);
            application.CourseId = courseId;
            application.Status = ApplicationStatusType.Pending;

            try
            {
                await _applicationRepository.AddAsync(application);
                await _applicationRepository.SaveChangesAsync();
                _logger.LogInformation("User {Email} registered for course ID {CourseId} successfully.", dto.Email, courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user {Email} for course ID {CourseId}.", dto.Email, courseId);
                throw new ApplicationSaveException("An error occurred while saving the application.", null, ex);
            }
        }

        public async Task UpdateApplicationStatusAsync(ApplicationUpdateDto applicationDto)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationDto.Id);
            if (application == null)
            {
                _logger.LogWarning("Application with ID {ApplicationId} not found for status update.", applicationDto.Id);
                throw new ApplicationNotFoundException(applicationDto.Id);
            }

            // Beispiel für Statusübergang-Validierung
            if (!IsValidStatusTransition(application.Status, applicationDto.Status))
            {
                _logger.LogWarning("Invalid status transition from {FromStatus} to {ToStatus} for application ID {ApplicationId}.",
                    application.Status, applicationDto.Status, applicationDto.Id);
                throw new InvalidStatusTransitionException(applicationDto.Id, application.Status.ToString(), applicationDto.Status.ToString());
            }

            application.Status = applicationDto.Status;
            _applicationRepository.UpdateApplication(application);

            try
            {
                await _applicationRepository.SaveChangesAsync();
                _logger.LogInformation("Application ID {ApplicationId} status updated to {Status}.", applicationDto.Id, applicationDto.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating application ID {ApplicationId}.", applicationDto.Id);
                throw new ApplicationSaveException("An error occurred while updating the application.", applicationDto.Id, ex);
            }
        }

        public async Task DeleteApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null)
            {
                _logger.LogWarning("Application with ID {ApplicationId} not found for deletion.", applicationId);
                throw new ApplicationNotFoundException(applicationId);
            }

            _applicationRepository.DeleteApplication(application);

            try
            {
                await _applicationRepository.SaveChangesAsync();
                _logger.LogInformation("Application ID {ApplicationId} deleted successfully.", applicationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting application ID {ApplicationId}.", applicationId);
                throw new ApplicationSaveException("An error occurred while deleting the application.", applicationId, ex);
            }
        }

        private bool IsValidStatusTransition(ApplicationStatusType currentStatus, ApplicationStatusType newStatus)
        {
            // Beispielhafte Implementierung der Statusübergangslogik
            return (currentStatus == ApplicationStatusType.Pending &&
                    (newStatus == ApplicationStatusType.Approved || newStatus == ApplicationStatusType.Rejected || newStatus == ApplicationStatusType.Waiting)) ||
                   (currentStatus == ApplicationStatusType.Waiting && newStatus == ApplicationStatusType.Approved);
        }

        public async Task AcceptApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                _logger.LogWarning("Application with ID {ApplicationId} not found for acceptance.", applicationId);
                throw new ApplicationNotFoundException(applicationId);
            }

            if (!IsValidStatusTransition(application.Status, ApplicationStatusType.Approved))
            {
                _logger.LogWarning("Invalid status transition to Approved for application ID {ApplicationId}.", applicationId);
                throw new InvalidStatusTransitionException(applicationId, application.Status.ToString(), ApplicationStatusType.Approved.ToString());
            }

            application.Status = ApplicationStatusType.Approved;
            application.DateUpdated = DateTime.Now;

            _applicationRepository.UpdateApplication(application);

            try
            {
                await _applicationRepository.SaveChangesAsync();
                _logger.LogInformation("Application ID {ApplicationId} accepted.", applicationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while accepting application ID {ApplicationId}.", applicationId);
                throw new ApplicationSaveException("An error occurred while accepting the application.", applicationId, ex);
            }
        }

        public async Task RejectApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                _logger.LogWarning("Application with ID {ApplicationId} not found for rejection.", applicationId);
                throw new ApplicationNotFoundException(applicationId);
            }

            if (!IsValidStatusTransition(application.Status, ApplicationStatusType.Rejected))
            {
                _logger.LogWarning("Invalid status transition to Rejected for application ID {ApplicationId}.", applicationId);
                throw new InvalidStatusTransitionException(applicationId, application.Status.ToString(), ApplicationStatusType.Rejected.ToString());
            }

            application.Status = ApplicationStatusType.Rejected;
            application.DateUpdated = DateTime.Now;

            _applicationRepository.UpdateApplication(application);

            try
            {
                await _applicationRepository.SaveChangesAsync();
                _logger.LogInformation("Application ID {ApplicationId} rejected.", applicationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting application ID {ApplicationId}.", applicationId);
                throw new ApplicationSaveException("An error occurred while rejecting the application.", applicationId, ex);
            }
        }

        public async Task SetApplicationToWaitingListAsync(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                _logger.LogWarning("Application with ID {ApplicationId} not found for waiting list.", applicationId);
                throw new ApplicationNotFoundException(applicationId);
            }

            if (!IsValidStatusTransition(application.Status, ApplicationStatusType.Waiting))
            {
                _logger.LogWarning("Invalid status transition to Waiting for application ID {ApplicationId}.", applicationId);
                throw new InvalidStatusTransitionException(applicationId, application.Status.ToString(), ApplicationStatusType.Waiting.ToString());
            }

            application.Status = ApplicationStatusType.Waiting;
            application.DateUpdated = DateTime.Now;

            _applicationRepository.UpdateApplication(application);

            try
            {
                await _applicationRepository.SaveChangesAsync();
                _logger.LogInformation("Application ID {ApplicationId} set to waiting list.", applicationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting application ID {ApplicationId} to waiting list.", applicationId);
                throw new ApplicationSaveException("An error occurred while updating the application status.", applicationId, ex);
            }
        }
    }
}
