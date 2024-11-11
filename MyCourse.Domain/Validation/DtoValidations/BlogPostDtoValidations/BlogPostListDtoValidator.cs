using FluentValidation;
using MyCourse.Domain.DTOs.BlogPostDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.DtoValidations.BlogPostDtoValidations
{
    public class BlogPostListDtoValidator : AbstractValidator<BlogPostListDto>
    {
        public BlogPostListDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("BlogPost ID must be greater than zero.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.ShortDescription)
                .MaximumLength(500).WithMessage("Short description cannot exceed 500 characters.");

            RuleForEach(x => x.Tags)
                .NotEmpty().WithMessage("Tags cannot be empty.")
                .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");

            RuleFor(x => x.ThumbnailUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ThumbnailUrl))
                .WithMessage("Thumbnail URL is invalid.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
