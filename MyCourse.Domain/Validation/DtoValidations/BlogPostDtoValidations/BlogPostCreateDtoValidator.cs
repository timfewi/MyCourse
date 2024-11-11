using FluentValidation;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.DtoValidations.BlogPostDtoValidations
{
    public class BlogPostCreateDtoValidator : AbstractValidator<BlogPostCreateDto>
    {
        public BlogPostCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Tags)
                .Must(tags => tags.Distinct().Count() == tags.Count).WithMessage("Duplicate tags are not allowed.")
                .Must(tags => tags.Count <= 5).WithMessage("A maximum of 5 tags can be used.");

            RuleForEach(x => x.Tags)
                .NotEmpty().WithMessage("Tags cannot be empty.")
                .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");

            RuleFor(x => x.Medias)
                .NotEmpty().WithMessage("At least one media item is required.")
                .Must(medias => medias.Count <= 20).WithMessage("A maximum of 20 media items can be uploaded.");

            RuleForEach(x => x.Medias)
                .SetValidator(new BlogPostMediaCreateDtoValidator());

            When(x => x.IsPublished, () =>
            {
                RuleFor(x => x.Title)
                    .Must(title => !string.IsNullOrWhiteSpace(title))
                        .WithMessage("Published posts must have a valid title.");

                RuleFor(x => x.Description)
                    .Must(description => description.Length >= 50)
                        .WithMessage("Description must be at least 50 characters long to be published.");
            });
        }
    }

    public class BlogPostMediaCreateDtoValidator : AbstractValidator<BlogPostMediaCreateDto>
    {
        public BlogPostMediaCreateDtoValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Media URL is required.")
                .Must(BeAValidUrl).WithMessage("Media URL is invalid.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.");

            RuleFor(x => x.MediaType)
                .IsInEnum().WithMessage("Invalid media type.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required.");

            RuleFor(x => x.FileSize)
                         .GreaterThan(0).WithMessage("File size must be greater than zero.")
                         .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size cannot exceed 10 MB.");

            RuleFor(x => x.Caption)
                .MaximumLength(500).WithMessage("Caption cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Caption));

        }
        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }

}
