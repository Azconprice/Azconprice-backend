using Application.Models.DTOs;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Request
{
    public class RequestDTOValidator : AbstractValidator<RequestDTO>
    {
        private const string AzerbaijanPhoneRegex =
            @"^\+994([ -]?)(10|20|21|22|23|24|25|26|27|28|29|50|51|55|60|70|77|99)([ -]?\d{3})([ -]?\d{2})([ -]?\d{2})$";

        public RequestDTOValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(AzerbaijanPhoneRegex)
                .WithMessage("Phone number must be a valid Azerbaijani number, e.g. +994502123456, +994 50 212 34 56, or +994-50-212-34-56.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Request type is required.")
                .Must(type => Enum.TryParse<RequestType>(type, ignoreCase: true, out _))
                .WithMessage($"Request type must be one of: {string.Join(", ", Enum.GetNames(typeof(RequestType)))}.");

            RuleFor(x => x.Note)
                .MaximumLength(1000)
                .WithMessage("Note cannot exceed 1000 characters.");
        }
    }
}