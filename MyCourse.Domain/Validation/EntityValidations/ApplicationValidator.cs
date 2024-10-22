using FluentValidation;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.EntityValidations
{
    public class ApplicationValidator : AbstractValidator<Application>
    {
        public ApplicationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must contain between 10 and 15 digits.");

            RuleFor(x => x.ApplicationDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Application date cannot be in the future.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status.");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("A valid CourseId is required.");
        }
    }
}
