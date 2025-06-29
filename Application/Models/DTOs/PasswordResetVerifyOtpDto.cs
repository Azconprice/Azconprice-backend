using Domain.Enums;

namespace Application.Models.DTOs
{
    public class PasswordResetVerifyOtpDto
    {
        public string Contact { get; set; }
        public string ContactType { get; set; }
        public string Otp { get; set; }
    }
}
