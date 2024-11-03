using FluentValidation;
using Microsoft.AspNetCore.Http;
using MyCourse.Domain.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.DtoValidations.CourseDtoValidations
{
    public class CourseEditWithImagesDtoValidator : AbstractValidator<CourseEditWithImagesDto>
    {
        public CourseEditWithImagesDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Der Titel ist erforderlich.")
                .MaximumLength(100).WithMessage("Der Titel darf maximal 100 Zeichen lang sein.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Die Beschreibung ist erforderlich.")
                .MaximumLength(1000).WithMessage("Die Beschreibung darf maximal 1000 Zeichen lang sein.");

            RuleFor(x => x.CourseDate)
                .NotEmpty().WithMessage("Das Kursdatum ist erforderlich.")
                .Must(date => date.Date >= DateTime.Today).WithMessage("Das Kursdatum muss in der Zukunft liegen.");

            RuleFor(x => x.CourseDuration)
                .NotEmpty().WithMessage("Die Kursdauer ist erforderlich.");

            RuleFor(x => x.MaxParticipants)
                .InclusiveBetween(1, 1000).WithMessage("Die maximale Teilnehmerzahl muss zwischen 1 und 1000 liegen.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Der Standort ist erforderlich.")
                .MaximumLength(200).WithMessage("Der Standort darf maximal 200 Zeichen lang sein.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Der Preis muss eine positive Zahl sein.");

            RuleForEach(x => x.NewImages)
                .Must(BeValidImage)
                // TODO Kurs darf maximal 10 bilder haben.
                .WithMessage("Nur gültige Bilddateien (jpg, jpeg, png, gif) mit maximal 5MB sind erlaubt.")
                  .When(x => x.NewImages != null && x.NewImages.Any());
            ;
        }

        private bool BeValidImage(IFormFile file)
        {
            if (file == null)
                return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return false;

            // Max 5MB
            if (file.Length > 5 * 1024 * 1024)
                return false;

            // Optional: Überprüfung des MIME-Typs
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedMimeTypes.Contains(file.ContentType))
                return false;

            return true;
        }
    }
}
