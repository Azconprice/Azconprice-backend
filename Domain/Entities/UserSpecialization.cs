using System;

namespace Domain.Entities
{
    public class UserSpecialization : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid SpecializationId { get; set; }
        public virtual Specialization Specialization { get; set; }
    }
}