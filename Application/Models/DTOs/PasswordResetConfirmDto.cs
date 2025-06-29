using Domain.Enums;

namespace Application.Models.DTOs
{
    public class PasswordResetConfirmDto
    {
        public string Contact { get; set; }
        public string ContactType { get; set; }
        public string NewPassword { get; set; }
    }
}
