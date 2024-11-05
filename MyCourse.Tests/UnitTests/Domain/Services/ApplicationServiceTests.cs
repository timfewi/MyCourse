using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using MyCourse.Domain.Exceptions.ApplicationEx;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Domain.Services.ApplicationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static MyCourse.Domain.Exceptions.ApplicationEx.ApplicationExceptions;
using static MyCourse.Domain.Exceptions.CourseEx.CourseExceptions;

namespace MyCourse.Tests.UnitTests.Domain.Services
{
    public class ApplicationServiceTests : TestBase
    {
        private readonly Mock<IApplicationRepository> _mockApplicationRepository;
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IValidator<ApplicationRegistrationDto>> _mockRegistrationDtoValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ApplicationService>> _mockLogger;
        private readonly ApplicationService _applicationService;

        public ApplicationServiceTests()
        {
            // Mock objects
            _mockApplicationRepository = new Mock<IApplicationRepository>();
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockRegistrationDtoValidator = new Mock<IValidator<ApplicationRegistrationDto>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ApplicationService>>();

            // ServiceProvider setup
            _services.AddSingleton(_mockRegistrationDtoValidator.Object);
            _services.AddSingleton(_mockMapper.Object);
            _services.AddSingleton(_mockLogger.Object);

            _serviceProvider = _services.BuildServiceProvider();

            // ApplicationService instantiation
            _applicationService = new ApplicationService(
                _mockApplicationRepository.Object,
                _mockCourseRepository.Object,
                _mockRegistrationDtoValidator.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        #region GetApplicationsByCourseIdAsync Tests
        [Fact]
        public async Task GetApplicationsByCourseIdAsync_ValidCourseId_ReturnsApplications()
        {
            // Arrange
            int courseId = 1;
            var applications = new List<Application>
            {
                new Application { Id = 1, CourseId = courseId, Email = "test1@example.com" },
                new Application { Id = 2, CourseId = courseId, Email = "test2@example.com" }
            };

            _mockApplicationRepository.Setup(repo => repo.GetApplicationsByCourseIdAsync(courseId))
                .ReturnsAsync(applications);

            var applicationDtos = new List<ApplicationDetailDto>
            {
                new ApplicationDetailDto { Id = 1, CourseId = courseId, Email = "test1@example.com" },
                new ApplicationDetailDto { Id = 2, CourseId = courseId, Email = "test2@example.com" }
            };

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ApplicationDetailDto>>(applications))
                .Returns(applicationDtos);

            // Act
            var result = await _applicationService.GetApplicationsByCourseIdAsync(courseId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(applicationDtos);
            _mockApplicationRepository.Verify(repo => repo.GetApplicationsByCourseIdAsync(courseId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<ApplicationDetailDto>>(applications), Times.Once);
        }
        #endregion

        #region GetApplicationByIdAsync Tests
        [Fact]
        public async Task GetApplicationByIdAsync_ExistingId_ReturnsApplicationDetailDto()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application { Id = applicationId, Email = "test@example.com" };

            _mockApplicationRepository.Setup(repo => repo.GetApplicationByIdAsync(applicationId))
                .ReturnsAsync(application);

            var applicationDto = new ApplicationDetailDto { Id = applicationId, Email = "test@example.com" };

            _mockMapper.Setup(mapper => mapper.Map<ApplicationDetailDto>(application))
                .Returns(applicationDto);

            // Act
            var result = await _applicationService.GetApplicationByIdAsync(applicationId);

            // Assert
            result.Should().BeEquivalentTo(applicationDto);
            _mockApplicationRepository.Verify(repo => repo.GetApplicationByIdAsync(applicationId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<ApplicationDetailDto>(application), Times.Once);
        }

        [Fact]
        public async Task GetApplicationByIdAsync_NonExistingId_ThrowsApplicationNotFoundException()
        {
            // Arrange
            int applicationId = 99;

            _mockApplicationRepository.Setup(repo => repo.GetApplicationByIdAsync(applicationId))
                .ReturnsAsync((Application)null!);

            // Act
            Func<Task> act = async () => await _applicationService.GetApplicationByIdAsync(applicationId);

            // Assert
            var exception = await Assert.ThrowsAsync<ApplicationNotFoundException>(act);
            exception.ErrorCode.Should().Be(ApplicationErrorCode.NotFound);
            exception.Message.Should().Be($"Application with ID {applicationId} not found.");
            exception.ApplicationId.Should().Be(applicationId);

            _mockApplicationRepository.Verify(repo => repo.GetApplicationByIdAsync(applicationId), Times.Once);
        }
        #endregion

        #region RegisterUserForCourseAsync Tests
        [Fact]
        public async Task RegisterUserForCourseAsync_ValidData_RegistersUser()
        {
            // Arrange
            int courseId = 1;
            var dto = new ApplicationRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789"
            };

            var validationResult = new ValidationResult();
            _mockRegistrationDtoValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            var course = new Course
            {
                Id = courseId,
                IsActive = true,
                MaxParticipants = 10,
                Applications = new List<Application>()
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            _mockApplicationRepository.Setup(repo => repo.GetByCourseAndEmailAsync(courseId, dto.Email))
                .ReturnsAsync((Application)null!);

            var application = new Application
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                CourseId = courseId,
                Status = ApplicationStatusType.Pending
            };

            _mockMapper.Setup(m => m.Map<Application>(dto))
                .Returns(application);

            _mockApplicationRepository.Setup(repo => repo.AddAsync(application))
                .Returns(Task.CompletedTask);

            _mockApplicationRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _applicationService.RegisterUserForCourseAsync(courseId, dto);

            // Assert
            _mockRegistrationDtoValidator.Verify(v => v.ValidateAsync(dto, default), Times.Once);
            _mockCourseRepository.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.GetByCourseAndEmailAsync(courseId, dto.Email), Times.Once);
            _mockMapper.Verify(m => m.Map<Application>(dto), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.AddAsync(application), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUserForCourseAsync_InvalidCourseId_ThrowsCourseNotFoundException()
        {
            // Arrange
            int courseId = -1; // Invalid course ID
            var dto = new ApplicationRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789"
            };

            // Act
            Func<Task> act = async () => await _applicationService.RegisterUserForCourseAsync(courseId, dto);

            // Assert
            var exception = await Assert.ThrowsAsync<CourseNotFoundException>(act);
            exception.ErrorCode.Should().Be(CourseErrorCode.NotFound);
            exception.Message.Should().Be($"Course with ID {courseId} not found.");
            exception.CourseId.Should().Be(courseId);

            _mockRegistrationDtoValidator.Verify(v => v.ValidateAsync(It.IsAny<ApplicationRegistrationDto>(), default), Times.Never);
            _mockCourseRepository.Verify(repo => repo.GetCourseWithDetailsAsync(It.IsAny<int>()), Times.Never);
            _mockApplicationRepository.Verify(repo => repo.GetByCourseAndEmailAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserForCourseAsync_InactiveCourse_ThrowsInactiveCourseException()
        {
            // Arrange
            int courseId = 1;
            var dto = new ApplicationRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789"
            };

            var validationResult = new ValidationResult();
            _mockRegistrationDtoValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            var course = new Course
            {
                Id = courseId,
                IsActive = false, // Inactive course
                MaxParticipants = 10,
                Applications = new List<Application>()
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            Func<Task> act = async () => await _applicationService.RegisterUserForCourseAsync(courseId, dto);

            // Assert
            var exception = await Assert.ThrowsAsync<NotActiveCourseException>(act);
            exception.ErrorCode.Should().Be(CourseErrorCode.NotActive);
            exception.Message.Should().Be($"Course with ID {courseId} is not active.");
            exception.CourseId.Should().Be(courseId);

            _mockRegistrationDtoValidator.Verify(v => v.ValidateAsync(dto, default), Times.Once);
            _mockCourseRepository.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.GetByCourseAndEmailAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserForCourseAsync_CourseFull_ThrowsMaxParticipantsExceededException()
        {
            // Arrange
            int courseId = 1;
            var dto = new ApplicationRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789"
            };

            var validationResult = new ValidationResult();
            _mockRegistrationDtoValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            var course = new Course
            {
                Id = courseId,
                IsActive = true,
                MaxParticipants = 1,
                Applications = new List<Application>
                {
                    new Application { Id = 1, CourseId = courseId, Email = "existing@example.com" }
                }
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            Func<Task> act = async () => await _applicationService.RegisterUserForCourseAsync(courseId, dto);

            // Assert
            var exception = await Assert.ThrowsAsync<MaxParticipantsExceededException>(act);
            exception.ErrorCode.Should().Be(CourseErrorCode.MaxParticipantsExceeded);
            exception.Message.Should().Be($"Maximum number of participants for course ID {courseId} has been reached.");
            exception.CourseId.Should().Be(courseId);

            _mockRegistrationDtoValidator.Verify(v => v.ValidateAsync(dto, default), Times.Once);
            _mockCourseRepository.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.GetByCourseAndEmailAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserForCourseAsync_DuplicateApplication_ThrowsDuplicateApplicationException()
        {
            // Arrange
            int courseId = 1;
            var dto = new ApplicationRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789"
            };

            var validationResult = new ValidationResult();
            _mockRegistrationDtoValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            var course = new Course
            {
                Id = courseId,
                IsActive = true,
                MaxParticipants = 10,
                Applications = new List<Application>()
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            var existingApplication = new Application
            {
                Id = 1,
                CourseId = courseId,
                Email = dto.Email
            };

            _mockApplicationRepository.Setup(repo => repo.GetByCourseAndEmailAsync(courseId, dto.Email))
                .ReturnsAsync(existingApplication);

            // Act
            Func<Task> act = async () => await _applicationService.RegisterUserForCourseAsync(courseId, dto);

            // Assert
            var exception = await Assert.ThrowsAsync<DuplicateApplicationException>(act);
            exception.ErrorCode.Should().Be(ApplicationErrorCode.DuplicateApplication);
            exception.Message.Should().Be($"An application with email '{dto.Email}' already exists for course ID {courseId}.");
            exception.AdditionalData.Should().BeEquivalentTo(new { email = dto.Email, courseId });

            _mockRegistrationDtoValidator.Verify(v => v.ValidateAsync(dto, default), Times.Once);
            _mockCourseRepository.Verify(repo => repo.GetCourseWithDetailsAsync(courseId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.GetByCourseAndEmailAsync(courseId, dto.Email), Times.Once);
        }
        #endregion

        #region UpdateApplicationStatusAsync Tests
        [Fact]
        public async Task UpdateApplicationStatusAsync_ValidStatusTransition_UpdatesStatus()
        {
            // Arrange
            var applicationDto = new ApplicationUpdateDto
            {
                Id = 1,
                Status = ApplicationStatusType.Approved
            };

            var application = new Application
            {
                Id = applicationDto.Id,
                Status = ApplicationStatusType.Pending
            };

            _mockApplicationRepository.Setup(repo => repo.GetApplicationByIdAsync(applicationDto.Id))
                .ReturnsAsync(application);

            _mockApplicationRepository.Setup(repo => repo.UpdateApplication(application));
            _mockApplicationRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _applicationService.UpdateApplicationStatusAsync(applicationDto);

            // Assert
            application.Status.Should().Be(ApplicationStatusType.Approved);
            _mockApplicationRepository.Verify(repo => repo.GetApplicationByIdAsync(applicationDto.Id), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(application), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateApplicationStatusAsync_InvalidStatusTransition_ThrowsInvalidStatusTransitionException()
        {
            // Arrange
            var applicationDto = new ApplicationUpdateDto
            {
                Id = 1,
                Status = ApplicationStatusType.Approved
            };

            var application = new Application
            {
                Id = applicationDto.Id,
                Status = ApplicationStatusType.Rejected // Invalid transition from Rejected to Approved
            };

            _mockApplicationRepository.Setup(repo => repo.GetApplicationByIdAsync(applicationDto.Id))
                .ReturnsAsync(application);

            // Act
            Func<Task> act = async () => await _applicationService.UpdateApplicationStatusAsync(applicationDto);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidStatusTransitionException>(act);
            exception.ErrorCode.Should().Be(ApplicationErrorCode.InvalidStatusTransition);
            exception.Message.Should().Be($"Cannot transition application ID {applicationDto.Id} from {application.Status} to {applicationDto.Status}.");
            exception.AdditionalData.Should().BeEquivalentTo(new { fromStatus = application.Status.ToString(), toStatus = applicationDto.Status.ToString() });

            _mockApplicationRepository.Verify(repo => repo.GetApplicationByIdAsync(applicationDto.Id), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(It.IsAny<Application>()), Times.Never);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region DeleteApplicationAsync Tests
        [Fact]
        public async Task DeleteApplicationAsync_ExistingId_DeletesApplication()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application { Id = applicationId };

            _mockApplicationRepository.Setup(repo => repo.GetApplicationByIdAsync(applicationId))
                .ReturnsAsync(application);

            _mockApplicationRepository.Setup(repo => repo.DeleteApplication(application));
            _mockApplicationRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _applicationService.DeleteApplicationAsync(applicationId);

            // Assert
            _mockApplicationRepository.Verify(repo => repo.GetApplicationByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.DeleteApplication(application), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteApplicationAsync_NonExistingId_ThrowsApplicationNotFoundException()
        {
            // Arrange
            int applicationId = 99;

            _mockApplicationRepository.Setup(repo => repo.GetApplicationByIdAsync(applicationId))
                .ReturnsAsync((Application)null!);

            // Act
            Func<Task> act = async () => await _applicationService.DeleteApplicationAsync(applicationId);

            // Assert
            var exception = await Assert.ThrowsAsync<ApplicationNotFoundException>(act);
            exception.ErrorCode.Should().Be(ApplicationErrorCode.NotFound);
            exception.Message.Should().Be($"Application with ID {applicationId} not found.");
            exception.ApplicationId.Should().Be(applicationId);

            _mockApplicationRepository.Verify(repo => repo.GetApplicationByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.DeleteApplication(It.IsAny<Application>()), Times.Never);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region AcceptApplicationAsync Tests
        [Fact]
        public async Task AcceptApplicationAsync_ValidStatusTransition_UpdatesStatus()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application
            {
                Id = applicationId,
                Status = ApplicationStatusType.Pending
            };

            _mockApplicationRepository.Setup(repo => repo.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            _mockApplicationRepository.Setup(repo => repo.UpdateApplication(application));
            _mockApplicationRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _applicationService.AcceptApplicationAsync(applicationId);

            // Assert
            application.Status.Should().Be(ApplicationStatusType.Approved);
            _mockApplicationRepository.Verify(repo => repo.GetByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(application), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AcceptApplicationAsync_InvalidStatusTransition_ThrowsInvalidStatusTransitionException()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application
            {
                Id = applicationId,
                Status = ApplicationStatusType.Rejected // Invalid transition from Rejected to Approved
            };

            _mockApplicationRepository.Setup(repo => repo.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            // Act
            Func<Task> act = async () => await _applicationService.AcceptApplicationAsync(applicationId);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidStatusTransitionException>(act);
            exception.ErrorCode.Should().Be(ApplicationErrorCode.InvalidStatusTransition);
            exception.Message.Should().Be($"Cannot transition application ID {applicationId} from {application.Status} to {ApplicationStatusType.Approved}.");
            exception.AdditionalData.Should().BeEquivalentTo(new { fromStatus = application.Status.ToString(), toStatus = ApplicationStatusType.Approved.ToString() });

            _mockApplicationRepository.Verify(repo => repo.GetByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(It.IsAny<Application>()), Times.Never);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region RejectApplicationAsync Tests
        [Fact]
        public async Task RejectApplicationAsync_ValidStatusTransition_UpdatesStatus()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application
            {
                Id = applicationId,
                Status = ApplicationStatusType.Pending
            };

            _mockApplicationRepository.Setup(repo => repo.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            _mockApplicationRepository.Setup(repo => repo.UpdateApplication(application));
            _mockApplicationRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _applicationService.RejectApplicationAsync(applicationId);

            // Assert
            application.Status.Should().Be(ApplicationStatusType.Rejected);
            _mockApplicationRepository.Verify(repo => repo.GetByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(application), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region SetApplicationToWaitingListAsync Tests
        [Fact]
        public async Task SetApplicationToWaitingListAsync_ValidStatusTransition_UpdatesStatus()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application
            {
                Id = applicationId,
                Status = ApplicationStatusType.Pending
            };

            _mockApplicationRepository.Setup(repo => repo.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            _mockApplicationRepository.Setup(repo => repo.UpdateApplication(application));
            _mockApplicationRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _applicationService.SetApplicationToWaitingListAsync(applicationId);

            // Assert
            application.Status.Should().Be(ApplicationStatusType.Waiting);
            _mockApplicationRepository.Verify(repo => repo.GetByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(application), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SetApplicationToWaitingListAsync_InvalidStatusTransition_ThrowsInvalidStatusTransitionException()
        {
            // Arrange
            int applicationId = 1;
            var application = new Application
            {
                Id = applicationId,
                Status = ApplicationStatusType.Approved // Invalid transition from Approved to Waiting
            };

            _mockApplicationRepository.Setup(repo => repo.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            // Act
            Func<Task> act = async () => await _applicationService.SetApplicationToWaitingListAsync(applicationId);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidStatusTransitionException>(act);
            exception.ErrorCode.Should().Be(ApplicationErrorCode.InvalidStatusTransition);
            exception.Message.Should().Be($"Cannot transition application ID {applicationId} from {application.Status} to {ApplicationStatusType.Waiting}.");
            exception.AdditionalData.Should().BeEquivalentTo(new { fromStatus = application.Status.ToString(), toStatus = ApplicationStatusType.Waiting.ToString() });

            _mockApplicationRepository.Verify(repo => repo.GetByIdAsync(applicationId), Times.Once);
            _mockApplicationRepository.Verify(repo => repo.UpdateApplication(It.IsAny<Application>()), Times.Never);
            _mockApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
        #endregion
    }
}
