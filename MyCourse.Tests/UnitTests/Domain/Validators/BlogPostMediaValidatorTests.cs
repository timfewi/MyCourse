using FluentValidation.TestHelper;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using MyCourse.Domain.Validation.EntityValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.Validators
{
    public class BlogPostMediaValidatorTests
    {
        private readonly BlogPostMediaValidator _validator;

        public BlogPostMediaValidatorTests()
        {
            _validator = new BlogPostMediaValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Media_Is_Null()
        {
            // Arrange
            var blogPostMedia = new BlogPostMedia { Media = null! };

            // Act
            var result = _validator.TestValidate(blogPostMedia);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Media)
                  .WithErrorMessage("Media cannot be null.");
        }

        [Fact]
        public void Should_Have_Error_When_Media_Url_Is_Empty()
        {
            // Arrange
            var blogPostMedia = new BlogPostMedia
            {
                Media = new Media { Url = "", MediaType = MediaType.Image }
            };

            // Act
            var result = _validator.TestValidate(blogPostMedia);

            // Assert
            result.ShouldHaveValidationErrorFor("Media.Url")
                  .WithErrorMessage("Media URL is required.");
        }

        [Theory]
        [InlineData("ftp://example.com/media.jpg")]
        [InlineData("http://example.com/invalid.com")]
        public void Should_Have_Error_When_Media_Url_Is_Invalid(string url)
        {
            // Arrange
            var blogPostMedia = new BlogPostMedia
            {
                Media = new Media { Url = url, MediaType = MediaType.Image }
            };

            // Act
            var result = _validator.TestValidate(blogPostMedia);

            // Assert
            result.ShouldHaveValidationErrorFor("Media.Url")
                  .WithErrorMessage("Media URL is invalid.");
        }

        [Theory]
        [InlineData("/media.jpg")]
        [InlineData("/media.png")]
        public void Should_Not_Have_Error_When_Media_Url_Is_Valid(string url)
        {
            // Arrange
            var blogPostMedia = new BlogPostMedia
            {
                Media = new Media { Url = url, MediaType = MediaType.Image }
            };

            // Act
            var result = _validator.TestValidate(blogPostMedia);

            // Assert
            result.ShouldNotHaveValidationErrorFor("Media.Url");
        }

        [Fact]
        public void Should_Have_Error_When_MediaType_Is_Invalid()
        {
            // Arrange
            var blogPostMedia = new BlogPostMedia
            {
                Media = new Media { Url = "http://example.com/media.jpg", MediaType = (MediaType)999 }
            };

            // Act
            var result = _validator.TestValidate(blogPostMedia);

            // Assert
            result.ShouldHaveValidationErrorFor("Media.MediaType")
                  .WithErrorMessage("Invalid media type.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_MediaType_Is_Valid()
        {
            // Arrange
            var blogPostMedia = new BlogPostMedia
            {
                Media = new Media { Url = "http://example.com/media.jpg", MediaType = MediaType.Video }
            };

            // Act
            var result = _validator.TestValidate(blogPostMedia);

            // Assert
            result.ShouldNotHaveValidationErrorFor("Media.MediaType");
        }
    }
}
