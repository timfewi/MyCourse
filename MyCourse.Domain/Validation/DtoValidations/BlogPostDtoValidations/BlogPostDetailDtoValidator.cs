using FluentValidation;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.DtoValidations.BlogPostDtoValidations
{
    public class BlogPostDetailDtoValidator : AbstractValidator<BlogPostDetailDto>
    {
        public BlogPostDetailDtoValidator()
        {
            // Id Validation
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("BlogPost ID must be greater than zero.");

            // Title Validation
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            // Description Validation
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty.");

            // Tags Validation
            RuleFor(x => x.Tags)
                .Must(tags => tags.Distinct(StringComparer.OrdinalIgnoreCase).Count() == tags.Count)
                    .WithMessage("Duplicate tags are not allowed.")
                .Must(tags => tags.Count <= 5)
                    .WithMessage("A maximum of 5 tags can be used.");

            RuleForEach(x => x.Tags)
                .NotEmpty().WithMessage("Tags cannot be empty.")
                .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");

            // Medias Validation
            RuleFor(x => x.Medias)
                .NotEmpty().WithMessage("At least one media item is required.")
                .Must(medias => medias.Count <= 20).WithMessage("A maximum of 20 media items can be associated.");

            RuleForEach(x => x.Medias)
                .SetValidator(new BlogPostMediaDetailDtoValidator());

            // PublishedDate Validation (optional)
            RuleFor(x => x.PublishedDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Published date cannot be in the future.")
                .When(x => x.IsPublished);
        }
    }

    public class BlogPostMediaDetailDtoValidator : AbstractValidator<BlogPostMediaDetailDto>
    {
        public BlogPostMediaDetailDtoValidator()
        {
            // MediaId Validation
            RuleFor(x => x.MediaId)
                .GreaterThan(0).WithMessage("Media ID must be greater than zero.");

            // Url Validation
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Media URL is required.")
                .Must(BeAValidUrl).WithMessage("Media URL is invalid.");

            // FileName Validation
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters.");

            // MediaType Validation
            RuleFor(x => x.MediaType)
                .IsInEnum().WithMessage("Invalid media type.");

            // ContentType Validation
            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required.")
                .MaximumLength(100).WithMessage("Content type cannot exceed 100 characters.");

            // FileSize Validation
            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("File size must be greater than zero.")
                .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size cannot exceed 10 MB.");

            // Caption Validation (optional)
            RuleFor(x => x.Caption)
                .MaximumLength(500).WithMessage("Caption cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Caption));

            // Order Validation
            RuleFor(x => x.Order)
                .GreaterThan(0).WithMessage("Order must be greater than zero.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
