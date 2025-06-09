namespace Domain.Entities
{
    public class WorkerProfile : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool HaveTaxId { get; set; }
        public string? TaxId { get; set; }
        public string Address { get; set; }
        public int Experience { get; set; }
        public virtual ICollection<WorkerSpecialization> WorkerSpecialization { get; set; }
        public double Price { get; set; }
    }
}