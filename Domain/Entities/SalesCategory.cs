namespace Domain.Entities
{
    public class SalesCategory : BaseEntity
    {
        public string Name { get; set; }
        public virtual IEnumerable<CompanyProfile> CompanyProfiles { get; set; }
    }
}
