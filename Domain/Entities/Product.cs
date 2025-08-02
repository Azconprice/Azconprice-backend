using Domain.Enums;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public Guid SalesCategoryId { get; set; }
        public virtual SalesCategory Category { get; set; }
        public ProductType Type { get; set; }
        public Guid MeasurmentUnitId { get; set; }
        public virtual MeasurementUnit MeasurementUnit { get; set; }
        public Guid CompanyProfileId { get; set; }
        public virtual CompanyProfile CompanyProfile { get; set; }
    }
}
