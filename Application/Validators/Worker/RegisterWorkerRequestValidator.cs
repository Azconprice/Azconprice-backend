using Application.Models.DTOs.Worker;
using FluentValidation;

namespace Application.Validators.Worker
{
    public class RegisterWorkerRequestValidator : AbstractValidator<RegisterWorkerRequest>
    {
        private const string AzerbaijanPhoneRegex = @"^\+994[-\s]?(10|50|51|55|60|70|77|99)[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$";
        public RegisterWorkerRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

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
                .WithMessage("Phone number must be in one of the following formats: +994102122908, +994 10 212 29 08, or +994-10-212-29-08.");

            RuleFor(x => x.ProfessionId)
                .NotEmpty().WithMessage("Profession ID is required.");


            RuleFor(x => x.Specizalizations)
                .NotNull().WithMessage("At least one specialization is required.")
                .Must(s => s.Any()).WithMessage("At least one specialization is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

            When(x => x.HaveTaxId, () =>
            {
                RuleFor(x => x.TaxId)
                    .NotEmpty().WithMessage("Tax ID is required when HaveTaxId is true.");
            });
        }
    }
}