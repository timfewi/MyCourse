using FluentValidation;
using MyCourse.Domain.Entities;

public class MediaValidator : AbstractValidator<Media>
{
    public MediaValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Url is required.")
            .Must(BeAValidRelativeUrl).WithMessage("Invalid Url format.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required.");

        RuleFor(x => x.MediaType)
            .IsInEnum().WithMessage("Invalid media type.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required.")
            .Matches(@"^[-\w]+/[-\w]+$").WithMessage("Invalid ContentType format.");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("FileSize must be greater than 0.")
            .LessThanOrEqualTo(25 * 1024 * 1024).WithMessage("FileSize must be less than or equal to 25MB.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _) || Uri.TryCreate(url, UriKind.Relative, out _);
    }
    private bool BeAValidRelativeUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Relative, out _);
    }
}
