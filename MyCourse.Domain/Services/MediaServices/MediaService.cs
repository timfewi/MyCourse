using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
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
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validierungsfehler beim Hinzufügen von Media zu Kurs: {Errors}", ex.Errors);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Hinzufügen von Media zu Kurs.");
                throw;
            }
        }

        public async Task<int> CreateMediaAsync(MediaCreateDto mediaDto)
        {
            try
            {

                var validationResult = await _mediaCreateValidator.ValidateAsync(mediaDto);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validierungsfehler bei MediaCreateDto: {Errors}", validationResult.Errors);
                    throw new ValidationException(validationResult.Errors);
                }

                var media = _mapper.Map<Media>(mediaDto);

                await _mediaRepository.AddAsync(media);
                await _mediaRepository.SaveChangesAsync();

                return media.Id;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen von Media.");
                throw;
            }
        }

    }
}
