using Domain.Enums;

namespace Application.Models.DTOs
{
    public class PasswordResetRequestDto
    {
        public string Contact { get; set; }
        public string ContactType { get; set; }
    }
}
