using FluentValidation;
using MyCourse.Domain.DTOs.MediaDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.DtoValidations.MediaDtoValidations
{
    public class MediaCreateDtoValidator : AbstractValidator<MediaCreateDto>
    {
        public MediaCreateDtoValidator()
        {
            RuleFor(x => x.Url)
               .NotEmpty().WithMessage("Die URL darf nicht leer sein.")
               .Must(BeAValidUrl).WithMessage("Die URL ist ungültig.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Der Dateiname darf nicht leer sein.");

            RuleFor(x => x.MediaType)
                .IsInEnum().WithMessage("Der Medientyp ist ungültig.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Der Inhaltstyp darf nicht leer sein.")
                .Must(BeAValidContentType).WithMessage("Der Inhaltstyp ist ungültig.");

            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("Die Dateigröße muss größer als 0 sein.")
                .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("Die Datei darf nicht größer als 10 MB sein.");

            // Optional: Weitere Validierungen für Beschreibung, falls erforderlich
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _);
        }

        private bool BeAValidContentType(string contentType)
        {
            // Definiere erlaubte Inhaltstypen
            var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
            return !string.IsNullOrEmpty(contentType) && permittedContentTypes.Contains(contentType);
        }
    }
}
