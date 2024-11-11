using FluentValidation;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Validation.EntityValidations
{
    public class ContactRequestValidator : AbstractValidator<ContactRequest>
    {

        public ContactRequestValidator() 
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 100 characters.");
            
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(c => c.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(150).WithMessage("Subject cannot exceed 150 characters.");

            RuleFor(c => c.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.");

            RuleFor(c => c.AnswerMessage)
                .MaximumLength(2000).WithMessage("Answer message cannot exceed 2000 characters.");

            RuleFor(c => c.AnswerDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Answer date cannot be in the future.");


        }
    }
}
