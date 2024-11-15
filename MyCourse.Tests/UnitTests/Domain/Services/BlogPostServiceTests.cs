using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyCourse.Domain.Data;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using MyCourse.Domain.DTOs.MediaDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using MyCourse.Domain.Exceptions.BlogPostEx;
using MyCourse.Domain.Services.BlogPostServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static MyCourse.Domain.Exceptions.BlogPostEx.BlogPostExceptions;

namespace MyCourse.Domain.Tests.Services
{
    public class BlogPostServiceTests : TestBase
    {
        private readonly Mock<IBlogPostRepository> _blogPostRepoMock;
        private readonly Mock<IMediaService> _mediaServiceMock;
        private readonly Mock<IMediaRepository> _mediaRepositoryMock;
        private readonly Mock<IValidator<BlogPostCreateDto>> _createDtoValidatorMock;
        private readonly Mock<IValidator<BlogPostMediaCreateDto>> _createMediaDtoValidatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BlogPostService>> _loggerMock;

        private readonly BlogPostService _service;

        public BlogPostServiceTests()
            : base()
        {
            // Erstellen der Mock-Objekte
            _blogPostRepoMock = new Mock<IBlogPostRepository>();
            _mediaServiceMock = new Mock<IMediaService>();
            _mediaRepositoryMock = new Mock<IMediaRepository>();
            _createDtoValidatorMock = new Mock<IValidator<BlogPostCreateDto>>();
            _createMediaDtoValidatorMock = new Mock<IValidator<BlogPostMediaCreateDto>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BlogPostService>>();

            // Registrieren der Mock-Objekte in der ServiceCollection
            _services.AddSingleton(_blogPostRepoMock.Object);
            _services.AddSingleton(_mediaServiceMock.Object);
            _services.AddSingleton(_createDtoValidatorMock.Object);
            _services.AddSingleton(_createMediaDtoValidatorMock.Object);
            _services.AddSingleton(_mapperMock.Object);
            _services.AddSingleton(_loggerMock.Object);

            // Neubau des ServiceProviders mit den gemockten Objekten
            _serviceProvider = _services.BuildServiceProvider();

            // Erstellen des BlogPostService mit den gemockten Abhängigkeiten
            _service = new BlogPostService(
                _context,
                _blogPostRepoMock.Object,
                _mediaServiceMock.Object,
                _mediaRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _createDtoValidatorMock.Object,
                _createMediaDtoValidatorMock.Object
            );
        }

        #region CreateBlogPostAsync Tests

