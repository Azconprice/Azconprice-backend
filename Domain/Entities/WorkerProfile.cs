namespace Domain.Entities
{
    public class WorkerProfile : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public bool HaveTaxId { get; set; }
        public string? TaxId { get; set; }
        public string Address { get; set; }
        public int Experience { get; set; }
        public double Price { get; set; }
        public string? ProfilePicture { get; set; }
    }
}