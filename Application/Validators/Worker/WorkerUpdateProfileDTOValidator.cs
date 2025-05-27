using Application.Models.DTOs.Worker;
using FluentValidation;

namespace Application.Validators.Worker
{
    public class WorkerUpdateProfileDTOValidator : AbstractValidator<WorkerUpdateProfileDTO>
    {
        private const string AzerbaijanPhoneRegex =
            @"^\+994([ -]?)(10|20|21|22|23|24|25|26|27|28|29|50|51|55|60|70|77|99)([ -]?\d{3})([ -]?\d{2})([ -]?\d{2})$";

        public WorkerUpdateProfileDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                .When(x => x.FirstName != null)
                .WithMessage("First name is too long.");

            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .When(x => x.LastName != null)
                .WithMessage("Last name is too long.");

            RuleFor(x => x.PhoneNumber)
                .Matches(AzerbaijanPhoneRegex)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number must be a valid Azerbaijani number, e.g. +99411111111, +994 11 111 11 11, or +994-11-111-11-11.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Experience.HasValue)
                .WithMessage("Experience must be non-negative.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price must be non-negative.");

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .When(x => x.Address != null)
                .WithMessage("Address is too long.");

            RuleFor(x => x.TaxId)
                .NotEmpty()
                .When(x => x.HaveTaxId == true)
                .WithMessage("Tax ID is required when HaveTaxId is true.");
        }
    }
}
