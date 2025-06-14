using System;

namespace Domain.Entities
{
    public class WorkerSpecialization : BaseEntity
    {
        public Guid WorkerProfileId { get; set; }
        public virtual WorkerProfile WorkerProfile { get; set; }
        public Guid SpecializationId { get; set; }
        public virtual Specialization Specialization { get; set; }
    }
}