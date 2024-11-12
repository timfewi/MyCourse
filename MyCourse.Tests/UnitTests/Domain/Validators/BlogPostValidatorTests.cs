using FluentValidation.TestHelper;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using MyCourse.Domain.Validation.EntityValidations;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyCourse.Tests.UnitTests.Domain.Validators
{
    public class BlogPostValidatorTests
    {
        private readonly BlogPostValidator _validator;

        public BlogPostValidatorTests()
        {
            _validator = new BlogPostValidator();
        }

        private BlogPost CreateValidBlogPost()
        {
            return new BlogPost
            {
                Title = "Valid Title",
                Description = "This is a valid description with more than fifty characters to pass the validation.",
                BlogPostMedias = new List<BlogPostMedia>
                {
                    new BlogPostMedia
                    {
                        Media = new Media
                        {
                            Url = TestConstants.ValidMediaUrl,
                            MediaType = MediaType.Image
                        }
                    }
                },
                Tags = new List<string> { "Tag1", "Tag2" },
                IsPublished = false
            };
        }

        // Title Validation Tests
        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Title = "";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_MaxLength()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Title = new string('A', 201);

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title cannot exceed 200 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_Valid()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Title = "A valid title";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        // Description Validation Tests
        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Description = "";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("Description is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Valid()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Description = "A valid description that is sufficiently long.";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        // BlogPostMedias Validation Tests
        [Fact]
        public void Should_Have_Error_When_BlogPostMedias_Is_Empty()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.BlogPostMedias = new List<BlogPostMedia>();

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BlogPostMedias)
                  .WithErrorMessage("At least one media item is required.");
        }

        [Fact]
        public void Should_Have_Error_When_BlogPostMedias_Exceeds_Maximum()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.BlogPostMedias = new List<BlogPostMedia>();
            for (int i = 0; i < 21; i++)
            {
                blogPost.BlogPostMedias.Add(new BlogPostMedia
                {
                    Media = new Media
                    {
                        Url = $"https://example.com/media{i}.jpg",
                        MediaType = MediaType.Image
                    }
                });
            }

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BlogPostMedias)
                  .WithErrorMessage("A maximum of 20 media items can be uploaded.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_BlogPostMedias_Is_Within_Limit()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.BlogPostMedias = new List<BlogPostMedia>();
            for (int i = 0; i < 20; i++)
            {
                blogPost.BlogPostMedias.Add(new BlogPostMedia
                {
                    Media = new Media
                    {
                        Url = $"https://example.com/media{i}.jpg",
                        MediaType = MediaType.Image
                    }
                });
            }

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.BlogPostMedias);
        }

        [Fact]
        public void Should_Have_Errors_For_Invalid_BlogPostMedias()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.BlogPostMedias = new List<BlogPostMedia>
            {
                new BlogPostMedia
                {
                    Media = null! 
                },
                new BlogPostMedia
                {
                    Media = new Media
                    {
                        Url = TestConstants.InvalidMediaUrl,
                        MediaType = (MediaType)999 
                    }
                }
            };

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor("BlogPostMedias[0].Media")
                  .WithErrorMessage("Media cannot be null.");

            result.ShouldHaveValidationErrorFor("BlogPostMedias[1].Media.Url")
                  .WithErrorMessage("Media URL is invalid.");

            result.ShouldHaveValidationErrorFor("BlogPostMedias[1].Media.MediaType")
                  .WithErrorMessage("Invalid media type.");
        }


        // Tags Validation Tests
        [Fact]
        public void Should_Have_Error_When_Duplicate_Tags_Are_Present()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Tags = new List<string> { "Tag1", "Tag1" };

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tags)
                  .WithErrorMessage("Duplicate tags are not allowed.");
        }

        [Fact]
        public void Should_Have_Error_When_Tag_Count_Exceeds_Maximum()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Tags = new List<string> { "Tag1", "Tag2", "Tag3", "Tag4", "Tag5", "Tag6" };

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tags)
                  .WithErrorMessage("A maximum of 5 tags can be used.");
        }

        [Fact]
        public void Should_Have_Error_When_Tag_Is_Empty()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Tags = new List<string> { "Tag1", "" };

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor("Tags[1]")
                  .WithErrorMessage("Tags cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_Tag_Exceeds_MaxLength()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Tags = new List<string> { "A valid tag", new string('B', 51) };

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor("Tags[1]")
                  .WithErrorMessage("Each tag cannot exceed 50 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Tags_Are_Valid()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.Tags = new List<string> { "Tag1", "Tag2", "Tag3" };

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            foreach (var tag in blogPost.Tags)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Tags);
            }
        }

        // IsPublished-dependent Rules Tests
        [Fact]
        public void Should_Have_Error_When_IsPublished_And_Title_Is_Whitespace()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.IsPublished = true;
            blogPost.Title = "   ";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Published posts must have a valid title.");
        }

        [Fact]
        public void Should_Have_Error_When_IsPublished_And_Description_Is_Short()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.IsPublished = true;
            blogPost.Description = "Too short";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("Description must be at least 50 characters long to be published.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_IsPublished_And_Title_And_Description_Are_Valid()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.IsPublished = true;
            blogPost.Title = "A valid published title";
            blogPost.Description = "This is a sufficiently long description that meets the minimum character requirement.";

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Apply_IsPublished_Rules_When_Not_Published()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();
            blogPost.IsPublished = false;
            blogPost.Title = ""; // Normally invalid, but should be allowed when not published
            blogPost.Description = ""; // Normally invalid, but should be allowed when not published

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            // General rules still apply
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required.");
            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("Description is required.");
        }

        // Overall Valid BlogPost Test
        [Fact]
        public void Should_Pass_Validation_For_Valid_BlogPost()
        {
            // Arrange
            var blogPost = CreateValidBlogPost();

            // Act
            var result = _validator.TestValidate(blogPost);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
