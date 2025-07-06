namespace Domain.Entities
{
    public class MeasurementUnit : BaseEntity
    {
        public string Unit { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
    }
}
