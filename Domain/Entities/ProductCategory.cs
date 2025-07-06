namespace Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
    }
}