        [Fact]
        public async Task CreateBlogPostAsync_ValidInput_CreatesBlogPost()
        {
            // Arrange
            var createDto = new BlogPostCreateDto
            {
                Title = "Test Blog Post",
                Description = "This is a test blog post description.",
                Tags = new List<string> { "Test", "Blog" },
                IsPublished = true,
                Medias = new List<BlogPostMediaCreateDto>
                {
                    new BlogPostMediaCreateDto
                    {
                        Url = "http://example.com/image1.jpg",
                        FileName = "image1.jpg",
                        MediaType = MediaType.Image,
                        ContentType = "image/jpeg",
                        Description = "Image 1",
                        FileSize = 1024,
                    }
                }
            };

            // Setup Validator: Validierung besteht
            _createDtoValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup Repository: Kein BlogPost mit dem gleichen Titel existiert
            _blogPostRepoMock.Setup(repo => repo.GetBlogPostByTitleAsync("Test Blog Post"))
                .ReturnsAsync((BlogPost)null!);

            // Setup Mapping: Map BlogPostCreateDto zu BlogPost
            var blogPostEntity = new BlogPost { Id = 1, Title = "Test Blog Post", BlogPostMedias = new List<BlogPostMedia>() };
            _mapperMock.Setup(m => m.Map<BlogPost>(createDto)).Returns(blogPostEntity);

            // Setup MediaService: Erstelle Media und gebe MediaId zurück
            _mapperMock.Setup(m => m.Map<MediaCreateDto>(It.IsAny<BlogPostMediaCreateDto>()))
                .Returns((BlogPostMediaCreateDto mediaDto) => new MediaCreateDto
                {
                    Url = mediaDto.Url,
                    FileName = mediaDto.FileName,
                    MediaType = mediaDto.MediaType,
                    ContentType = mediaDto.ContentType,
                    Description = mediaDto.Description,
                    FileSize = mediaDto.FileSize
                });

            _mediaServiceMock.Setup(m => m.CreateMediaAsync(It.IsAny<MediaCreateDto>()))
                .ReturnsAsync(1); // MediaId = 1

            // Setup Repository: AddAsync soll den BlogPost hinzufügen
            _blogPostRepoMock.Setup(repo => repo.AddAsync(It.IsAny<BlogPost>()))
                .Callback<BlogPost>(bp => _context.BlogPosts.Add(bp))
                .Returns(Task.CompletedTask);

            // Setup SaveChangesAsync
            _context.SaveChangesAsync().GetAwaiter();

            // Setup Mapping: Map BlogPost zu BlogPostDetailDto
            var blogPostDetailDto = new BlogPostDetailDto
            {
                Id = 1,
                Title = "Test Blog Post",
                Description = "This is a test blog post description.",
                Tags = new List<string> { "Test", "Blog" },
                IsPublished = true,
                Medias = new List<BlogPostMediaDetailDto>
                {
                    new BlogPostMediaDetailDto
                    {
                        MediaId = 1,
                        Url = "http://example.com/image1.jpg",
                        FileName = "image1.jpg",
                        MediaType = MediaType.Image,
                        ContentType = "image/jpeg",
                        Description = "Image 1",
                        FileSize = 1024,
                        Caption = "Caption 1",
                        Order = 1
                    }
                }
            };

            _mapperMock.Setup(m => m.Map<BlogPostDetailDto>(blogPostEntity))
                .Returns(blogPostDetailDto);

            // Act
            var result = await _service.CreateBlogPostAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Blog Post");
            result.Medias.Should().HaveCount(1);
            result.Medias.First().MediaId.Should().Be(1);

            _createDtoValidatorMock.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.GetBlogPostByTitleAsync("Test Blog Post"), Times.Once);
            _mapperMock.Verify(m => m.Map<BlogPost>(createDto), Times.Once);
            _mapperMock.Verify(m => m.Map<MediaCreateDto>(It.IsAny<BlogPostMediaCreateDto>()), Times.Once);
            _mediaServiceMock.Verify(m => m.CreateMediaAsync(It.IsAny<MediaCreateDto>()), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.AddAsync(blogPostEntity), Times.Once);
            _mapperMock.Verify(m => m.Map<BlogPostDetailDto>(blogPostEntity), Times.Once);
        }

