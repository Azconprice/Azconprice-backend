namespace Domain.Entities
{
    public class CompanyProfile : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string TaxId { get; set; }
        public string Address { get; set; }
        public string? CompanyLogo { get; set; }
    }
}