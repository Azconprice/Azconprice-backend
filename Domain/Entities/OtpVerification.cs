using Domain.Enums;

namespace Domain.Entities
{
    public class OtpVerification : BaseEntity
    {
        public string Code { get; set; } = default!;

        // Changed from PhoneNumber to general Contact
        public string Contact { get; set; } = default!; // phone or email

        public DateTime ExpirationDate { get; set; }
        public bool IsVerified { get; set; }

        // Optional: Enum to indicate contact type
        public ContactType ContactType { get; set; }
    }
}
