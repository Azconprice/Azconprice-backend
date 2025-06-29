using Application.Models.Enums;

namespace Application.Models.DTOs
{
    public class RequestOtpDto
    {
        public ContactType ContactType { get; set; }
        public string ContactValue { get; set; } = default!;
    }
}
