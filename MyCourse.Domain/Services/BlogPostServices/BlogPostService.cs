using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.BlogPostEx;
using MyCourse.Domain.Services.MediaServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyCourse.Domain.Exceptions.BlogPostEx.BlogPostExceptions;

namespace MyCourse.Domain.Services.BlogPostServices
{
    [Injectable(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
    public class BlogPostService : IBlogPostService
    {
        private readonly AppDbContext _dbContext;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMediaService _mediaService;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogPostService> _logger;
        private readonly IValidator<BlogPostCreateDto> _createDtoValidator;
        private readonly IValidator<BlogPostMediaCreateDto> _createMediaDtoValidator;

        public BlogPostService(
            AppDbContext dbContext,
            IBlogPostRepository blogPostRepository,
            IMediaService mediaService,
            IMapper mapper,
            ILogger<BlogPostService> logger,
            IValidator<BlogPostCreateDto> createDtoValidator,
            IValidator<BlogPostMediaCreateDto> createMediaDtoValidator
            )
        {
            _dbContext = dbContext;
            _blogPostRepository = blogPostRepository;
            _mediaService = mediaService;
            _mapper = mapper;
            _logger = logger;
            _createDtoValidator = createDtoValidator;
            _createMediaDtoValidator = createMediaDtoValidator;
        }

        /// <summary>
        /// Ruft alle BlogPosts ab.
        /// </summary>
        /// <returns>Liste von BlogPostListDto</returns>
        public async Task<IEnumerable<BlogPostListDto>> GetAllBlogPostsAsync()
        {
            return await _blogPostRepository
                .GetAllBlogPostsQuery()
                .ProjectTo<BlogPostListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        /// <summary>
        /// Ruft alle veröffentlichten BlogPosts ab.
        /// </summary>
        /// <returns>Liste von BlogPostListDto</returns>
        public async Task<IEnumerable<BlogPostListDto>> GetPublishedBlogPostsAsync()
        {
            return await _blogPostRepository
                .GetAllPublishedPostsQuery()
                .ProjectTo<BlogPostListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }


        /// <summary>
        /// Ruft detaillierte Informationen eines bestimmten BlogPosts ab.
        /// </summary>
        /// <param name="id">ID des BlogPosts</param>
        /// <returns>BlogPostDetailDto</returns>
        /// <exception cref="BlogPostNotFoundException">Wenn der BlogPost nicht gefunden wird.</exception>
        public async Task<BlogPostDetailDto> GetBlogPostDetailAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null || !blogPost.IsPublished)
            {
                throw new BlogPostNotFoundException(id);
            }

            return _mapper.Map<BlogPostDetailDto>(blogPost);
        }


        /// <summary>
        /// Erstellt einen neuen BlogPost.
        /// </summary>
        /// <param name="createDto">Daten für die Erstellung des BlogPosts</param>
        /// <returns>BlogPostDetailDto des erstellten BlogPosts</returns>
        /// <exception cref="BlogPostDuplicateTitleException">Wenn ein BlogPost mit dem gleichen Titel bereits existiert.</exception>
        /// <exception cref="BlogPostValidationException">Wenn die Validierung fehlschlägt.</exception>
        /// <exception cref="BlogPostDatabaseException">Wenn ein Datenbankfehler auftritt.</exception>
        public async Task<BlogPostDetailDto> CreateBlogPostAsync(BlogPostCreateDto createDto)
        {
            // Starten einer neuen Transaktion
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Validierung des CreateDto
                var validationResult = await _createDtoValidator.ValidateAsync(createDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    throw new BlogPostValidationException("Validation failed.", null, errors);
                }

                // Prüfen auf doppelten Titel
                var existingPost = await _blogPostRepository.GetBlogPostByTitleAsync(createDto.Title);
                if (existingPost != null)
                {
                    throw new BlogPostDuplicateTitleException(createDto.Title);
                }

                // Mappen des DTOs auf die Entity
                var blogPost = _mapper.Map<BlogPost>(createDto);

                // Hinzufügen der zugehörigen Medien über MediaService
                foreach (var mediaDto in createDto.Medias)
                {
                    // Umwandeln von BlogPostMediaCreateDto zu MediaCreateDto
                    var mediaCreateDto = _mapper.Map<MediaCreateDto>(mediaDto);

                    // Erstelle das Medium und erhalte die MediaId
                    var mediaId = await _mediaService.CreateMediaAsync(mediaCreateDto);

                    var blogPostMedia = new BlogPostMedia
                    {
                        MediaId = mediaId,
                        Order = blogPost.BlogPostMedias.Count + 1,
                        Caption = mediaDto.Caption
                    };

                    blogPost.BlogPostMedias.Add(blogPostMedia);
                }

                // Hinzufügen des BlogPosts
                await _blogPostRepository.AddAsync(blogPost);

                // Speichern der Änderungen
                await _dbContext.SaveChangesAsync();

                // Commit der Transaktion
                await transaction.CommitAsync();

                return _mapper.Map<BlogPostDetailDto>(blogPost);
            }
            catch (Exception ex)
            {
                // Rollback der Transaktion bei Fehlern
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Fehler beim Erstellen des neuen BlogPosts.");
                if (ex is BlogPostValidationException || ex is BlogPostDuplicateTitleException)
                {
                    throw;
                }
                throw new BlogPostDatabaseException("Failed to save the new blog post.", null, ex.Message);
            }
        }


        /// <summary>
        /// Aktualisiert einen bestehenden BlogPost.
        /// </summary>
        /// <param name="id">ID des BlogPosts</param>
        /// <param name="updateDto">Aktualisierte Daten des BlogPosts</param>
        /// <returns>BlogPostDetailDto des aktualisierten BlogPosts</returns>
        /// <exception cref="BlogPostNotFoundException">Wenn der BlogPost nicht gefunden wird.</exception>
        /// <exception cref="BlogPostDuplicateTitleException">Wenn ein BlogPost mit dem gleichen Titel bereits existiert.</exception>
        /// <exception cref="BlogPostValidationException">Wenn die Validierung fehlschlägt.</exception>
        /// <exception cref="BlogPostDatabaseException">Wenn ein Datenbankfehler auftritt.</exception>
        public async Task<BlogPostDetailDto> UpdateBlogPostAsync(int id, BlogPostCreateDto updateDto)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
            {
                _logger.LogWarning("BlogPost not Found with ID: {id}", id);
                throw new BlogPostNotFoundException(id);
            }
            // Starten einer neuen Transaktion
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Validierung des UpdateDto
                var validationResult = await _createDtoValidator.ValidateAsync(updateDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    throw new BlogPostValidationException("Validation failed.", id, errors);
                }

                // Prüfen auf doppelten Titel, falls der Titel geändert wurde
                if (!string.Equals(blogPost.Title, updateDto.Title, StringComparison.OrdinalIgnoreCase))
                {
                    var existingPost = await _blogPostRepository.GetBlogPostByTitleAsync(updateDto.Title);
                    if (existingPost != null && existingPost.Id != id)
                    {
                        throw new BlogPostDuplicateTitleException(updateDto.Title, id);
                    }
                }

                // Mappen der aktualisierten Daten auf die Entity
                _mapper.Map(updateDto, blogPost);

                // Verwalten der Medien:
                // 1. Ermitteln der bestehenden Medien
                var existingBlogPostMedias = blogPost.BlogPostMedias.ToList();

                // 2. Ermitteln der neuen Medien aus dem updateDto
                var updatedMediaDtos = updateDto.Medias;

                // 3. Ermitteln der Medien, die gelöscht werden müssen (basierend auf URL)
                var mediasToDelete = existingBlogPostMedias
                    .Where(existingMedia => !updatedMediaDtos.Any(u => string.Equals(u.Url, existingMedia.Media.Url, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                // 4. Löschen der nicht mehr benötigten Medien über MediaService
                foreach (var mediaToDelete in mediasToDelete)
                {
                    blogPost.BlogPostMedias.Remove(mediaToDelete);
                    await _mediaService.DeleteMediaAsync(mediaToDelete.MediaId);
                }

                // 5. Hinzufügen oder Aktualisieren der Medien
                foreach (var mediaDto in updatedMediaDtos)
                {
                    // Prüfen, ob das Media bereits existiert (basierend auf URL)
                    var existingMedia = existingBlogPostMedias.FirstOrDefault(m => string.Equals(m.Media.Url, mediaDto.Url, StringComparison.OrdinalIgnoreCase));

                    if (existingMedia != null)
                    {
                        // Mappe BlogPostMediaCreateDto zu MediaCreateDto
                        var mediaUpdateDto = _mapper.Map<MediaCreateDto>(mediaDto);

                        // Aktualisiere das Medium über MediaService
                        await _mediaService.UpdateMediaAsync(existingMedia.MediaId, mediaUpdateDto);

                        existingMedia.Caption = mediaDto.Caption;
                    }
                    else
                    {
                        // Mappe BlogPostMediaCreateDto zu MediaCreateDto
                        var mediaCreateDto = _mapper.Map<MediaCreateDto>(mediaDto);

                        // Erstelle das Medium und erhalte die MediaId
                        var mediaId = await _mediaService.CreateMediaAsync(mediaCreateDto);

                        var blogPostMedia = new BlogPostMedia
                        {
                            MediaId = mediaId, // Verwende die zurückgegebene MediaId direkt
                            Order = blogPost.BlogPostMedias.Count + 1,
                            Caption = mediaDto.Caption
                        };

                        blogPost.BlogPostMedias.Add(blogPostMedia);
                    }
                }

                // Aktualisieren des BlogPosts
                _blogPostRepository.Update(blogPost);

                // Speichern der Änderungen
                await _dbContext.SaveChangesAsync();

                // Commit der Transaktion
                await transaction.CommitAsync();

                return _mapper.Map<BlogPostDetailDto>(blogPost);
            }
            catch (BlogPostNotFoundException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Fehler beim Aktualisieren des BlogPosts mit ID {id}.");
                if (ex is BlogPostValidationException || ex is BlogPostDuplicateTitleException)
                {
                    throw;
                }
                throw new BlogPostDatabaseException("Failed to update the blog post.", id, ex.Message);
            }
        }

        /// <summary>
        /// Löscht einen bestehenden BlogPost.
        /// </summary>
        /// <param name="id">ID des BlogPosts</param>
        /// <returns>Task</returns>
        /// <exception cref="BlogPostNotFoundException">Wenn der BlogPost nicht gefunden wird.</exception>
        /// <exception cref="BlogPostInvalidOperationException">Wenn der BlogPost nicht gelöscht werden kann.</exception>
        /// <exception cref="BlogPostDatabaseException">Wenn ein Datenbankfehler auftritt.</exception>
        public async Task DeleteBlogPostAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
            {
                throw new BlogPostExceptions.BlogPostNotFoundException(id);
            }

            try
            {
                foreach(var blogPostMedia in blogPost.BlogPostMedias.ToList())
                {
                    _blogPostRepository.RemoveBlogPostMedia(blogPostMedia);
                    await _mediaService.DeleteMediaAsync(blogPostMedia.MediaId);
                }

                _blogPostRepository.DeleteBlogPost(blogPost);
                await _blogPostRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fehler beim Löschen des BlogPosts mit ID {id}.");
                throw new BlogPostExceptions.BlogPostDatabaseException("Failed to delete the blog post.", id, ex.Message);
            }
        }
    }
}
