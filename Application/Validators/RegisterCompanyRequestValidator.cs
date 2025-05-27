using Application.Models.DTOs.Company;
using FluentValidation;

namespace Application.Validators
{
    public class RegisterCompanyRequestValidator : AbstractValidator<RegisterCompanyRequest>
    {
        private const string AzerbaijanPhoneRegex =
            @"^\+994([ -]?)(10|50|51|55|60|70|77|99)([ -]?\d{3})([ -]?\d{2})([ -]?\d{2})$";

        public RegisterCompanyRequestValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(AzerbaijanPhoneRegex)
                .WithMessage("Phone number must be a valid Azerbaijani number, e.g. +994502123456, +994 50 212 34 56, or +994-50-212-34-56.");

            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("Tax ID is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");
        }
    }
}