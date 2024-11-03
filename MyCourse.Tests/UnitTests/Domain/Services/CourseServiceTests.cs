using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Domain.Exceptions.CourseExceptions.CourseEx;
using MyCourse.Domain.Services.CourseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.Services
{
    public class CourseServiceTests : TestBase
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IApplicationRepository> _mockApplicationRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<CourseCreateDto>> _mockCreateDtoValidator;
        private readonly Mock<IValidator<ApplicationRegistrationDto>> _mockRegistrationDtoValidator;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            // Mock objects
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockApplicationRepository = new Mock<IApplicationRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCreateDtoValidator = new Mock<IValidator<CourseCreateDto>>();
            _mockRegistrationDtoValidator = new Mock<IValidator<ApplicationRegistrationDto>>();

            // Mock objects to ServiceCollection
            _services.AddSingleton(_mockCreateDtoValidator.Object);
            _services.AddSingleton(_mockApplicationRepository.Object);
            _services.AddSingleton(_mockMapper.Object);
            _services.AddSingleton(_mockCreateDtoValidator.Object);
            _services.AddSingleton(_mockRegistrationDtoValidator.Object);

            _serviceProvider = _services.BuildServiceProvider();

            _courseService = new CourseService(
                _mockCourseRepository.Object,
                _mockApplicationRepository.Object,
                _mockMapper.Object,
                _serviceProvider.GetRequiredService<ILogger<CourseService>>(),
                _mockCreateDtoValidator.Object,
                _mockRegistrationDtoValidator.Object
                );
        }


        #region GetAllCoursesAsync Tests
        [Fact]
        public async Task GetAllCoursesAsync_ReturnsAllCourses()
        {
            // Arrange 
            var courses = new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Title = TestConstants.ValidTitle,
                    Description = TestConstants.ValidDescription,
                    CourseDate = DateTime.Today,
                    CourseDuration = TimeSpan.FromHours(2),
                    MaxParticipants = 10,
                    Price = TestConstants.ValidPrice,
                    Location = TestConstants.ValidLocation,
                    IsActive = true,
                },
                new Course
                {
                    Id = 1,
                    Title = TestConstants.ValidTitle + "2",
                    Description = TestConstants.ValidDescription + "2",
                    CourseDate = DateTime.Today,
                    CourseDuration = TimeSpan.FromHours(4),
                    MaxParticipants = 20,
                    Price = TestConstants.ValidPrice,
                    Location = TestConstants.ValidLocation,
                    IsActive = false,
                }
            };

            _mockCourseRepository.Setup(repo => repo.GetAllCoursesAsync())
                .ReturnsAsync(courses);

            var courseListDtos = new List<CourseListDto>
            {
                new CourseListDto { Id = 1, Title = TestConstants.ValidTitle, Description = TestConstants.ValidDescription ,CourseDuration = TimeSpan.FromHours(4), CourseDate = DateTime.Today, IsActive = true },
                new CourseListDto { Id = 2, Title = TestConstants.ValidTitle + "2", Description = TestConstants.ValidDescription + "2",CourseDuration = TimeSpan.FromHours(4) ,CourseDate = DateTime.Today, IsActive = false },
            };

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<CourseListDto>>(It.IsAny<IEnumerable<Course>>()))
                .Returns(courseListDtos);

            // Act
            var result = await _courseService.GetAllCoursesAsync();

            // Assert 
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(courseListDtos);
            _mockCourseRepository.Verify(repo => repo.GetAllCoursesAsync(), Times.Once());
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<CourseListDto>>(courses), Times.Once());

        }
        #endregion

        #region GetAllActiveCoursesAsync Tests
        [Fact]
        public async Task GetAllActiveCoursesAsync_ReturnsAllCourses()
        {
            // Arrange 
            var courses = new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Title = TestConstants.ValidTitle,
                    Description = TestConstants.ValidDescription,
                    CourseDate = DateTime.Today,
                    CourseDuration = TimeSpan.FromHours(2),
                    MaxParticipants = 10,
                    Price = TestConstants.ValidPrice,
                    Location = TestConstants.ValidLocation,
                    IsActive = true,
                },
                new Course
                {
                    Id = 1,
                    Title = TestConstants.ValidTitle + "2",
                    Description = TestConstants.ValidDescription + "2",
                    CourseDate = DateTime.Today,
                    CourseDuration = TimeSpan.FromHours(4),
                    MaxParticipants = 20,
                    Price = TestConstants.ValidPrice,
                    Location = TestConstants.ValidLocation,
                    IsActive = false,
                }
            };

            _mockCourseRepository.Setup(repo => repo.GetAllActiveCoursesAsync())
                .ReturnsAsync(courses);

            var courseListDtos = new List<CourseListDto>
            {
                new CourseListDto { Id = 1, Title = TestConstants.ValidTitle, Description = TestConstants.ValidDescription ,CourseDuration = TimeSpan.FromHours(4), CourseDate = DateTime.Today, IsActive = true },
            };

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<CourseListDto>>(It.IsAny<IEnumerable<Course>>()))
                .Returns(courseListDtos);

            // Act
            var result = await _courseService.GetAllActiveCoursesAsync();

            // Assert 
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(courseListDtos);
            _mockCourseRepository.Verify(repo => repo.GetAllActiveCoursesAsync(), Times.Once());
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<CourseListDto>>(courses), Times.Once());

        }
        #endregion

        #region GetCourseByIdAsync Tests
        [Fact]
        public async Task GetCourseByIdAsync_ExistingId_ReturnsCourseDetailDto()
        {
            // Arrange
            int courseId = 1;
            var course = new Course
            {
                Id = courseId,
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Today.AddDays(1),
                CourseDuration = TimeSpan.FromHours(2),
                MaxParticipants = 10,
                IsActive = true
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseByIdAsync(courseId))
              .ReturnsAsync(course);

            var courseDetailDto = new CourseDetailDto
            {
                Id = courseId,
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Today.AddDays(1),
                CourseDuration = TimeSpan.FromHours(2),
                MaxParticipants = 10,
                IsActive = true
            };

            _mockMapper.Setup(mapper => mapper.Map<CourseDetailDto>(course))
                .Returns(courseDetailDto);

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            result.Should().BeEquivalentTo(courseDetailDto);
            result.Should().BeOfType(typeof(CourseDetailDto));
            _mockCourseRepository.Verify(repo => repo.GetCourseByIdAsync(courseId), Times.Once());
            _mockMapper.Verify(mapper => mapper.Map<CourseDetailDto>(course), Times.Once());


        }


        [Fact]
        public async Task GetCourseByIdAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            int courseId = 1;

            _mockCourseRepository.Setup(repo => repo.GetCourseByIdAsync(courseId))
                .ReturnsAsync((Course)null!);

            // Act 
            Func<Task> act = async () => await _courseService.GetCourseByIdAsync(courseId);

            // Assert 
            var exception = await Assert.ThrowsAsync<CourseException>(act);
            exception.ErrorCode.Should().Be(CourseErrorCode.NotFound);
            exception.Message.Should().Be($"Course with id {courseId} not found.");
            exception.CourseId.Should().Be(courseId);
            _mockCourseRepository.Verify(repo => repo.GetCourseByIdAsync(courseId), Times.Once);

        }
        #endregion

        #region CreateCourseAsync Tests
        [Fact]
        public async Task CreateCourseAsync_ValidCourse_CreatesCourse()
        {
            // Arrange
            var courseCreateDto = new CourseCreateDto
            {
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Today,
                CourseDuration = TimeSpan.FromHours(1),
                MaxParticipants = 10,
                Location = TestConstants.ValidLocation,
                Price = TestConstants.ValidPrice,
                IsActive = true,
            };

            var course = new Course
            {
                Title = courseCreateDto.Title,
                Description = courseCreateDto.Description,
                CourseDate = courseCreateDto.CourseDate,
                CourseDuration = courseCreateDto.CourseDuration,
                MaxParticipants = courseCreateDto.MaxParticipants,
                Location = courseCreateDto.Location,
                Price = courseCreateDto.Price,
                IsActive = courseCreateDto.IsActive,
            };

            _mockCreateDtoValidator.Setup(v => v.ValidateAsync(courseCreateDto, default))
               .ReturnsAsync(new ValidationResult());

            _mockMapper.Setup(m => m.Map<Course>(It.IsAny<CourseCreateDto>())).Returns(course);

            _mockCourseRepository.Setup(repo => repo.AddCourseAsync(It.IsAny<Course>()))
                .Callback<Course>(c => c.Id = 1)
                .Returns(Task.CompletedTask);

            _mockCourseRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act 
            var courseId = await _courseService.CreateCourseAsync(courseCreateDto);

            // Assert
            _mockCreateDtoValidator.Verify(v => v.ValidateAsync(courseCreateDto, default), Times.Once);
            _mockMapper.Verify(m => m.Map<Course>(courseCreateDto), Times.Once);
            _mockCourseRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Once);
            _mockCourseRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCourseAsync_InvalidCourse_ThrowsValidationException()
        {
            // Arrange 
            var courseCreateDto = new CourseCreateDto
            {
                Title = TestConstants.ValidTitle,
                Description = TestConstants.InvalidDescription,
                Location = TestConstants.InvalidLocation,
                Price = TestConstants.InvalidPrice,
                CourseDuration = TestConstants.InvalidCourseDuration,
                CourseDate = DateTime.Today.AddDays(-10),
                MaxParticipants = -1,
                IsActive = false,
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is invalid."),
                new ValidationFailure("Description", "Description is invalid."),
                new ValidationFailure("Location", "Location is invalid."),
                new ValidationFailure("Price", "Price must be a positive value."),
                new ValidationFailure("CourseDate", "Course date cannot be in the past."),
                new ValidationFailure("MaxParticipants", "Max participants must be a positive   number.")
            };

            var validationResult = new ValidationResult(validationFailures);
            _mockCreateDtoValidator.Setup(v => v.ValidateAsync(courseCreateDto, default))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task<int>> act = async () => await _courseService.CreateCourseAsync(courseCreateDto);

            // Assert 
            var exception = await Assert.ThrowsAsync<ValidationException>(act);
            exception.Errors.Should().BeEquivalentTo(validationFailures);

            _mockCreateDtoValidator.Verify(v => v.ValidateAsync(courseCreateDto, default), Times.Once());
            _mockMapper.Verify(m => m.Map<Course>(It.IsAny<CourseCreateDto>()), Times.Never());
            _mockCourseRepository.Verify(repo => repo.AddCourseAsync(It.IsAny<Course>()), Times.Never());
            _mockCourseRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never());

        }
        #endregion

        #region UpdateCourseAsync Tests
        [Fact]
        public async Task UpdateCourseAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange 
            var courseUpdateDto = new CourseUpdateDto
            {
                Id = 99,
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Now.AddDays(1),
                CourseDuration = TestConstants.ValidCourseDuration,
                Price = 45.99m,
                MaxParticipants = 10,
                Location = TestConstants.ValidLocation,
                IsActive = true,
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseByIdAsync(courseUpdateDto.Id)).ReturnsAsync((Course)null!);

            // Act
            Func<Task> act = async () => await _courseService.UpdateCourseAsync(courseUpdateDto);

            var exception = await Assert.ThrowsAsync<CourseException>(act);
            exception.ErrorCode.Should().Be(CourseErrorCode.NotFound);
            exception.Message.Should().Be($"Course with id {courseUpdateDto.Id} not found.");
            exception.CourseId.Should().Be(courseUpdateDto.Id);

            _mockCourseRepository.Verify(repo => repo.GetCourseByIdAsync(courseUpdateDto.Id), Times.Once);
            _mockMapper.Verify(m => m.Map(It.IsAny<CourseUpdateDto>(), It.IsAny<Course>()), Times.Never);
            _mockCourseRepository.Verify(repo => repo.UpdateCourse(It.IsAny<Course>()), Times.Never);
            _mockCourseRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);

        }
        #endregion

        #region DeleteCourseAsync Tests
        [Fact]
        public async Task DeleteCourseAsync_ExistingId_DeletesCourse()
        {
            // Arrange
            int courseId = 1;

            var existingCourse = new Course
            {
                Id = 99,
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Now.AddDays(1),
                CourseDuration = TestConstants.ValidCourseDuration,
                Price = 45.99m,
                MaxParticipants = 10,
                Location = TestConstants.ValidLocation,
                IsActive = true,
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseByIdAsync(courseId))
                .ReturnsAsync(existingCourse);

            _mockCourseRepository.Setup(repo => repo.DeleteCourse(existingCourse));
            _mockCourseRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _courseService.DeleteCourseAsync(courseId);

            // Assert
            _mockCourseRepository.Verify(repo => repo.GetCourseByIdAsync(courseId), Times.Once);
            _mockCourseRepository.Verify(repo => repo.DeleteCourse(existingCourse), Times.Once);
            _mockCourseRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            int courseId = 99;

            _mockCourseRepository.Setup(repo => repo.GetCourseByIdAsync(courseId))
                .ReturnsAsync((Course)null!);

            // Act
            Func<Task> act = async () => await _courseService.DeleteCourseAsync(courseId);

            // Assert
            var exception = await Assert.ThrowsAsync<CourseException>(act);
            exception.ErrorCode.Should().Be(CourseErrorCode.NotFound);
            exception.Message.Should().Be($"Course with id {courseId} not found.");
            exception.CourseId.Should().Be(courseId);

            _mockCourseRepository.Verify(repo => repo.GetCourseByIdAsync(courseId), Times.Once);
            _mockCourseRepository.Verify(repo => repo.DeleteCourse(It.IsAny<Course>()), Times.Never);
            _mockCourseRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        #endregion
    }
}
