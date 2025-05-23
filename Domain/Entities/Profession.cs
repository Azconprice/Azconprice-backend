namespace Domain.Entities
{
    public class Profession : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Specialization> Specializations { get; set; }
    }
}