        [Fact]
        public async Task CreateBlogPostAsync_DuplicateTitle_ThrowsException()
        {
            // Arrange
            var createDto = new BlogPostCreateDto
            {
                Title = "Existing Blog Post",
                Description = "Description",
                Tags = new List<string> { "Tag1" },
                IsPublished = true,
                Medias = new List<BlogPostMediaCreateDto>()
            };

            // Setup Validator: Validierung besteht
            _createDtoValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup Repository: Ein BlogPost mit dem gleichen Titel existiert
            var existingBlogPost = new BlogPost { Id = 2, Title = "Existing Blog Post" };
            _blogPostRepoMock.Setup(repo => repo.GetBlogPostByTitleAsync("Existing Blog Post"))
                .ReturnsAsync(existingBlogPost);

            // Act & Assert
            await Assert.ThrowsAsync<BlogPostDuplicateTitleException>(() => _service.CreateBlogPostAsync(createDto));

            _createDtoValidatorMock.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.GetBlogPostByTitleAsync("Existing Blog Post"), Times.Once);
            _mediaServiceMock.Verify(m => m.CreateMediaAsync(It.IsAny<MediaCreateDto>()), Times.Never);
            _blogPostRepoMock.Verify(repo => repo.AddAsync(It.IsAny<BlogPost>()), Times.Never);
        }

        [Fact]
        public async Task CreateBlogPostAsync_InvalidInput_ThrowsValidationException()
        {
            // Arrange
            var createDto = new BlogPostCreateDto
            {
                Title = "", // Ungültiger Titel
                Description = "Description",
                Tags = new List<string>(),
                IsPublished = true,
                Medias = new List<BlogPostMediaCreateDto>()
            };

            // Setup Validator: Validierung fehlschlägt
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title cannot be empty.")
            };
            var validationResult = new ValidationResult(failures);
            _createDtoValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BlogPostValidationException>(() => _service.CreateBlogPostAsync(createDto));
            exception.Message.Should().Be("Validation failed.");
            exception.BlogPostId.Should().BeNull();
           

            _createDtoValidatorMock.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.GetBlogPostByTitleAsync(It.IsAny<string>()), Times.Never);
            _mediaServiceMock.Verify(m => m.CreateMediaAsync(It.IsAny<MediaCreateDto>()), Times.Never);
            _blogPostRepoMock.Verify(repo => repo.AddAsync(It.IsAny<BlogPost>()), Times.Never);
        }

        #endregion

        #region GetBlogPostDetailAsync Tests

        [Fact]
        public async Task GetBlogPostDetailAsync_ExistingId_ReturnsBlogPostDetailDto()
        {
            // Arrange
            int blogPostId = 1;
            var blogPost = new BlogPost
            {
                Id = blogPostId,
                Title = "Test Blog Post",
                Description = "Detailed description",
                IsPublished = true,
                BlogPostMedias = new List<BlogPostMedia>
                {
                    new BlogPostMedia
                    {
                        MediaId = 1,
                        Media = new Media
                        {
                            Id = 1,
                            Url = "http://example.com/image1.jpg",
                            FileName = "image1.jpg",
                            MediaType = MediaType.Image,
                            ContentType = "image/jpeg",
                            Description = "Image 1",
                            FileSize = 1024
                        },
                        Order = 1
                    }
                }
            };

            _blogPostRepoMock.Setup(repo => repo.GetBlogPostByIdAsync(blogPostId))
                .ReturnsAsync(blogPost);

            var blogPostDetailDto = new BlogPostDetailDto
            {
                Id = blogPostId,
                Title = "Test Blog Post",
                Description = "Detailed description",
                Tags = new List<string>(),
                IsPublished = true,
                Medias = new List<BlogPostMediaDetailDto>
                {
                    new BlogPostMediaDetailDto
                    {
                        MediaId = 1,
                        Url = "http://example.com/image1.jpg",
                        FileName = "image1.jpg",
                        MediaType = MediaType.Image,
                        ContentType = "image/jpeg",
                        Description = "Image 1",
                        FileSize = 1024,
                        Order = 1
                    }
                }
            };

            _mapperMock.Setup(m => m.Map<BlogPostDetailDto>(blogPost))
                .Returns(blogPostDetailDto);

            // Act
            var result = await _service.GetBlogPostDetailAsync(blogPostId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(blogPostId);
            result.Title.Should().Be("Test Blog Post");
            result.Medias.Should().HaveCount(1);
            result.Medias.First().MediaId.Should().Be(1);

            _blogPostRepoMock.Verify(repo => repo.GetBlogPostByIdAsync(blogPostId), Times.Once);
            _mapperMock.Verify(m => m.Map<BlogPostDetailDto>(blogPost), Times.Once);
        }

        [Fact]
        public async Task GetBlogPostDetailAsync_NonExistingId_ThrowsBlogPostNotFoundException()
        {
            // Arrange
            int blogPostId = 99;

            _blogPostRepoMock.Setup(repo => repo.GetBlogPostByIdAsync(blogPostId))
                .ReturnsAsync((BlogPost)null!);

            // Act
            Func<Task> act = async () => await _service.GetBlogPostDetailAsync(blogPostId);

            // Assert
            var exception = await Assert.ThrowsAsync<BlogPostNotFoundException>(act);
            exception.ErrorCode.Should().Be(BlogPostErrorCode.NotFound);
            exception.Message.Should().Be($"BlogPost with ID {blogPostId} not found.");
            exception.BlogPostId.Should().Be(blogPostId);

            _blogPostRepoMock.Verify(repo => repo.GetBlogPostByIdAsync(blogPostId), Times.Once);
            _mapperMock.Verify(m => m.Map<BlogPostDetailDto>(It.IsAny<BlogPost>()), Times.Never);
        }

        #endregion

        #region UpdateBlogPostAsync Tests

        [Fact]
        public async Task UpdateBlogPostWithImagesAsync_ValidInput_UpdatesBlogPost()
        {
            // Arrange
            int blogPostId = 1;
            var updateDto = new BlogPostEditWithImagesDto
            {
                BlogPostId = blogPostId,
                Title = "Updated Blog Post",
                Description = "Updated description",
                Tags = new List<string> { "UpdatedTag" },
                IsPublished = true,
                ExistingImages = new List<BlogPostImageDto>
        {
            new BlogPostImageDto
            {
                MediaId = 1,
                Url = "http://example.com/image1.jpg",
                ToDelete = false
            },
            new BlogPostImageDto
            {
                MediaId = 2,
                Url = "http://example.com/image2.jpg",
                ToDelete = true
            }
        },
                NewImages = new List<IFormFile>
        {
            GetMockFormFile("image3.jpg"),
        }
            };

            var existingBlogPost = new BlogPost
            {
                Id = blogPostId,
                Title = "Original Blog Post",
                Description = "Original description",
                IsPublished = false,
                BlogPostMedias = new List<BlogPostMedia>
        {
            new BlogPostMedia
            {
                MediaId = 1,
                Media = new Media
                {
                    Id = 1,
                    Url = "http://example.com/image1.jpg",
                    FileName = "image1.jpg",
                    MediaType = MediaType.Image,
                    ContentType = "image/jpeg",
                    Description = "Image 1",
                    FileSize = 1024
                },
                Order = 1
            },
            new BlogPostMedia
            {
                MediaId = 2,
                Media = new Media
                {
                    Id = 2,
                    Url = "http://example.com/image2.jpg",
                    FileName = "image2.jpg",
                    MediaType = MediaType.Image,
                    ContentType = "image/jpeg",
                    Description = "Image 2",
                    FileSize = 2048
                },
                Order = 2
            }
        }
            };

            _blogPostRepoMock.Setup(repo => repo.GetByIdAsync(blogPostId))
                .ReturnsAsync(existingBlogPost);

            _blogPostRepoMock.Setup(repo => repo.LoadBlogPostMediasAsync(existingBlogPost))
                .Returns(Task.CompletedTask);

            _blogPostRepoMock.Setup(repo => repo.UpdateTagsAsync(existingBlogPost, updateDto.Tags))
                .Returns(Task.CompletedTask);

            _mediaRepositoryMock.Setup(mr => mr.DeleteImage(It.IsAny<Media>()))
             .Callback<Media>(media =>
             {
                 // Simulieren Sie das Löschen der Datei
             });

            _mediaRepositoryMock.Setup(mr => mr.RemoveBlogPostMedia(It.IsAny<BlogPostMedia>()))
                .Callback<BlogPostMedia>(bpm => existingBlogPost.BlogPostMedias.Remove(bpm));

            _mediaRepositoryMock.Setup(mr => mr.SaveImageAsync(It.IsAny<IFormFile>(), blogPostId))
                .ReturnsAsync(new Media { Id = 3, Url = "http://example.com/image3.jpg" });

            _mediaRepositoryMock.Setup(mr => mr.AddBlogPostMediaAsync(blogPostId, 3))
                .Returns(Task.CompletedTask)
                .Callback<int, int>((bpId, mediaId) =>
                {
                    existingBlogPost.BlogPostMedias.Add(new BlogPostMedia
                    {
                        MediaId = mediaId,
                        Media = new Media { Id = mediaId },
                        Order = existingBlogPost.BlogPostMedias.Count + 1
                    });
                });

            _blogPostRepoMock.Setup(repo => repo.Update(existingBlogPost))
                .Verifiable();

            _blogPostRepoMock.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateBlogPostWithImagesAsync(updateDto);

            // Assert
            // Überprüfen der aktualisierten Eigenschaften
            existingBlogPost.Title.Should().Be(updateDto.Title);
            existingBlogPost.Description.Should().Be(updateDto.Description);
            existingBlogPost.IsPublished.Should().Be(updateDto.IsPublished);

            // Überprüfen, dass das Tag-Update aufgerufen wurde
            _blogPostRepoMock.Verify(repo => repo.UpdateTagsAsync(existingBlogPost, updateDto.Tags), Times.Once);

            // Überprüfen, dass das zu löschende Bild entfernt wurde
            _mediaRepositoryMock.Verify(mr => mr.DeleteImage(It.Is<Media>(m => m.Id == 2)), Times.Once);
            _mediaRepositoryMock.Verify(mr => mr.RemoveBlogPostMedia(It.Is<BlogPostMedia>(bpm => bpm.MediaId == 2)), Times.Once);

            existingBlogPost.BlogPostMedias.Should().NotContain(bpm => bpm.MediaId == 2);

            // Überprüfen, dass das neue Bild hinzugefügt wurde
            _mediaRepositoryMock.Verify(mr => mr.SaveImageAsync(It.IsAny<IFormFile>(), blogPostId), Times.Once);
            _mediaRepositoryMock.Verify(mr => mr.AddBlogPostMediaAsync(blogPostId, 3), Times.Once);

            existingBlogPost.BlogPostMedias.Should().Contain(bpm => bpm.MediaId == 3);

            // Überprüfen, dass Update und SaveChanges aufgerufen wurden
            _blogPostRepoMock.Verify(repo => repo.Update(existingBlogPost), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        private IFormFile GetMockFormFile(string fileName)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Dummy file content"));
            return new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }


        [Fact]
        public async Task UpdateBlogPostWithImagesAsync_ErrorDeletingMedia_ThrowsBlogPostUpdateException()
        {
            // Arrange
            int blogPostId = 1;
            var updateDto = new BlogPostEditWithImagesDto
            {
                BlogPostId = blogPostId,
                Title = "Updated Blog Post",
                Description = "Updated description",
                Tags = new List<string> { "UpdatedTag" },
                IsPublished = true,
                ExistingImages = new List<BlogPostImageDto>
        {
            new BlogPostImageDto
            {
                MediaId = 2,
                Url = "http://example.com/image2.jpg",
                ToDelete = true
            }
        },
                NewImages = new List<IFormFile>()
            };

            var existingBlogPost = new BlogPost
            {
                Id = blogPostId,
                Title = "Original Blog Post",
                Description = "Original description",
                IsPublished = false,
                BlogPostMedias = new List<BlogPostMedia>
        {
            new BlogPostMedia
            {
                MediaId = 2,
                Media = new Media
                {
                    Id = 2,
                    Url = "http://example.com/image2.jpg",
                    FileName = "image2.jpg",
                    MediaType = MediaType.Image,
                    ContentType = "image/jpeg",
                    Description = "Image 2",
                    FileSize = 2048
                },
                Order = 1
            }
        }
            };

            _blogPostRepoMock.Setup(repo => repo.GetByIdAsync(blogPostId))
                .ReturnsAsync(existingBlogPost);

            _blogPostRepoMock.Setup(repo => repo.LoadBlogPostMediasAsync(existingBlogPost))
                .Returns(Task.CompletedTask);

            _mediaRepositoryMock.Setup(mr => mr.DeleteImage(It.IsAny<Media>()))
                .Throws(new Exception("Error deleting image"));

            // Act & Assert
            await Assert.ThrowsAsync<BlogPostUpdateException>(() => _service.UpdateBlogPostWithImagesAsync(updateDto));

            // Überprüfen, dass die Transaktion zurückgesetzt wurde
            _blogPostRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mediaRepositoryMock.Verify(mr => mr.DeleteImage(It.Is<Media>(m => m.Id == 2)), Times.Once);
            _mediaRepositoryMock.Verify(mr => mr.RemoveBlogPostMedia(It.IsAny<BlogPostMedia>()), Times.Never);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fehler beim Löschen des Mediums mit ID 2 während der Aktualisierung des BlogPosts.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }



        #endregion

        #region DeleteBlogPostAsync Tests

        [Fact]
        public async Task DeleteBlogPostAsync_ExistingId_DeletesBlogPost()
        {
            // Arrange
            int blogPostId = 1;
            var blogPost = new BlogPost
            {
                Id = blogPostId,
                Title = "Test Blog Post",
                Description = "Detailed description",
                IsPublished = true,
                BlogPostMedias = new List<BlogPostMedia>
                {
                    new BlogPostMedia
                    {
                        MediaId = 1,
                        Media = new Media
                        {
                            Id = 1,
                            Url = "http://example.com/image1.jpg",
                            FileName = "image1.jpg",
                            MediaType = MediaType.Image,
                            ContentType = "image/jpeg",
                            Description = "Image 1",
                            FileSize = 1024
                        },
                        Order = 1
                    }
                }
            };

            _blogPostRepoMock.Setup(repo => repo.GetByIdAsync(blogPostId))
                .ReturnsAsync(blogPost);

            _blogPostRepoMock.Setup(repo => repo.RemoveBlogPostMedia(It.IsAny<BlogPostMedia>()))
                .Callback<BlogPostMedia>(bpm => blogPost.BlogPostMedias.Remove(bpm));

            _mediaServiceMock.Setup(m => m.DeleteMediaAsync(1))
                .Returns(Task.CompletedTask);

            _blogPostRepoMock.Setup(repo => repo.DeleteBlogPost(blogPost))
                .Callback<BlogPost>(bp => _context.BlogPosts.Remove(bp));

            // Setup SaveChangesAsync
            _context.SaveChangesAsync().GetAwaiter();

            // Act
            await _service.DeleteBlogPostAsync(blogPostId);

            // Assert
            _blogPostRepoMock.Verify(repo => repo.GetByIdAsync(blogPostId), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.RemoveBlogPostMedia(It.IsAny<BlogPostMedia>()), Times.Once);
            _mediaServiceMock.Verify(m => m.DeleteMediaAsync(1), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.DeleteBlogPost(blogPost), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _context.BlogPosts.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteBlogPostAsync_NonExistingId_ThrowsBlogPostNotFoundException()
        {
            // Arrange
            int blogPostId = 99;

            _blogPostRepoMock.Setup(repo => repo.GetByIdAsync(blogPostId))
                .ReturnsAsync((BlogPost)null!);

            // Act
            Func<Task> act = async () => await _service.DeleteBlogPostAsync(blogPostId);

            // Assert
            var exception = await Assert.ThrowsAsync<BlogPostNotFoundException>(act);
            exception.ErrorCode.Should().Be(BlogPostErrorCode.NotFound);
            exception.Message.Should().Be($"BlogPost with ID {blogPostId} not found.");
            exception.BlogPostId.Should().Be(blogPostId);

            _blogPostRepoMock.Verify(repo => repo.GetByIdAsync(blogPostId), Times.Once);
            _blogPostRepoMock.Verify(repo => repo.RemoveBlogPostMedia(It.IsAny<BlogPostMedia>()), Times.Never);
            _mediaServiceMock.Verify(m => m.DeleteMediaAsync(It.IsAny<int>()), Times.Never);
            _blogPostRepoMock.Verify(repo => repo.DeleteBlogPost(It.IsAny<BlogPost>()), Times.Never);
            _blogPostRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region GetAllBlogPostsAsync Tests
        // TODO Tests Erstellen

        #endregion

        #region GetPublishedBlogPostsAsync Tests
        // TODO Tests Erstellen
       
        #endregion
    }
}
