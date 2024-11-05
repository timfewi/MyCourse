using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.Data.Repositories.MediaRepositories;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.MediaEx;
using MyCourse.Domain.Services.MediaServices;
using MyCourse.Domain.Validation.DtoValidations.MediaDtoValidations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.Services
{
    public class MediaServiceTests : TestBase
    {
        private readonly IMediaService _mediaService;
        private readonly IValidator<MediaCreateDto> _mediaCreateValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<MediaService> _logger;
        private readonly IMediaRepository _mediaRepository;


        public MediaServiceTests()
        {
            ConfigureServices(_services);

            _mediaRepository = _serviceProvider.GetRequiredService<IMediaRepository>();
            _mediaCreateValidator = _serviceProvider.GetRequiredService<IValidator<MediaCreateDto>>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _logger = _serviceProvider.GetRequiredService<ILogger<MediaService>>();

            _mediaService = new MediaService(
                _mediaRepository,
                _logger,
                _mapper,
                _mediaCreateValidator
            );
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddScoped<IMediaRepository, MediaRepository>();

            services.AddValidatorsFromAssemblyContaining<MediaCreateDtoValidator>();
            services.AddAutoMapper(typeof(MediaService).Assembly);

        }

        [Fact]
        public async Task CreateMediaAsync_WithValidationData_ShouldReturnMediaId()
        {
            var mediaDto = new MediaCreateDto
            {
                FileName = TestConstants.ValidFileName,
                Url = TestConstants.ValidMediaUrl,
                MediaType = MyCourse.Domain.Enums.MediaType.Image,
                ContentType = "image/jpg",
                Description = TestConstants.ValidDescription,
                FileSize = TestConstants.ValidFileSize,
            };

            // Act
            var mediaId = await _mediaService.CreateMediaAsync(mediaDto);

            // Assert
            Assert.True(mediaId > 0);

            var media = await _context.Medias.FindAsync(mediaId);
            Assert.NotNull(media);
            Assert.Equal(mediaDto.FileName, media.FileName);
            Assert.Equal(mediaDto.Url, media.Url);
            Assert.Equal(mediaDto.MediaType, media.MediaType);
            Assert.Equal(mediaDto.ContentType, media.ContentType);
            Assert.Equal(mediaDto.Description, media.Description);
            Assert.Equal(mediaDto.FileSize, media.FileSize);
        }


        [Fact]
        public async Task CreateMediaAsync_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var mediaDto = new MediaCreateDto
            {
                FileName = TestConstants.ValidFileName,
                Url = TestConstants.ValidMediaUrl,
                MediaType = MyCourse.Domain.Enums.MediaType.Image,
                ContentType = "image/jpg",
                Description = TestConstants.ValidDescription,
                FileSize = TestConstants.InvalidFileSize,
            };

            // Act & Assert
            await Assert.ThrowsAsync<MediaValidationException>(() => _mediaService.CreateMediaAsync(mediaDto));

        }

        [Fact]
        public async Task AddMediaToCourseAsync_WithValidData_ShouldAddMediaToCourse()
        {
            // Arrange
            var course = new Course
            {
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Now,
                CourseDuration = TestConstants.ValidCourseDuration,
                MaxParticipants = TestConstants.ValidMaxParticipants,
                Price = TestConstants.ValidPrice,
                Location = TestConstants.ValidLocation,
                IsActive = TestConstants.ValidIsActive
            };
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            var mediaDto = new MediaCreateDto
            {
                FileName = TestConstants.ValidFileName,
                Url = TestConstants.ValidMediaUrl,
                MediaType = MyCourse.Domain.Enums.MediaType.Image,
                ContentType = "image/jpg",
                Description = TestConstants.ValidDescription,
                FileSize = TestConstants.ValidFileSize,
            };

            // Act
            await _mediaService.AddMediaToCourseAsync(course.Id, mediaDto);

            // Assert
            var media = await _context.Medias.FirstOrDefaultAsync(m => m.FileName == mediaDto.FileName);
            Assert.NotNull(media);

            var courseMedia = await _context.CourseMedias.FindAsync(media.Id);
            Assert.NotNull(courseMedia);
        }

        [Fact]
        public async Task AddMediaToCourseAsync_WithInvalidMedia_ShouldThrowValidationException()
        {
            // Arrange
            var course = new Course
            {
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDate = DateTime.Now,
                CourseDuration = TestConstants.ValidCourseDuration,
                MaxParticipants = TestConstants.ValidMaxParticipants,
                Price = TestConstants.ValidPrice,
                Location = TestConstants.ValidLocation,
                IsActive = TestConstants.ValidIsActive
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            var mediaDto = new MediaCreateDto
            {
                FileName = TestConstants.InvalidFileName,
                Url = TestConstants.ValidMediaUrl,
                MediaType = MyCourse.Domain.Enums.MediaType.Image,
                ContentType = "image/jpg",
                Description = TestConstants.ValidDescription,
                FileSize = TestConstants.ValidFileSize,
            };

            // Act & Assert
            await Assert.ThrowsAsync<MediaValidationException>(() => _mediaService.AddMediaToCourseAsync(course.Id, mediaDto));
        }

    }
}
