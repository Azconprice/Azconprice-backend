namespace Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; }
        public Guid ProfessionId { get; set; }
        public virtual Profession Profession { get; set; }
        public virtual ICollection<WorkerSpecialization> WorkerSpecializations { get; set; }
    }
}