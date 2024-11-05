using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Domain.Exceptions.MediaEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyCourse.Domain.Exceptions.CourseEx.CourseExceptions;

namespace MyCourse.Domain.Services.CourseServices
{
    [Injectable(ServiceLifetime.Scoped)]
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseService> _logger;
        private readonly IValidator<CourseCreateDto> _createDtoValidator;
        private readonly IValidator<ApplicationRegistrationDto> _registrationDtoValidator;

        public CourseService(
            ICourseRepository courseRepository,
            IApplicationRepository applicationRepository,
            IMediaRepository mediaRepository,
            IMapper mapper,
            ILogger<CourseService> logger,
            IValidator<CourseCreateDto> createDtoValidator,
            IValidator<ApplicationRegistrationDto> registrationDtoValidator
            )
        {
            _courseRepository = courseRepository;
            _applicationRepository = applicationRepository;
            _mediaRepository = mediaRepository;
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
                _logger.LogWarning("Course with ID {CourseId} not found.", courseId);
                throw new CourseNotFoundException(courseId);
            }

            await _courseRepository.LoadCourseMediasAsync(course);

            return _mapper.Map<CourseDetailDto>(course);
        }

        public async Task<int> CreateCourseAsync(CourseCreateDto courseDto)
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
                throw new InvalidCourseOperationException("Mapping from CourseCreateDto to Course failed.");
            }

            try
            {
                await _courseRepository.AddCourseAsync(course);
                await _courseRepository.SaveChangesAsync();
                _logger.LogInformation("Course '{CourseName}' created successfully with ID {CourseId}.", course.Title, course.Id);

                return course.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course.");
                throw new CourseSaveException("An error occurred while saving the course.", course.Id, ex);
            }
        }

        public async Task UpdateCourseAsync(CourseUpdateDto courseDto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseDto.Id);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for update.", courseDto.Id);
                throw new CourseNotFoundException(courseDto.Id);
            }

            _mapper.Map(courseDto, course);
            _courseRepository.UpdateCourse(course);

            try
            {
                await _courseRepository.SaveChangesAsync();
                _logger.LogInformation("Course with ID {CourseId} updated successfully.", course.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating course with ID {CourseId}.", course.Id);
                throw new CourseSaveException("An error occurred while updating the course.", course.Id, ex);
            }
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for deletion.", courseId);
                throw new CourseNotFoundException(courseId);
            }

            _courseRepository.DeleteCourse(course);

            try
            {
                await _courseRepository.SaveChangesAsync();
                _logger.LogInformation("Course with ID {CourseId} deleted successfully.", courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course with ID {CourseId}.", courseId);
                throw new CourseSaveException("An error occurred while deleting the course.", courseId, ex);
            }
        }

        public async Task ToggleCourseStatusAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for status toggle.", courseId);
                throw new CourseNotFoundException(courseId);
            }

            course.IsActive = !course.IsActive;
            _courseRepository.UpdateCourse(course);

            try
            {
                await _courseRepository.SaveChangesAsync();
                _logger.LogInformation("Course '{CourseName}' status toggled to {Status}.", course.Title, course.IsActive ? "Aktiv" : "Inaktiv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling status for course with ID {CourseId}.", courseId);
                throw new CourseSaveException("An error occurred while toggling course status.", courseId, ex);
            }
        }

        public async Task<CourseEditWithImagesDto> GetCourseEditDetailsWithImagesAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found.", courseId);
                throw new CourseNotFoundException(courseId);
            }

            await _courseRepository.LoadCourseMediasAsync(course);

            var dto = _mapper.Map<CourseEditWithImagesDto>(course);
            dto.NewImages = new List<IFormFile>();
            dto.ExistingImages = course.CourseMedias.Select(cm => new CourseImageDto
            {
                MediaId = cm.MediaId,
                Url = cm.Media.Url,
                ToDelete = false
            }).ToList();

            return dto;
        }

        public async Task UpdateCourseWithImagesAsync(CourseEditWithImagesDto courseDto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseDto.CourseId);
            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found for update.", courseDto.CourseId);
                throw new CourseNotFoundException(courseDto.CourseId);
            }

            await _courseRepository.LoadCourseMediasAsync(course);

            // Update course properties
            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            course.CourseDate = courseDto.CourseDate;
            course.CourseDuration = courseDto.CourseDuration;
            course.MaxParticipants = courseDto.MaxParticipants;
            course.Location = courseDto.Location;
            course.Price = courseDto.Price;
            course.IsActive = courseDto.IsActive;

            // Handle existing images
            if (courseDto.ExistingImages != null && courseDto.ExistingImages.Any())
            {
                var imagesToDelete = courseDto.ExistingImages
                    .Where(ei => ei.ToDelete)
                    .Select(ei => ei.MediaId)
                    .ToList();

                if (imagesToDelete.Any())
                {
                    var mediasToDelete = course.CourseMedias
                        .Where(cm => imagesToDelete.Contains(cm.MediaId))
                        .Select(cm => cm.Media)
                        .ToList();

                    foreach (var media in mediasToDelete)
                    {
                        try
                        {
                            _mediaRepository.DeleteImage(media);
                            var courseMedia = course.CourseMedias.First(cm => cm.MediaId == media.Id);
                            _mediaRepository.RemoveCourseMedia(courseMedia);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deleting media with ID {MediaId} during course update.", media.Id);
                            throw new CourseUpdateException("An error occurred while deleting media during course update.", course.Id, ex);
                        }
                    }
                }
            }

            // Handle new images
            if (courseDto.NewImages != null && courseDto.NewImages.Any())
            {
                foreach (var image in courseDto.NewImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        try
                        {
                            // Save image and get the media entity
                            var media = await _mediaRepository.SaveImageAsync(image, course.Id);

                            // Add media to course
                            await _mediaRepository.AddCourseMediaAsync(course.Id, media.Id);
                        }
                        catch (MediaException ex)
                        {
                            _logger.LogError(ex, "MediaException occurred while adding new image to course with ID {CourseId}.", course.Id);
                            throw; // Weiterwerfen, um in der höheren Ebene behandelt zu werden
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unexpected error occurred while adding new image to course with ID {CourseId}.", course.Id);
                            throw new CourseUpdateException("An error occurred while adding new images to the course.", course.Id, ex);
                        }
                    }
                }
            }

            _courseRepository.UpdateCourse(course);

            try
            {
                await _courseRepository.SaveChangesAsync();
                _logger.LogInformation("Course with ID {CourseId} updated successfully.", course.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving course with ID {CourseId}.", course.Id);
                throw new CourseSaveException("An error occurred while saving the course.", course.Id, ex);
            }
        }
    }
}
