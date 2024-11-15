using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.MediaEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Services.MediaServices
{
    [Injectable(ServiceLifetime.Scoped)]
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly ILogger<MediaService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<MediaCreateDto> _mediaCreateValidator;

        public MediaService(
            IMediaRepository mediaRepository,
            ILogger<MediaService> logger,
            IMapper mapper,
            IValidator<MediaCreateDto> mediaCreateValidator
            )
        {
            _mediaRepository = mediaRepository;
            _logger = logger;
            _mapper = mapper;
            _mediaCreateValidator = mediaCreateValidator;
        }

        public async Task AddMediaToCourseAsync(int courseId, MediaCreateDto mediaDto)
        {
            try
            {
                var mediaId = await CreateMediaAsync(mediaDto);
                await _mediaRepository.AddCourseMediaAsync(courseId, mediaId);
                await _mediaRepository.SaveChangesAsync();
            }
            catch (MediaValidationException)
            {
                throw;
            }
            catch (InvalidMediaOperationException ex)
            {
                _logger.LogError(ex, "An error occurred while adding media to course.");
                throw new MediaException(MediaErrorCode.InvalidOperation, "Failed to add media to course.", ex.MediaId, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                throw new MediaException(MediaErrorCode.InvalidOperation, "An unexpected error occurred.", null, ex);
            }
        }

        public async Task AddMediaToBlogPostAsync(int blogPostId, MediaCreateDto mediaDto)
        {
            try
            {
                var mediaId = await CreateMediaAsync(mediaDto);
                await _mediaRepository.AddBlogPostMediaAsync(blogPostId, mediaId);
                await _mediaRepository.SaveChangesAsync();
            }
            catch (MediaValidationException)
            {
                throw;
            }
            catch (InvalidMediaOperationException ex)
            {
                _logger.LogError(ex, "An error occurred while adding media to blogPost.");
                throw new MediaException(MediaErrorCode.InvalidOperation, "Failed to add media to blogPost.", ex.MediaId, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                throw new MediaException(MediaErrorCode.InvalidOperation, "An unexpected error occurred.", null, ex);
            }
        }

        public async Task<int> CreateMediaAsync(MediaCreateDto mediaDto)
        {
            try
            {
                var validationResult = await _mediaCreateValidator.ValidateAsync(mediaDto);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation errors for MediaCreateDto: {Errors}", validationResult.Errors);
                    throw new MediaValidationException(validationResult.Errors);
                }

                var media = _mapper.Map<Media>(mediaDto);
                await _mediaRepository.AddAsync(media);
                await _mediaRepository.SaveChangesAsync();

                return media.Id;
            }
            catch (MediaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating media.");
                throw new MediaSaveException("An error occurred while saving media.", null, ex);
            }
        }

        public async Task DeleteMediaAsync(int mediaId)
        {
            var media = await _mediaRepository.GetByIdAsync(mediaId);
            if (media == null)
            {
                throw new MediaNotFoundException(mediaId);
            }

            try
            {
                _mediaRepository.DeleteImage(media);
                await _mediaRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fehler beim Löschen des Media mit ID {mediaId}.");
                throw new MediaDatabaseException("Failed to delete the media.", mediaId, ex.Message);
            }
        }

        public async Task UpdateMediaAsync(int mediaId, MediaCreateDto mediaDto)
        {
            var media = await _mediaRepository.GetByIdAsync(mediaId);
            if (media == null)
            {
                throw new MediaNotFoundException(mediaId);
            }

            try
            {
                var validationResult = await _mediaCreateValidator.ValidateAsync(mediaDto);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation errors for MediaCreateDto: {Errors}", validationResult.Errors);
                    throw new MediaValidationException(validationResult.Errors);
                }

                _mapper.Map(mediaDto, media);
                _mediaRepository.Update(media);
                await _mediaRepository.SaveChangesAsync();
            }
            catch (MediaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when updating media with ID {mediaId}.");
                throw new MediaDatabaseException("An error occurred while updating media.", mediaId, ex.Message);
            }
        }
    }
}
