namespace Domain.Entities
{
    public class PhoneVerification : BaseEntity
    {
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsVerified { get; set; }
    }
}
