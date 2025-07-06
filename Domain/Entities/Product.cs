using Domain.Enums;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Guid CategoryId { get; set; }
        public virtual ProductCategory Category { get; set; }
        public ProductType Type { get; set; }
        public Guid MeasurmentUnitId { get; set; }
        public virtual MeasurementUnit MeasurementUnit { get; set; }
    }
}
