using Application.Models.DTOs;
using FluentValidation;

namespace Application.Validators.User
{
    public class SendPhoneVerificationRequestValidator : AbstractValidator<SendPhoneVerificationRequest>
    {
        // Accepts +994XXXXXXXXX, +994 XX XXX XX XX, +994-XX-XXX-XX-XX, etc.
        private const string AzerbaijanPhoneRegex = @"^\+994[-\s]?(10|50|51|55|60|70|77|99)[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$";

        public SendPhoneVerificationRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(AzerbaijanPhoneRegex)
                .WithMessage("Phone number must be a valid Azerbaijani number, e.g. +994502123456, +994 50 212 34 56, or +994-50-212-34-56.");
        }
    }
}