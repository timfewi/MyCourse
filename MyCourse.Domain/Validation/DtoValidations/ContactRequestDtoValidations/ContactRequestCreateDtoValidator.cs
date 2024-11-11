using FluentValidation;
using MyCourse.Domain.DTOs.ContactRequestDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.DtoValidations.ContactRequestDtoValidations
{
    public class ContactRequestCreateDtoValidator : AbstractValidator<ContactRequestCreateDto>
    {
        public ContactRequestCreateDtoValidator()
        {
            RuleFor(x => x.Name)
              .NotEmpty().WithMessage("Name is required.")
              .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.")
                .EmailAddress().WithMessage("Email is not valid.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(200).WithMessage("Subject cannot exceed 200 characters.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000).WithMessage("Subject cannot exceed 2000 characters.");
        }
    }
}
