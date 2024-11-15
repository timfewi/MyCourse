using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.Data.Repositories.CourseRepositories;
using MyCourse.Domain.Data.Repositories.MediaRepositories;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.BlogPostEx;
using MyCourse.Domain.Exceptions.MediaEx;
using MyCourse.Domain.Services.MediaServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyCourse.Domain.Exceptions.BlogPostEx.BlogPostExceptions;
using static MyCourse.Domain.Exceptions.CourseEx.CourseExceptions;

namespace MyCourse.Domain.Services.BlogPostServices
{
    [Injectable(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
    public class BlogPostService : IBlogPostService
    {
        private readonly AppDbContext _dbContext;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMediaService _mediaService;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogPostService> _logger;
        private readonly IValidator<BlogPostCreateDto> _createDtoValidator;
        private readonly IValidator<BlogPostMediaCreateDto> _createMediaDtoValidator;

        public BlogPostService(
            AppDbContext dbContext,
            IBlogPostRepository blogPostRepository,
            IMediaService mediaService,
            IMediaRepository mediaRepository,
            IMapper mapper,
            ILogger<BlogPostService> logger,
            IValidator<BlogPostCreateDto> createDtoValidator,
            IValidator<BlogPostMediaCreateDto> createMediaDtoValidator
            )
        {
            _dbContext = dbContext;
            _blogPostRepository = blogPostRepository;
            _mediaService = mediaService;
            _mediaRepository = mediaRepository;
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
            var blogPost = await _blogPostRepository.GetBlogPostByIdAsync(id);
            if (blogPost == null)
            {
                throw new BlogPostNotFoundException(id);
            }

            return _mapper.Map<BlogPostDetailDto>(blogPost);
        }

        public async Task<BlogPostEditWithImagesDto> GetBlogPostEditDetailsWithImagesAsync(int blogPostId)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);
            if (blogPost == null)
            {
                _logger.LogWarning("BlogPost with ID {BlogPostId} not found.", blogPostId);
                throw new BlogPostNotFoundException(blogPostId);
            }
            await _blogPostRepository.LoadBlogPostMediasAsync(blogPost);
            var dto = _mapper.Map<BlogPostEditWithImagesDto>(blogPost);

            dto.NewImages = new List<IFormFile>();
            dto.ExistingImages = blogPost.BlogPostMedias.Select(bpm => new BlogPostImageDto
            {
                MediaId = bpm.MediaId,
                Url = bpm.Media.Url,
                ToDelete = false
            }).ToList();

            return dto;
        }

        /// <summary>
        /// Erstellt einen neuen BlogPost.
        /// </summary>
        /// <param name="createDto">Daten für die Erstellung des BlogPosts</param>
        /// <returns>BlogPostDetailDto des erstellten BlogPosts</returns>
        /// <exception cref="BlogPostDuplicateTitleException">Wenn ein BlogPost mit dem gleichen Titel bereits existiert.</exception>
        /// <exception cref="BlogPostValidationException">Wenn die Validierung fehlschlägt.</exception>
        /// <exception cref="BlogPostInvalidOperationException">Wenn das Mapping fehlschlägt.</exception>
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
                if (blogPost == null)
                {
                    _logger.LogError("Mapping from BlogPostCreateDto to BlogPost failed");
                    throw new BlogPostInvalidOperationException("Mapping from BlogPostCreateDto to Entity Failed");
                }

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
        /// Aktualisiert einen bestehenden BlogPost einschließlich seiner Medien (Bilder) und Tags.
        /// </summary>
        /// <param name="blogPostDto">Das DTO mit den aktualisierten Daten des BlogPosts.</param>
        /// <exception cref="BlogPostNotFoundException">
        /// Wird ausgelöst, wenn kein BlogPost mit der angegebenen ID gefunden wird.
        /// </exception>
        /// <exception cref="BlogPostUpdateException">
        /// Wird ausgelöst, wenn beim Aktualisieren des BlogPosts ein Fehler auftritt.
        /// </exception>
        /// <exception cref="BlogPostDatabaseException">
        /// Wird ausgelöst, wenn beim Speichern des BlogPosts in der Datenbank ein Fehler auftritt.
        /// </exception>
        public async Task UpdateBlogPostWithImagesAsync(BlogPostEditWithImagesDto blogPostDto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var blogPost = await _blogPostRepository.GetByIdAsync(blogPostDto.BlogPostId);
                if (blogPost == null)
                {
                    _logger.LogWarning("BlogPost mit ID {BlogPostId} nicht gefunden.", blogPostDto.BlogPostId);
                    throw new BlogPostNotFoundException(blogPostDto.BlogPostId);
                }
                await _blogPostRepository.LoadBlogPostMediasAsync(blogPost);

                // BlogPost-Eigenschaften aktualisieren
                blogPost.Title = blogPostDto.Title;
                blogPost.Description = blogPostDto.Description;
                blogPost.IsPublished = blogPostDto.IsPublished;

                // Tags aktualisieren (falls erforderlich)
                if (blogPostDto.Tags != null)
                {
                    // Annahme: Es gibt eine Methode zum Aktualisieren der Tags
                    await _blogPostRepository.UpdateTagsAsync(blogPost, blogPostDto.Tags);
                }

                // Vorhandene Bilder behandeln
                if (blogPostDto.ExistingImages != null && blogPostDto.ExistingImages.Any())
                {
                    var imagesToDelete = blogPostDto.ExistingImages
                        .Where(ei => ei.ToDelete)
                        .Select(ei => ei.MediaId)
                        .ToList();

                    if (imagesToDelete.Any())
                    {
                        var mediasToDelete = blogPost.BlogPostMedias
                            .Where(bpm => imagesToDelete.Contains(bpm.MediaId))
                            .Select(bpm => bpm.Media)
                            .ToList();

                        foreach (var media in mediasToDelete)
                        {
                            if (media != null)
                            {
                                try
                                {
                                    _mediaRepository.DeleteImage(media);
                                    var blogPostMedia = blogPost.BlogPostMedias.FirstOrDefault(bpm => bpm.MediaId == media.Id);
                                    if (blogPostMedia != null)
                                    {
                                        _mediaRepository.RemoveBlogPostMedia(blogPostMedia);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("No BlogPostMedia found for MediaId {MediaId}.", media.Id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Fehler beim Löschen des Mediums mit ID {MediaId} während der Aktualisierung des BlogPosts.", media.Id);
                                    throw new BlogPostUpdateException("Fehler beim Löschen des Mediums während der Aktualisierung des BlogPosts.", blogPost.Id, ex);
                                }
                            }
                        }
                    }
                }

                // Neue Bilder hinzufügen
                if (blogPostDto.NewImages != null && blogPostDto.NewImages.Any())
                {
                    foreach (var image in blogPostDto.NewImages)
                    {
                        if (image != null && image.Length > 0)
                        {
                            try
                            {
                                // Bild speichern und Media-Entity erhalten
                                var media = await _mediaRepository.SaveImageAsync(image, blogPost.Id);

                                // Medium zum BlogPost hinzufügen
                                await _mediaRepository.AddBlogPostMediaAsync(blogPost.Id, media.Id);
                            }
                            catch (MediaException ex)
                            {
                                _logger.LogError(ex, "Fehler beim Hinzufügen eines neuen Bildes zum BlogPost mit ID {BlogPostId}.", blogPost.Id);
                                throw;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Unerwarteter Fehler beim Hinzufügen eines neuen Bildes zum BlogPost mit ID {BlogPostId}.", blogPost.Id);
                                throw new BlogPostUpdateException("Fehler beim Hinzufügen neuer Bilder zum BlogPost.", blogPost.Id, ex);
                            }
                        }
                    }
                }

                _blogPostRepository.Update(blogPost);

                try
                {
                    await _blogPostRepository.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("BlogPost mit ID {BlogPostId} erfolgreich aktualisiert.", blogPost.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler beim Speichern des BlogPosts mit ID {BlogPostId}.", blogPost.Id);
                    throw new BlogPostDatabaseException("Fehler beim Speichern des BlogPosts.", blogPost.Id, ex);
                }
            }
            catch (Exception ex)
            {
                // Rollback der Transaktion bei Fehlern
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Fehler beim Updaten des neuen BlogPosts.");
                if (ex is BlogPostValidationException || ex is BlogPostDuplicateTitleException || ex is BlogPostUpdateException)
                {
                    throw;
                }
                throw new BlogPostDatabaseException("Failed to save the new blog post.", null, ex.Message);
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
                foreach (var blogPostMedia in blogPost.BlogPostMedias.ToList())
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

        public async Task ToggleBlogPostStatusAsync(int blogPostId)
        {
            var blogPost = await _blogPostRepository.GetBlogPostByIdAsync(blogPostId);
            if (blogPost == null)
            {
                _logger.LogWarning("BlogPost with ID {BlogPostId} not found for status toggle.", blogPostId);
                throw new CourseNotFoundException(blogPostId);
            }

            blogPost.IsPublished = !blogPost.IsPublished;
            blogPost.DateUpdated = DateTime.Now;
            _blogPostRepository.UpdateBlogPost(blogPost);

            try
            {
                await _blogPostRepository.SaveChangesAsync();
                _logger.LogInformation("BlogPost '{BlogPostTitle}' status toggled to {Status}.", blogPost.Title, blogPost.IsPublished ? "Veröffentlicht" : "Unveröffentlicht");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling status for BlogPost with ID {BlogPostId}.", blogPostId);
                throw new CourseSaveException("An error occurred while toggling blog status.", blogPostId, ex);
            }
        }
    }
}
