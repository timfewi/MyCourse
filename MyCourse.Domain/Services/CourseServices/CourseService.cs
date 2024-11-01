using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.CourseEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Services.CourseServices
{
    [Injectable(ServiceLifetime.Scoped)]
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseService> _logger;
        private readonly IValidator<CourseCreateDto> _createDtoValidator;
        private readonly IValidator<ApplicationRegistrationDto> _registrationDtoValidator;

        public CourseService(
            ICourseRepository courseRepository,
            IApplicationRepository applicationRepository,
            IMapper mapper,
            ILogger<CourseService> logger,
            IValidator<CourseCreateDto> createDtoValidator,
            IValidator<ApplicationRegistrationDto> registrationDtoValidator)
        {
            _courseRepository = courseRepository;
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _logger = logger;
            _createDtoValidator = createDtoValidator;
            _registrationDtoValidator = registrationDtoValidator;
        }

        public async Task<IEnumerable<CourseListDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return _mapper.Map<IEnumerable<CourseListDto>>(courses);
        }

        public async Task<IEnumerable<CourseListDto>> GetAllActiveCoursesAsync()
        {
            var activeCourses = await _courseRepository.GetAllActiveCoursesAsync();
            return _mapper.Map<IEnumerable<CourseListDto>>(activeCourses);
        }

        public async Task<CourseDetailDto> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogError("Course with {courseId} not found", courseId);
                throw CourseExceptions.NotFound(courseId);
            }
            return _mapper.Map<CourseDetailDto>(course);
        }

        public async Task CreateCourseAsync(CourseCreateDto courseDto)
        {
            var validationResult = await _createDtoValidator.ValidateAsync(courseDto);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for creating course: {Errors}",
                       string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult.Errors);
            }

            var course = _mapper.Map<Course>(courseDto);
            if (course == null)
            {
                _logger.LogError("Mapping from CourseCreateDto to Course failed.");
                throw new InvalidOperationException("Mapping from CourseCreateDto to Course failed.");
            }
            await _courseRepository.AddCourseAsync(course);
            await _courseRepository.SaveChangesAsync();
            _logger.LogInformation("Course '{CourseName}' created successfully with ID {CourseId}.", course.Title, course.Id);

        }

        public async Task UpdateCourseAsync(CourseUpdateDto courseDto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseDto.Id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for update.", courseDto.Id);
                throw CourseExceptions.NotFound(courseDto.Id);
            }
            _mapper.Map(courseDto, course);
            _courseRepository.UpdateCourse(course);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for deletion.", courseId);
                throw CourseExceptions.NotFound(courseId);
            }
            _courseRepository.DeleteCourse(course);
            await _courseRepository.SaveChangesAsync();
        }

    }

}
