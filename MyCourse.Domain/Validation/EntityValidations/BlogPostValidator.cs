using FluentValidation;
using MyCourse.Domain.Entities;
using System;

namespace MyCourse.Domain.Validation.EntityValidations
{
    public class BlogPostValidator : AbstractValidator<BlogPost>
    {
        public BlogPostValidator()
        {
            // Title Validation
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            // Description Validation
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            // Medien-Validierung
            RuleFor(x => x.BlogPostMedias)
                .NotEmpty().WithMessage("At least one media item is required.")
                .Must(medias => medias.Count <= 20).WithMessage("A maximum of 20 media items can be uploaded.");

            RuleForEach(x => x.BlogPostMedias)
                .SetValidator(new BlogPostMediaValidator());

            // Tags Validation
            RuleFor(x => x.Tags)
                .Must(tags => tags.Distinct().Count() == tags.Count).WithMessage("Duplicate tags are not allowed.")
                .Must(tags => tags.Count <= 5).WithMessage("A maximum of 5 tags can be used.");

            RuleForEach(x => x.Tags)
                .NotEmpty().WithMessage("Tags cannot be empty.")
                .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");

            When(x => x.IsPublished, () =>
            {
                RuleFor(x => x.Title)
                    .Must(title => !string.IsNullOrWhiteSpace(title)).WithMessage("Published posts must have a valid title.");

                RuleFor(x => x.Description)
                    .Must(description => description.Length >= 50).WithMessage("Description must be at least 50 characters long to be published.");
            });
        }

    }
    public class BlogPostMediaValidator : AbstractValidator<BlogPostMedia>
    {
        public BlogPostMediaValidator()
        {
            RuleFor(x => x.Media)
                .NotNull().WithMessage("Media cannot be null.");

            RuleFor(x => x.Media.Url)
                .NotEmpty().WithMessage("Media URL is required.")
                .Must(BeAValidUrl).WithMessage("Media URL is invalid.")
                .When(x => x.Media != null);

            RuleFor(x => x.Media.MediaType)
                .IsInEnum().WithMessage("Invalid media type.")
                .When(x => x.Media != null);
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }

}
