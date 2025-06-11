using Application.Models.DTOs.Company;
using FluentValidation;

namespace Application.Validators.Company
{
    public class UpdateCompanyProfileDTOValidator : AbstractValidator<UpdateCompanyProfileDTO>
    {
        private const string AzerbaijanPhoneRegex = @"^\+994[-\s]?(10|50|51|55|60|70|77|99)[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$";

        public UpdateCompanyProfileDTOValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .Matches(AzerbaijanPhoneRegex)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number must be a valid Azerbaijani number, e.g. +99411111111, +994 11 111 11 11, or +994-11-111-11-11.");

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .When(x=> !string.IsNullOrWhiteSpace(x.Address))
                .WithMessage("Address is too long.");

            RuleFor(x => x.SalesCategoryId)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.SalesCategoryId))
                .WithMessage("Sales category is required.");
        }
    }
}