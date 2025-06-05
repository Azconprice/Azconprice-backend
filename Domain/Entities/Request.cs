using Domain.Enums;

namespace Domain.Entities
{
    public class Request : BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public RequestType Type { get; set; }
        public string Note { get; set; }
    }
}
