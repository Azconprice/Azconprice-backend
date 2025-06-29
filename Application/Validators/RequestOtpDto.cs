using Application.Models.DTOs;
using Application.Models.Enums;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Validators
{
    public class RequestOtpDtoValidator : AbstractValidator<RequestOtpDto>
    {
        public RequestOtpDtoValidator()
        {
            RuleFor(x => x.ContactType)
                .IsInEnum()
                .WithMessage("Contact type must be either 'Email' or 'Phone'.");

            RuleFor(x => x.ContactValue)
                .NotEmpty()
                .WithMessage("Contact value is required.")
                .MaximumLength(50)
                .WithMessage("Contact value cannot exceed 100 characters.")
                .Must((dto, value) => IsValidContact(dto.ContactType, value))
                .WithMessage("Invalid contact format for the selected contact type.");
        }

        private bool IsValidContact(ContactType type, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return type switch
            {
                ContactType.Email => new EmailAddressAttribute().IsValid(value),
                ContactType.Phone => Regex.IsMatch(value, @"^\+994[-\s]?(10|50|51|55|60|70|77|99)[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$"),
                _ => false
            };
        }
    }
}
