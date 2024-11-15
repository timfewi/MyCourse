using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Repositories.MediaRepositories
{
    [Injectable(ServiceLifetime.Scoped)]
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<MediaRepository> _logger;

        public MediaRepository(
            AppDbContext dbContext,
            IWebHostEnvironment webHostEnvironment,
            ILogger<MediaRepository> logger
            ) : base(dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task AddCourseMediaAsync(int courseId, int mediaId)
        {
            var exists = await _dbContext.CourseMedias
                .AnyAsync(cm => cm.CourseId == courseId && cm.MediaId == mediaId);

            if (!exists)
            {
                var courseMedia = new CourseMedia
                {
                    CourseId = courseId,
                    MediaId = mediaId
                };

                await _dbContext.CourseMedias.AddAsync(courseMedia);
            }
        }

        public async Task AddBlogPostMediaAsync(int blogPostId, int mediaId)
        {
            var exists = await _dbContext.BlogPostMedias
                .AnyAsync(bpm => bpm.BlogPostId == blogPostId && bpm.MediaId == mediaId);

            if (!exists)
            {
                var blogPostMedia = new BlogPostMedia
                {
                    BlogPostId = blogPostId,
                    MediaId = mediaId
                };

                await _dbContext.BlogPostMedias.AddAsync(blogPostMedia);
            }
        }

        public void RemoveCourseMedia(CourseMedia courseMedia)
        {
            _dbContext.CourseMedias.Remove(courseMedia);
        }

        public void RemoveBlogPostMedia(BlogPostMedia blogPostMedia)
        {
            _dbContext.BlogPostMedias.Remove(blogPostMedia);
        }


        public async Task<Media> SaveImageAsync(IFormFile file, int courseId)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "images", courseId.ToString());
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
            if (!allowedMimeTypes.Contains(file.ContentType))
            {
                throw new InvalidOperationException("Ungültiger MIME-Typ für das Bild.");
            }

            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/images/{courseId}/{uniqueFileName}";

            var media = new Media
            {
                Url = imageUrl,
                FileName = Path.GetFileName(file.FileName),
                MediaType = Domain.Enums.MediaType.Image,
                ContentType = file.ContentType,
                Description = string.Empty,
                FileSize = file.Length
            };

            await _dbContext.Medias.AddAsync(media);
            await _dbContext.SaveChangesAsync();

            return media;
        }

        public void DeleteImage(Media media)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, media.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Deleted image at path: {FilePath}", filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting image at path: {FilePath}", filePath);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent image at path: {FilePath}", filePath);
            }

            _dbContext.Medias.Remove(media);
        }

    }
}
