
namespace Domain.Entities
{
    public class WorkerFunctionSpecialization : BaseEntity
    {
        public Guid WorkerFunctionId { get; set; }
        public virtual WorkerFunction WorkerFunction { get; set; }
        public Guid SpecializationId { get; set; }
        public virtual Specialization Specialization { get; set; }
    }
}