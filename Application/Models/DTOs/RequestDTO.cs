using Domain.Enums;

namespace Application.Models.DTOs
{
    public class RequestDTO
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
    }
}
