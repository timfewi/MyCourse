using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.Validation.DtoValidations.BlogPostDtoValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using MyCourse.Domain.Enums;

namespace MyCourse.Tests.UnitTests.Domain.Validators.DTOs.BlogPostDtoValidators
{
    public class BlogPostCreateDtoValidatorTests
    {
        private readonly BlogPostCreateDtoValidator _validator;

        public BlogPostCreateDtoValidatorTests()
        {
            _validator = new BlogPostCreateDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var model = new BlogPostCreateDto { Title = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_MaxLength()
        {
            var model = new BlogPostCreateDto { Title = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title cannot exceed 200 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var model = new BlogPostCreateDto { Description = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("Description is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Tags_Are_Duplicate()
        {
            var model = new BlogPostCreateDto
            {
                Tags = new List<string> { "Art", "Drawing", "Art" }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Tags)
                  .WithErrorMessage("Duplicate tags are not allowed.");
        }

        [Fact]
        public void Should_Have_Error_When_Tags_Exceed_Maximum()
        {
            var model = new BlogPostCreateDto
            {
                Tags = new List<string> { "Art", "Drawing", "Painting", "Sculpture", "Photography", "Digital" }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Tags)
                  .WithErrorMessage("A maximum of 5 tags can be used.");
        }

        [Fact]
        public void Should_Have_Error_When_Medias_Are_Empty()
        {
            var model = new BlogPostCreateDto { Medias = new List<BlogPostMediaCreateDto>() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Medias)
                  .WithErrorMessage("At least one media item is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Medias_Exceed_Maximum()
        {
            var model = new BlogPostCreateDto
            {
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                Medias = new List<BlogPostMediaCreateDto>()
            };

            // Add 11 media items to exceed the maximum of 10
            for (int i = 1; i <= 21; i++)
            {
                model.Medias.Add(new BlogPostMediaCreateDto
                {
                    Url = $"http://example.com/image{i}.jpg",
                    FileName = $"image{i}.jpg",
                    MediaType = MediaType.Image,
                    ContentType = "image/jpeg",
                    Description = $"Description for image {i}",
                    FileSize = 1024 * i,
                    Caption = $"Caption {i}"
                });
            }

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Medias)
                  .WithErrorMessage("A maximum of 20 media items can be uploaded.");
        }

        [Fact]
        public void Should_Have_Error_When_Media_Has_Invalid_Url()
        {
            var model = new BlogPostCreateDto
            {
                Medias = new List<BlogPostMediaCreateDto>
                {
                    new BlogPostMediaCreateDto
                    {
                        Url = "invalid-url",
                        FileName = "image1.jpg",
                        MediaType = MediaType.Image,
                        ContentType = "image/jpeg",
                        Description = "Description for image 1",
                        FileSize = 1024,
                        Caption = "Caption 1"
                    }
                }
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor("Medias[0].Url")
                  .WithErrorMessage("Media URL is invalid.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = new BlogPostCreateDto
            {
                Title = "Valid Title",
                Description = "This is a valid description that is more than fifty characters long.",
                Tags = new List<string> { "Art", "Drawing" },
                IsPublished = true,
                Medias = new List<BlogPostMediaCreateDto>
                {
                    new BlogPostMediaCreateDto
                    {
                        Url = "http://example.com/image1.jpg",
                        FileName = "image1.jpg",
                        MediaType = MediaType.Image,
                        ContentType = "image/jpeg",
                        Description = "Description for image 1",
                        FileSize = 1024,
                        Caption = "Caption 1"
                    },
                    new BlogPostMediaCreateDto
                    {
                        Url = "http://example.com/image2.png",
                        FileName = "image2.png",
                        MediaType = MediaType.Image,
                        ContentType = "image/png",
                        Description = "Description for image 2",
                        FileSize = 2048,
                        Caption = "Caption 2"
                    }
                }
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
