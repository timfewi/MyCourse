using FluentValidation;
using FluentValidation.AspNetCore;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.EntityValidations
{
    public class CourseValidator : AbstractValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.CourseDate)
                .GreaterThan(DateTime.Now).WithMessage("Course date must be in the future.");

            RuleFor(x => x.CourseDuration)
                .GreaterThan(TimeSpan.Zero).WithMessage("Course duration must be positive.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("Max participants must be greater than zero.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        }
    }
}