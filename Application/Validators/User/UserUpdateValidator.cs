using Application.Models.DTOs.User;
using FluentValidation;

namespace Application.Validators.User
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
    {
        private const string AzerbaijanPhoneRegex =
           @"^\+994([ -]?)(10|20|21|22|23|24|25|26|27|28|29|50|51|55|60|70|77|99)([ -]?\d{3})([ -]?\d{2})([ -]?\d{2})$";

        public UserUpdateValidator()
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
        }
    }
}
