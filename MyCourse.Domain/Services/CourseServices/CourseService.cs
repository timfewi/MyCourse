using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IValidator<CourseCreateDto> _createDtoValidator;
        private readonly IValidator<ApplicationRegistrationDto> _registrationDtoValidator;

        public CourseService(
            ICourseRepository courseRepository,
            IApplicationRepository applicationRepository,
            IMapper mapper,
            IValidator<CourseCreateDto> createDtoValidator,
            IValidator<ApplicationRegistrationDto> registrationDtoValidator)
        {
            _courseRepository = courseRepository;
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _createDtoValidator = createDtoValidator;
            _registrationDtoValidator = registrationDtoValidator;
        }

        public async Task<IEnumerable<CourseListDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return _mapper.Map<IEnumerable<CourseListDto>>(courses);
        }

        public async Task<CourseDetailDto> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                throw CourseExceptions.NotFound(courseId);
            }
            return _mapper.Map<CourseDetailDto>(course);
        }

        public async Task CreateCourseAsync(CourseCreateDto courseDto)
        {
            var validationResult = await _createDtoValidator.ValidateAsync(courseDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var course = _mapper.Map<Course>(courseDto);
            await _courseRepository.AddCourseAsync(course);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(CourseUpdateDto courseDto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseDto.Id);
            if (course == null)
            {
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
                throw CourseExceptions.NotFound(courseId);
            }
            _courseRepository.DeleteCourse(course);
            await _courseRepository.SaveChangesAsync();
        }

        // TODO
        public async Task RegisterUserForCourseAsync(int courseId, ApplicationRegistrationDto dto)
        {
            // Validierung des DTOs (optional mit )
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
                throw CourseExceptions.InvalidOperation("User is already registered for this course.", courseId, new { dto.Email });
            }

            // Mapping vom DTO zur Entität
            var application = _mapper.Map<Application>(dto);
            application.CourseId = courseId;

            await _applicationRepository.AddAsync(application);
            await _applicationRepository.SaveChangesAsync();
        }
    }

}
