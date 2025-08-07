namespace Domain.Entities
{
    public class WorkerFunction: BaseEntity
    {
        public Guid WorkerProfileId { get; set; }
        public virtual WorkerProfile WorkerProfile { get; set; }
        public Guid ProfessionId { get; set; }
        public virtual Profession Profession { get; set; }
        public Guid MeasurementUnitId { get; set; }
        public virtual MeasurementUnit MeasurementUnit { get; set; }
        public virtual ICollection<WorkerFunctionSpecialization> WorkerFunctionSpecializations { get; set; }
        public double Price { get; set; }
    }
}
