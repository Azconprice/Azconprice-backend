namespace Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; }
        public Guid ProfessionId { get; set; }
        public Profession Profession { get; set; }
        public ICollection<UserSpecialization> UserSpecializations { get; set; }
    }
}